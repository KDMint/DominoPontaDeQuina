using System.ComponentModel.DataAnnotations;

namespace DominoPontaDeQuina.Api.Contracts;

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Senha);

public record LoginResponse(string Token, string Tipo, DateTime ExpiraEm, UsuarioResponse Usuario);

public record UsuarioAutenticadoResponse(Guid Id, string Nome, string Email);
