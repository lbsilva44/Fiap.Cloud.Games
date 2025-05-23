using Microsoft.AspNetCore.Mvc;
using Fiap.Cloud.Games.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Fiap.Cloud.Games.Domain.Enums;
using Fiap.Cloud.Games.Application.DTOs.Usuario;

namespace Fiap.Cloud.Games.API.Controllers;

public class UsuarioController(IUsuarioService usuarioService) : ControllerBase
{
    #region ── Acesso Publico ─────────────────────────────────────────────────────────────
    [HttpPost("Registrar-se")]
    [EndpointSummary("Registrar-se no sistema")]
    [EndpointDescription("Realizar seu registro no sistema (sempre como usuario comum)")]
    public async Task<IActionResult> Registrar([FromBody] RegistroUsuarioDto user)
    {
        await usuarioService.RegistrarUsuario(user);
        return Ok(new { Mensagem = "Usuário cadastrado com sucesso." });
    }

    [HttpPost("Login")]
    [EndpointSummary("Login no sistema")]
    [EndpointDescription("Realizar login no sistema com e-mail e senha")]
    public async Task<IActionResult> Login([FromBody] LoginUsuarioDto user)
    {
        var token = await usuarioService.LoginUsuario(user);
        return Ok(new{Mensagem = "Usuário logado com sucesso.",Token = token});
    }
    #endregion

    #region ── Acesso Admin ─────────────────────────────────────────────────────────────
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Alterar tipo de acesso do usuario")]
    [EndpointDescription("Alteração do tipo de acesso do usuario, somente o Admin pode atualizar")]
    [HttpPut("Alterar-acesso")]
    public async Task<IActionResult> AlterarAcessoUsuario(int idUsuario, [FromBody] AlterarAcessoUsuarioDto dto)
    {
        await usuarioService.AlterarAcessoUsuario(idUsuario, dto.NovaRole);
        return Ok(new { Mensagem = "Tipo Acesso do usuário alterado com sucesso." });
    }

    [Authorize(Roles = "Admin")]
    [EndpointSummary("Tipos de acesso do usuario")]
    [EndpointDescription("Lista quais tipos de acesso podem ter para atualizar, somente o admin pode ver")]
    [HttpGet("Tipo-acessos")]
    public IActionResult ListarRoles()
    {
        var roles = Enum.GetNames(typeof(TipoAcesso));
        return Ok(new { RolesDisponiveis = roles });
    }

    [Authorize(Roles = "Admin")]
    [EndpointSummary("Listar usuarios do sistema")]
    [EndpointDescription("Lista todos os usuarios cadastrado no sistema, somente o admin pode ver")]
    [HttpGet("Lista-usuarios")]
    public async Task<IActionResult> ListarUsuarios()
    {
        var usuarios = await usuarioService.ListarUsuarios();
        return Ok(usuarios);
    }

    [Authorize(Roles = "Admin")]
    [EndpointSummary("Desativar Usuario do sistema")]
    [EndpointDescription("Desativa o usuario do sistema, somente o admin fazer ação")]
    [HttpPut("Desativar-usuario")]
    public async Task<IActionResult> DesativarUsuario(int idUsuario)
    {
        await usuarioService.DesativarUsuario(idUsuario);
        return Ok(new { Mensagem = "Usuário desativado com sucesso." });
    }

    [Authorize(Roles = "Admin")]
    [EndpointSummary("Ativar Usuario no sistema")]
    [EndpointDescription("Ativa o usuario novamente ao sistema, somente o admin fazer ação")]
    [HttpPut("Ativar-usuario")]
    public async Task<IActionResult> AtivarUsuario(int idUsuario)
    {
        await usuarioService.AtivarUsuario(idUsuario);
        return Ok(new { Mensagem = "Usuário ativado com sucesso." });
    }
    #endregion

    #region ── Acesso Usuario ─────────────────────────────────────────────────────────────
    [Authorize(Roles = "Usuario,Admin")]
    [EndpointSummary("Depositar saldo a carteira do usuario")]
    [EndpointDescription("Deposita valor a sua carteira, precisa estar logado no sistema")]
    [HttpPost("Depositar-saldo")]
    public async Task<IActionResult> Depositar(int id, [FromBody] decimal valor)
    {
        await usuarioService.Depositar(id, valor);
        return Ok(new { Mensagem = "Depósito realizado com sucesso." });
    }

    [HttpGet("Detalhe-usuario")]
    [EndpointSummary("Visualizar detalhes da conta do usuario")]
    [EndpointDescription("Consegue visualizar detalhes de do perfil do usuario")]
    public async Task<IActionResult> GetDetalhes(int id)
    {
        var detalhes = await usuarioService.ObterDetalhesUsuario(id);
        return detalhes is null? NotFound(new { Mensagem = "Usuário não encontrado." }): Ok(detalhes);
    }
    #endregion
}
