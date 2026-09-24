using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Interfaces;
using DominoPontaDeQuina.Services.Interfaces;

namespace DominoPontaDeQuina.Services.Services;

public class UsuarioService(IUsuarioRepository usuarioRepository) : IUsuarioService
{
    public async Task<Usuario> CadastrarAsync(
        string nome,
        string email,
        string hashSenha,
        IEnumerable<string> nomesJogadores,
        CancellationToken cancelamento = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(hashSenha);
        ArgumentNullException.ThrowIfNull(nomesJogadores);

        var emailNormalizado = NormalizarEmail(email);
        if (await usuarioRepository.ObterPorEmailAsync(emailNormalizado, cancelamento) is not null)
            throw new InvalidOperationException($"Já existe um usuário cadastrado com o e-mail '{emailNormalizado}'.");

        var jogadores = nomesJogadores
            .Select(n => n?.Trim())
            .Where(n => !string.IsNullOrEmpty(n))
            .Select(n => new Jogador { NomeExibicao = n! })
            .ToList();

        if (jogadores.Count == 0)
            throw new ArgumentException("O usuário deve possuir ao menos um jogador.", nameof(nomesJogadores));

        var usuario = new Usuario
        {
            Nome = nome.Trim(),
            Email = emailNormalizado,
            HashSenha = hashSenha,
            Jogadores = jogadores
        };

        return await usuarioRepository.AdicionarAsync(usuario, cancelamento);
    }

    public async Task<(Usuario Usuario, bool Criado)> ObterOuCadastrarAsync(
        string nome,
        string email,
        string hashSenha,
        IEnumerable<string> nomesJogadores,
        CancellationToken cancelamento = default)
    {
        var existente = await ObterPorEmailAsync(email, cancelamento);
        if (existente is not null)
            return (existente, false);

        var novo = await CadastrarAsync(nome, email, hashSenha, nomesJogadores, cancelamento);
        return (novo, true);
    }

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancelamento = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        return usuarioRepository.ObterPorEmailAsync(NormalizarEmail(email), cancelamento);
    }

    public Task<List<Usuario>> ListarComJogadoresAsync(CancellationToken cancelamento = default) =>
        usuarioRepository.ListarComJogadoresAsync(cancelamento);

    private static string NormalizarEmail(string email) => email.Trim().ToLowerInvariant();
}
