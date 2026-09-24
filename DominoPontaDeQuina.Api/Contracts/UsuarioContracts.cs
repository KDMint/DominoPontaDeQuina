using System.ComponentModel.DataAnnotations;
using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Api.Contracts;

public record CadastrarUsuarioRequest(
    [Required, MaxLength(150)] string Nome,
    [Required, EmailAddress, MaxLength(254)] string Email,
    [Required, MinLength(6)] string Senha,
    [Required, MinLength(1)] List<string> Jogadores);

public record UsuarioResponse(Guid Id, string Nome, string Email, DateTime CriadoEm, List<JogadorResumoResponse> Jogadores)
{
    public static UsuarioResponse De(Usuario usuario) => new(
        usuario.Id,
        usuario.Nome,
        usuario.Email,
        usuario.CriadoEm,
        usuario.Jogadores.Select(JogadorResumoResponse.De).ToList());
}
