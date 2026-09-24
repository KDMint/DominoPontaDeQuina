using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Services.Interfaces;

public interface IAutenticacaoService
{
    /// <summary>Valida as credenciais e retorna o usuário, ou null se e-mail ou senha estiverem incorretos.</summary>
    Task<Usuario?> AutenticarAsync(string email, string senha, CancellationToken cancelamento = default);
}
