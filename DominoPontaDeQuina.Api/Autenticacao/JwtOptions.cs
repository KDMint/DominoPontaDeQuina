using System.ComponentModel.DataAnnotations;

namespace DominoPontaDeQuina.Api.Autenticacao;

/// <summary>Configurações do JWT, lidas da seção "Jwt" do appsettings.</summary>
public class JwtOptions
{
    public const string Secao = "Jwt";

    [Required]
    public string Emissor { get; init; } = string.Empty;

    [Required]
    public string Audiencia { get; init; } = string.Empty;

    /// <summary>Chave simétrica HMAC-SHA256; precisa ter ao menos 32 caracteres (256 bits).</summary>
    [Required, MinLength(32)]
    public string Chave { get; init; } = string.Empty;

    [Range(1, 1440)]
    public int ExpiracaoMinutos { get; init; } = 60;
}
