using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Api.Autenticacao;

public record TokenGerado(string Token, DateTime ExpiraEm);

public interface IGeradorTokenJwt
{
    TokenGerado Gerar(Usuario usuario);
}
