using Fiap.Cloud.Games.Application.DTOs.Jogo;
using Fiap.Cloud.Games.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fiap.Cloud.Games.API.Controllers;

public class PromocaoController(IPromocaoService promocaoService) : ControllerBase
{
    #region ── Acesso Admin ─────────────────────────────────────────────────────────────
    [Authorize(Roles = "Admin")]
    [EndpointSummary("Criar promoção de jogos no sistema")]
    [EndpointDescription("Cria promoção de jogos no sistema, somente Admin pode realizar cadastro")]
    [HttpPost("Criar-Promocao")]
    public async Task<IActionResult> Criar([FromBody] CriarPromocaoDto dto)
    {
        var id = await promocaoService.CriarPromocao(dto);
        return Ok(new { Mensagem = $"Promoção cadastrada com sucesso. id:{id}" });
    }

    [Authorize(Roles = "Admin")]
    [EndpointSummary("Ativa promoção de jogos no sistema")]
    [EndpointDescription("Ativa promoção de jogos no sistema, somente Admin pode realizar ativação")]
    [HttpPost("Ativar-Promocao")]
    public async Task<IActionResult> Ativar(int idPromocao)
    {
        await promocaoService.AtivarPromocao(idPromocao);
        return Ok(new { Mensagem = $"Promoção ativada com sucesso." });
    }

    [Authorize(Roles = "Admin")]
    [EndpointSummary("Adiciona jogos a promoção")]
    [EndpointDescription("Adiciona jogos a promoção somente se não for ativada ainda, somente Admin pode realizar adição")]
    [HttpPost("Adicinar-Jogo-Promocao")]
    public async Task<IActionResult> AdicionarJogo(int idPromocao, int jogoId)
    {
        await promocaoService.AdicionarJogoPromocao(idPromocao, jogoId);
        return Ok(new { Mensagem = $"Jogo Adicionado a promoção com sucesso." });
    }

    [Authorize(Roles = "Admin")]
    [EndpointSummary("Exclui a promoção de jogos do sistema")]
    [EndpointDescription("Exclui a promoção de jogos do sistema estando ativa ou não, somente Admin pode realizar exclusão")]
    [HttpPost("Excluir-Promocao")]
    public async Task<IActionResult> Excluir(int idPromocao)
    {
        await promocaoService.ExcluirPromocao(idPromocao);
        return Ok(new { Mensagem = $"Promoção excluida com sucesso." });
    }
    #endregion

    #region ── Acesso Publico ─────────────────────────────────────────────────────────────
    [AllowAnonymous]
    [EndpointSummary("Lista promoção de jogos no sistema")]
    [EndpointDescription("Lista promoção de jogos no sistema, somente Admin pode visualizar as que estão ativas e inativas")]
    [HttpGet("Listar-promocao")]
    public async Task<IActionResult> ListarPromocao([FromQuery] bool somenteAtivas = true)
    {
        bool isAdmin = User.IsInRole("Admin");
        // Para não-admin força o filtro de ativas
        var lista = await promocaoService.ListarPromocao(somenteAtivas: !isAdmin || somenteAtivas, isAdmin: isAdmin);
        return Ok(lista);
    }
    #endregion

    #region ── Acesso Usuario ─────────────────────────────────────────────────────────────
    [Authorize(Roles = "Usuario,Admin")]
    [EndpointSummary("Adquirir promoção com os jogos")]
    [EndpointDescription("Adquiri a promoção dos jogos, precisa esta logado no sistema para adquirir")]
    [HttpPost("Adquirir-promocao")]
    public async Task<IActionResult> AdquirirPromocao(int idPromocao)
    {
        if (!User.Identity?.IsAuthenticated ?? true)
            return Unauthorized(new { Mensagem = "Usuário não autenticado." });

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int idUsuario))
            return Unauthorized(new { Mensagem = "Usuário inválido." });

        await promocaoService.AdquirirPromocao(idUsuario,idPromocao);
        return Ok(new { Mensagem = "Promoção adquirida com sucesso." });
    }
    #endregion
}