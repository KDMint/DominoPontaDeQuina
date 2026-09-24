using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Context;
using DominoPontaDeQuina.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Repository.Repositories;

public class UsuarioRepository(DominoDbContext contexto) : IUsuarioRepository
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

    public Task<List<Usuario>> ListarComJogadoresAsync(CancellationToken cancelamento = default) =>
        contexto.Usuarios
                .Include(usuario => usuario.Jogadores)
                .OrderBy(usuario => usuario.Nome)
                .ToListAsync(cancelamento);
}

