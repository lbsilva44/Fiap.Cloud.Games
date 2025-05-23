
using Fiap.Cloud.Games.Application.DTOs.Jogo;
using Fiap.Cloud.Games.Application.Interfaces;
using Fiap.Cloud.Games.Domain.Entities;
using Fiap.Cloud.Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fiap.Cloud.Games.Application.Services;

public class PromocaoService(BaseDbContext context) : IPromocaoService
{
    public async Task<int> CriarPromocao(CriarPromocaoDto dto)
    {
        // validações básicas
        if (dto.DescontoPercentual is < 0 or > 100)
            throw new ArgumentException("Desconto deve estar entre 0 e 100 %.");

        if (dto.DataFimUtc <= dto.DataInicioUtc)
            throw new ArgumentException("Data Fim deve ser maior que Data Início.");

        if (dto.JogosIds is null || dto.JogosIds.Count == 0)
            throw new ArgumentException("Informe pelo menos um jogo para a promoção.");

        // confere se os jogos existem e estão ativos/publicados
        var jogosValidos = await context.Set<Jogo>()
            .Where(j => dto.JogosIds.Contains(j.Id) && j.Publicado && j.Ativo)
            .Select(j => j.Id)
            .ToListAsync();

        if (jogosValidos.Count == 0)
            throw new ArgumentException("Nenhum dos jogos informados é válido para promoção.");

        // cria a entidade
        var promocao = new Promocao(
            dto.Titulo,
            dto.Descricao,
            dto.DescontoPercentual,
            dto.DataInicioUtc.DateTime,
            dto.DataFimUtc.DateTime);

        foreach (var jogoId in jogosValidos)
            promocao.AdicionarJogo(jogoId);

        context.Add(promocao);
        await context.SaveChangesAsync();

        return promocao.Id;
    }

    public async Task AtivarPromocao(int id)
    {
        var promocao = await context.Set<Promocao>()
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new ArgumentException("Promoção não encontrada.");

        if (promocao.Ativa)
            throw new InvalidOperationException("Promoção já está ativa.");


        promocao.Ativar();
        await context.SaveChangesAsync();
    }

    public async Task ExcluirPromocao(int id)
    {
        var promocao = await context.Set<Promocao>()
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new ArgumentException("Promoção não encontrada.");

        context.Remove(promocao);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<PromocaoDto>> ListarPromocao(bool somenteAtivas, bool isAdmin)
    {
        var query = context.Promocoes
            .Include(p => p.Jogos).ThenInclude(pj => pj.Jogo)
            .AsQueryable();

        // Se não for Admin, SEMPRE filtra ativas e não expiradas
        // Se for Admin filtra somente se parametro = true
        if (!isAdmin || somenteAtivas)
            query = query.Where(p => p.Ativa && p.DataFim > DateTime.Now);

        return await query
            .Select(p => new PromocaoDto
            {
                Id = p.Id,
                Titulo = p.Titulo,
                Descricao = p.Descricao,
                DescontoPercentual = p.DescontoPercentual,
                DataInicioUtc = p.DataInicio,
                DataFimUtc = p.DataFim,
                Ativa = p.Ativa,
                Expirada = p.Expirada,
                Jogos = p.Jogos.Select(j => new JogoDto
                {
                    Id = j.Jogo.Id,
                    Nome = j.Jogo.Nome,
                    Descricao = j.Jogo.Descricao,
                    Preco = j.Jogo.Preco,
                    Tipo = j.Jogo.Tipo,
                    Ativo = j.Jogo.Ativo,
                    Publicado = j.Jogo.Publicado
                })
            })
            .ToListAsync();
    }

    public async Task AdicionarJogoPromocao(int promocaoId, int jogoId)
    {
        var promocao = await context.Promocoes
        .Include(p => p.Jogos)
        .FirstOrDefaultAsync(p => p.Id == promocaoId)
        ?? throw new ArgumentException("Promoção não encontrada.");

        // não pode mexer em promoção ativada
        if (promocao.Ativa)
            throw new InvalidOperationException("Promoção já ativada; não é possível adicionar novos jogos.");

        // não pode mexer se já expirou
        if (promocao.DataFim <= DateTime.Now)
            throw new InvalidOperationException("Promoção expirada; não é possível adicionar novos jogos.");

        // evita duplicar
        if (promocao.Jogos.Any(j => j.JogoId == jogoId))
            throw new ArgumentException("Esse jogo já está associado à promoção.");

        // confere se o jogo existe e está ativo/publicado
        bool jogoExiste = await context.Set<Jogo>()
            .AnyAsync(j => j.Id == jogoId && j.Ativo && j.Publicado);

        if (!jogoExiste)
            throw new ArgumentException("Jogo não encontrado ou inativo.");

        promocao.AdicionarJogo(jogoId);
        await context.SaveChangesAsync();
    }

    public async Task AdquirirPromocao(int usuarioId, int promocaoId)
    {
        // 1) Carrega usuário com tudo que precisa
        var usuario = await context.Usuarios
            .Include(u => u.Carteira)
            .Include(u => u.Movimentos)
            .Include(u => u.Bibliotecas)
            .FirstOrDefaultAsync(u => u.Id == usuarioId);

        if (usuario is null)
            throw new ArgumentException("Usuário não encontrado.");

        // 2) Carrega promoção e os jogos vinculados
        var promocao = await context.Promocoes
                        .Include(p => p.Jogos) // join table
                        .ThenInclude(pj => pj.Jogo)              // inclui o Jogo nesta associação
                        .FirstOrDefaultAsync(p => p.Id == promocaoId)
    ?? throw new ArgumentException("Promoção não encontrada.");

        if (promocao is null)
            throw new ArgumentException("Promoção não encontrada.");

        if (!promocao.Ativa)
            throw new InvalidOperationException("Promoção não está ativa.");

        var jogosPendentes = promocao.Jogos
                            .Select(pj => pj.Jogo!)
                            .Where(jogo => !usuario.Bibliotecas.Any(b => b.JogoId == jogo.Id))
                            .ToList();

        if (!jogosPendentes.Any())
            throw new InvalidOperationException("Usuário já possui todos os jogos da promoção.");

        // 4) Calcula o custo total com desconto
        decimal total = jogosPendentes
            .Sum(jogo => jogo.Preco * (1 - promocao.DescontoPercentual / 100m));

        // 5) Valida saldo **antes** de qualquer operação
        if (total > usuario.Carteira.Saldo)
            throw new InvalidOperationException($"Saldo insuficiente: custo total R$ {total:F2}, saldo R$ {usuario.Carteira.Saldo:F2}.");


        // 3) Para cada jogo da promoção
        foreach (var pj in promocao.Jogos)
        {
            var jogo = pj.Jogo!;           // extrai o Jogo da associação

            // 3.1) ignora duplicidade
            if (usuario.Bibliotecas.Any(b => b.JogoId == jogo.Id))
                continue;

            // 3.2) Preço original vem de jogo.Preco
            var precoOriginal = jogo.Preco;
            var precoComDesconto = precoOriginal * (1 - promocao.DescontoPercentual / 100m);

            // 3.3) Registra débito e compra
            usuario.ComprarJogo(jogo, precoComDesconto);
        }

        // 4) Salva tudo no banco
        await context.SaveChangesAsync();
    }
}
