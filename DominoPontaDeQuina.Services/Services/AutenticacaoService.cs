using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Services.Interfaces;

namespace DominoPontaDeQuina.Services.Services;

public class AutenticacaoService(
    IUsuarioService usuarioService,
    IGeradorHashSenha geradorHashSenha) : IAutenticacaoService
{
    public async Task<Usuario?> AutenticarAsync(string email, string senha, CancellationToken cancelamento = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(senha))
            return null;

        var usuario = await usuarioService.ObterPorEmailAsync(email, cancelamento);

        // Mesmo sem usuário, calcula um hash para que o tempo de resposta não revele quais e-mails existem.
        if (usuario is null)
        {
            geradorHashSenha.GerarHash(senha);
            return null;
        }

        return geradorHashSenha.Verificar(senha, usuario.HashSenha) ? usuario : null;
    }
}
