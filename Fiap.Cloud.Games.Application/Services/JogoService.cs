using Fiap.Cloud.Games.Domain.Entities;
using Fiap.Cloud.Games.Application.Interfaces;
using Fiap.Cloud.Games.Application.DTOs.Jogo;
using Fiap.Cloud.Games.Domain.Enums;
using Fiap.Cloud.Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Fiap.Cloud.Games.Application.Services;

public class JogoService(BaseDbContext context) : IJogoService
{
    public async Task CadastrarJogo(JogoDto dto)
    {
        if (!Enum.IsDefined(typeof(TipoJogo), dto.Tipo))
            throw new ArgumentException("Tipo de jogo inválido.");

        // 🔥 NORMALIZA nome para evitar espaços e caixa
        var nomeNormalizado = dto.Nome.Trim().ToLowerInvariant();

        var existe = await context.Set<Jogo>().AnyAsync(j => j.Nome.Trim().ToLower() == nomeNormalizado && j.Ativo);

        if (existe)throw new ArgumentException("Já existe um jogo cadastrado com este nome.");

        var jogo = new Jogo(dto.Nome, dto.Descricao, dto.Preco, dto.Tipo);

        context.Add(jogo);
        await context.SaveChangesAsync();
    }

    public async Task PublicarJogo(int id)
    {
        var jogo = await context.Set<Jogo>().FindAsync(id) ?? throw new ArgumentException("Jogo não encontrado.");

        if (jogo.Publicado) throw new InvalidOperationException("O jogo já está publicado.");

        jogo.Publicar(); // Método da entidade Jogo
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<JogoDto>> ListarJogos(string role)
    {
    var query = context.Set<Jogo>().AsQueryable();

    bool isAdmin = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase);

    if (!isAdmin)
    {
        query = query.Where(j => j.Publicado && j.Ativo);
    }

    var jogos = await query
        .Select(j => new JogoDto
        {
            Id = j.Id,
            Nome = j.Nome,
            Descricao = j.Descricao,
            Preco = j.Preco,
            Tipo = j.Tipo,
            Ativo = j.Ativo,
            // Admin vê status Ativo/Publicado, usuário comum não precisa
            Publicado = isAdmin ? j.Publicado : null
        })
        .ToListAsync();

    return jogos;
    }

    public async Task AdquirirJogo(int idJogo, int idUsuario)
    {
        var jogo = await context.Jogos
                                .FirstOrDefaultAsync(j => j.Id == idJogo && j.Publicado && j.Ativo)
                   ?? throw new Exception("Jogo não encontrado.");

        var usuario = await context.Usuarios
                                   .Include(u => u.Movimentos)
                                   .FirstOrDefaultAsync(u => u.Id == idUsuario)
                   ?? throw new Exception("Usuário não encontrado.");

        // ---------- VERIFICA SE JÁ POSSUI ----------
        var jaTem = await context.Bibliotecas
                                 .AnyAsync(b => b.UsuarioId == idUsuario && b.JogoId == idJogo);

        if (jaTem)
            throw new InvalidOperationException("Você já possui este jogo na sua biblioteca.");

        // 1) debita saldo e gera movimento
        usuario.ComprarJogo(jogo);

        // 2) registra o jogo na tabela Biblioteca
        context.Bibliotecas.Add(new Biblioteca(usuario.Id, jogo.Id));

        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<JogoDto>> ListarJogosAdquiridosUsuario(int idUsuario)
    {
        return await context.Set<Biblioteca>()
            .Where(b => b.UsuarioId == idUsuario)
            .Include(b => b.Jogo)
            .Select(b => new JogoDto
            {
                Id = b.JogoId,
                Nome = b.Jogo.Nome,
                Descricao = b.Jogo.Descricao,
                Preco = b.Jogo.Preco,
                Tipo = b.Jogo.Tipo,
                Ativo = b.Jogo.Ativo,
                Publicado = b.Jogo.Publicado
            })
            .ToListAsync();
    }

}