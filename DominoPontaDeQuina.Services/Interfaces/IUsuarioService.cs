using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Services.Interfaces;

public interface IUsuarioService
{
    /// <summary>Cadastra um usuário com seus perfis de jogador, garantindo e-mail único.</summary>
    Task<Usuario> CadastrarAsync(
        string nome,
        string email,
        string hashSenha,
        IEnumerable<string> nomesJogadores,
        CancellationToken cancelamento = default);

    /// <summary>Retorna o usuário do e-mail informado ou o cadastra caso ainda não exista.</summary>
    Task<(Usuario Usuario, bool Criado)> ObterOuCadastrarAsync(
        string nome,
        string email,
        string hashSenha,
        IEnumerable<string> nomesJogadores,
        CancellationToken cancelamento = default);

    Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancelamento = default);

    Task<List<Usuario>> ListarComJogadoresAsync(CancellationToken cancelamento = default);
}
