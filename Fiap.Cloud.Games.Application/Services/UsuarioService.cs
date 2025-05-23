using Fiap.Cloud.Games.Application.Interfaces;
using Fiap.Cloud.Games.Domain.Entities;
using Fiap.Cloud.Games.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Fiap.Cloud.Games.Domain.Enums;
using Fiap.Cloud.Games.Application.DTOs.Usuario;
using Fiap.Cloud.Games.Application.DTOs.Jogo;

namespace Fiap.Cloud.Games.Application.Services;

public class UsuarioService(BaseDbContext context, IJwtService jwtService) : IUsuarioService
{
    public async Task<Usuario> RegistrarUsuario(RegistroUsuarioDto user)
    {
        // Verificar se o e-mail já está cadastrado
        if (await context.Usuarios.AnyAsync(u => u.Email.Valor == user.Email))throw new Exception("Email já cadastrado.");

        // Criação do usuário (validações de e-mail e senha ocorrem automaticamente nos Value Objects)
        var usuario = new Usuario(user.Nome, user.Email, user.Senha, 0);
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        return usuario;
    }

    public async Task<string?> LoginUsuario(LoginUsuarioDto user)
    {
        // Verificar se o usuario e senha tao cadastrado
        var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Email.Valor == user.Email);

        if (usuario is null || usuario.Senha.Valor != user.Senha)throw new ArgumentException("Usuário ou senha inválidos.");

        //Gerar token para Liberar Acesso
        var token = jwtService.GerarToken(usuario);
        return token;
    }

    public async Task<bool> AlterarAcessoUsuario(int idUsuario, string novaRole)
    {
        // Tenta localizar usuario
        var usuario = await context.Usuarios.FindAsync(idUsuario)?? throw new ArgumentException("Usuário não encontrado.");

        TipoAcesso roleValida;

        // Tenta parsear como número primeiro
        if (int.TryParse(novaRole, out var numero))
        {
            if (!Enum.IsDefined(typeof(TipoAcesso), numero))throw new ArgumentException("Tipo de acesso inválido. Aceito apenas 'Admin' (1) ou 'Usuario' (2).");

            roleValida = (TipoAcesso)numero;
        }
        else
        {
            // Se não for número, tenta como string
            if (!Enum.TryParse<TipoAcesso>(novaRole, true, out roleValida))throw new ArgumentException("Tipo de acesso inválido. Aceito apenas 'Admin' ou 'Usuario'.");
        }

        usuario.AlterarRole(roleValida.ToString());
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<UsuarioDto>> ListarUsuarios()
    {
        return await context.Usuarios
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Nome = u.Nome,
                Email = u.Email.Valor,
                Role = u.Role,
                Ativo = u.Ativo,
                DataCadastro = u.DataCadastro
            })
            .ToListAsync();
    }

    public async Task<bool> DesativarUsuario(int idUsuario)
    {
        var usuario = await context.Usuarios.FindAsync(idUsuario)?? throw new ArgumentException("Usuário não encontrado.");

        if (!usuario.Ativo)throw new InvalidOperationException("Usuário já está desativado.");

        usuario.Desativar(); // Usa o método da entidade Usuario que já existe
        await context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AtivarUsuario(int idUsuario)
    {
        var usuario = await context.Usuarios.FindAsync(idUsuario)?? throw new ArgumentException("Usuário não encontrado.");

        if (usuario.Ativo)throw new InvalidOperationException("Usuário já está ativo.");

        usuario.Ativar(); // Usa o método da entidade Usuario que já existe
        await context.SaveChangesAsync();

        return true;
    }

    public async Task Depositar(int usuarioId, decimal valor)
    {
        var usuario = await context.Usuarios
                                   .Include(u => u.Movimentos)
                                   .FirstOrDefaultAsync(u => u.Id == usuarioId)
                      ?? throw new ArgumentException("Usuário não encontrado.");

        usuario.Depositar(valor);
        await context.SaveChangesAsync();
    }

    public async Task<UsuarioDetalhesDto?> ObterDetalhesUsuario(int usuarioId)
    {
        var usuario = await context.Usuarios
                                   .AsNoTracking()
                                   .Include(u => u.Movimentos)
                                   .Include(u => u.Bibliotecas)          // navegação ↓
                                       .ThenInclude(b => b.Jogo)         // se existir prop Jogo
                                   .FirstOrDefaultAsync(u => u.Id == usuarioId);

        if (usuario is null) return null;

        return new UsuarioDetalhesDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email.Valor,          // ajuste ao seu VO
            DataCadastro = usuario.DataCadastro,
            Role = usuario.Role,
            Saldo = usuario.Carteira.Saldo,

            Biblioteca = usuario.Bibliotecas
                                .Select(b => new JogoBibliotecaDto
                                {
                                    JogoId = b.JogoId,
                                    Titulo = b.Jogo.Nome,
                                    Preco = b.Jogo.Preco,
                                    DataAquisicao = b.DataAquisicao
                                })
                                .ToList(),

            Movimentos = usuario.Movimentos
                                .OrderByDescending(m => m.DataHora)
                                .Select(m => new MovimentoCarteiraDto
                                {
                                    Id = m.Id,
                                    Tipo = m.Tipo.ToString(),
                                    Valor = m.Valor,
                                    SaldoAntes = m.SaldoAntes,
                                    SaldoDepois = m.SaldoDepois,
                                    JogoId = m.JogoId,
                                    DataHora = m.DataHora
                                })
                                .ToList()
        };
    }
}
