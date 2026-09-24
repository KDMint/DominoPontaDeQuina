using System.Security.Claims;
using System.Text;
using DominoPontaDeQuina.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DominoPontaDeQuina.Api.Autenticacao;

public class GeradorTokenJwt(IOptions<JwtOptions> opcoes, TimeProvider relogio) : IGeradorTokenJwt
{
    private readonly JwtOptions jwt = opcoes.Value;

    public TokenGerado Gerar(Usuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        var agora = relogio.GetUtcNow().UtcDateTime;
        var expiraEm = agora.AddMinutes(jwt.ExpiracaoMinutos);

        var descritor = new SecurityTokenDescriptor
        {
            Issuer = jwt.Emissor,
            Audience = jwt.Audiencia,
            IssuedAt = agora,
            NotBefore = agora,
            Expires = expiraEm,
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(JwtRegisteredClaimNames.Name, usuario.Nome),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            ]),
            SigningCredentials = new SigningCredentials(
                CriarChave(jwt.Chave), SecurityAlgorithms.HmacSha256)
        };

        return new TokenGerado(new JsonWebTokenHandler().CreateToken(descritor), expiraEm);
    }

    public static SymmetricSecurityKey CriarChave(string chave) => new(Encoding.UTF8.GetBytes(chave));
}
