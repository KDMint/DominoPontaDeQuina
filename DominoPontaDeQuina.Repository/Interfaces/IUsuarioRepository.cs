using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Repository.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario> AdicionarAsync(Usuario usuario, CancellationToken cancelamento = default);
    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancelamento = default);
    Task<List<Usuario>> ListarComJogadoresAsync(CancellationToken cancelamento = default);
}
