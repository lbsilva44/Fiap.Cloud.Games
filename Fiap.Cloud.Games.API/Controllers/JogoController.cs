using Fiap.Cloud.Games.Application.DTOs.Jogo;
using Fiap.Cloud.Games.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fiap.Cloud.Games.API.Controllers;

public class JogoController(IJogoService jogoService) : ControllerBase
{
    #region ── Acesso Admin ─────────────────────────────────────────────────────────────
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Cadatrar jogo no sistema")]
    [EndpointDescription("Cadastrar no sistema um jogo novo, somente o Admin pode cadastrar")]
    [HttpPost("Cadastrar-jogo")]
    public async Task<IActionResult> Cadastrar([FromBody] JogoDto dto)
    {
        await jogoService.CadastrarJogo(dto);
        return Ok(new { Mensagem = "Jogo cadastrado com sucesso." });
    }

    [Authorize(Roles = "Admin")]
    [EndpointSummary("Publicar jogo no sistema")]
    [EndpointDescription("Publica o jogo no sistema para poder efetuar aquisição, somente o Admin pode publicar")]
    [HttpPut("Publicar-jogo")]
    public async Task<IActionResult> Publicar(int idJogo)
    {
        await jogoService.PublicarJogo(idJogo);
        return Ok(new { Mensagem = "Jogo publicado com sucesso." });
    }
    #endregion

    #region ── Acesso Publico ─────────────────────────────────────────────────────────────
    [AllowAnonymous]
    [EndpointSummary("Listar jogos cadastrados no sistema")]
    [EndpointDescription("Lista os jogos que estão publicados no sistema, somente o Admin pode ver os que estão desativados tambem")]
    [HttpGet("Listar-jogos")]
    public async Task<IActionResult> Listar()
    {
        var TipoAcesso = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "Anonimo";
        var jogos = await jogoService.ListarJogos(TipoAcesso);
        return Ok(jogos);
    }
#endregion

    #region ── Acesso Usuario ─────────────────────────────────────────────────────────────
    [Authorize(Roles = "Usuario,Admin")]
    [EndpointSummary("Adquirir o jogo para sua biblioteca")]
    [EndpointDescription("Realiza a compra do jogo somente se tiver saldo na carteira")]
    [HttpPost("Adquirir-jogo")]
    public async Task<IActionResult> Adquirir(int idJogo)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Unauthorized(new { Mensagem = "Usuário não autenticado." });

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int idUsuario))
            return Unauthorized(new { Mensagem = "Usuário inválido." });

        await jogoService.AdquirirJogo(idJogo, idUsuario);
        return Ok(new { Mensagem = "Jogo adquirido com sucesso." });
    }

    [Authorize(Roles = "Usuario,Admin")]
    [EndpointSummary("Biblioteca de jogos do usuario")]
    [EndpointDescription("Consulta a biblioteca de jogos do usuario logado")]
    [HttpGet("Biblioteca-jogos-usuario")]
    public async Task<IActionResult> BibliotecaUsuario()
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Unauthorized(new { Mensagem = "Usuário não autenticado." });

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int idUsuario))
            return Unauthorized(new { Mensagem = "Usuário inválido." });

        var jogos = await jogoService.ListarJogosAdquiridosUsuario(idUsuario);
        return Ok(jogos);
    }
    #endregion
}
