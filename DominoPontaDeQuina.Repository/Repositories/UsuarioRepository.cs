using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Context;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Repository.Repositories;

public class UsuarioRepository(DominoDbContext contexto)
{
    public async Task<Usuario> AdicionarAsync(Usuario usuario, CancellationToken cancelamento = default)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        await contexto.Usuarios.AddAsync(usuario, cancelamento);
        await contexto.SaveChangesAsync(cancelamento);
        return usuario;
    }

    public Task<Usuario?> ObterPorEmailAsync(string email, CancellationToken cancelamento = default) =>
        contexto.Usuarios.SingleOrDefaultAsync(usuario => usuario.Email == email, cancelamento);
}

