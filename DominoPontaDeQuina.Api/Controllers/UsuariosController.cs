using DominoPontaDeQuina.Api.Contracts;
using DominoPontaDeQuina.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DominoPontaDeQuina.Api.Controllers;

[ApiController]
[Route("api/usuarios")]
[Produces("application/json")]
public class UsuariosController(
    IUsuarioService usuarioService,
    IJogadorService jogadorService,
    IGeradorHashSenha geradorHashSenha) : ControllerBase
{
    /// <summary>Cadastra um usuário e seus perfis de jogador.</summary>
    [HttpPost]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UsuarioResponse>> Cadastrar(
        CadastrarUsuarioRequest requisicao,
        CancellationToken cancelamento)
    {
        var usuario = await usuarioService.CadastrarAsync(
            requisicao.Nome,
            requisicao.Email,
            geradorHashSenha.GerarHash(requisicao.Senha),
            requisicao.Jogadores,
            cancelamento);

        return CreatedAtAction(nameof(ObterPorEmail), new { email = usuario.Email }, UsuarioResponse.De(usuario));
    }

    /// <summary>Lista os usuários com seus jogadores.</summary>
    [HttpGet]
    public async Task<ActionResult<List<UsuarioResponse>>> Listar(CancellationToken cancelamento)
    {
        var usuarios = await usuarioService.ListarComJogadoresAsync(cancelamento);
        return usuarios.Select(UsuarioResponse.De).ToList();
    }

    /// <summary>Obtém um usuário pelo e-mail.</summary>
    [HttpGet("por-email")]
    [ProducesResponseType<UsuarioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioResponse>> ObterPorEmail(
        [FromQuery] string email,
        CancellationToken cancelamento)
    {
        var usuario = await usuarioService.ObterPorEmailAsync(email, cancelamento);
        return usuario is null ? NotFound() : UsuarioResponse.De(usuario);
    }

    /// <summary>Lista os jogadores de um usuário.</summary>
    [HttpGet("{usuarioId:guid}/jogadores")]
    public async Task<ActionResult<List<JogadorResumoResponse>>> ListarJogadores(
        Guid usuarioId,
        CancellationToken cancelamento)
    {
        var jogadores = await jogadorService.ListarPorUsuarioAsync(usuarioId, cancelamento);
        return jogadores.Select(JogadorResumoResponse.De).ToList();
    }
}
