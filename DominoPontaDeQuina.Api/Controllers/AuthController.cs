using DominoPontaDeQuina.Api.Autenticacao;
using DominoPontaDeQuina.Api.Contracts;
using DominoPontaDeQuina.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

namespace DominoPontaDeQuina.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController(
    IAutenticacaoService autenticacaoService,
    IGeradorTokenJwt geradorTokenJwt) : ControllerBase
{
    /// <summary>Autentica com e-mail e senha e retorna um token JWT (Bearer).</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType<LoginResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest requisicao, CancellationToken cancelamento)
    {
        var usuario = await autenticacaoService.AutenticarAsync(requisicao.Email, requisicao.Senha, cancelamento);
        if (usuario is null)
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "E-mail ou senha inválidos.");

        var token = geradorTokenJwt.Gerar(usuario);
        return new LoginResponse(token.Token, "Bearer", token.ExpiraEm, UsuarioResponse.De(usuario));
    }

    /// <summary>Retorna os dados do usuário dono do token.</summary>
    [HttpGet("me")]
    [ProducesResponseType<UsuarioAutenticadoResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<UsuarioAutenticadoResponse> Me() => new UsuarioAutenticadoResponse(
        Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)!.Value),
        User.FindFirst(JwtRegisteredClaimNames.Name)!.Value,
        User.FindFirst(JwtRegisteredClaimNames.Email)!.Value);
}
