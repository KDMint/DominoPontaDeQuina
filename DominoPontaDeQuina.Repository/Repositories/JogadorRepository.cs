using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Context;
using DominoPontaDeQuina.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Repository.Repositories;

public class JogadorRepository(DominoDbContext contexto) : IJogadorRepository
{
    /// <summary>Adiciona um novo jogador ao banco.</summary>
    public async Task<Jogador> AdicionarAsync(Jogador jogador, CancellationToken cancelamento = default)
    {
        ArgumentNullException.ThrowIfNull(jogador);
        await contexto.Jogadores.AddAsync(jogador, cancelamento);
        await contexto.SaveChangesAsync(cancelamento);
        return jogador;
    }

    /// <summary>Obtém um jogador pelo Id, incluindo o usuário e participações.</summary>
    public Task<Jogador?> ObterPorIdAsync(Guid id, CancellationToken cancelamento = default) =>
        contexto.Jogadores
                .Include(j => j.Usuario)
                .Include(j => j.Participacoes)
                .SingleOrDefaultAsync(j => j.Id == id, cancelamento);

    /// <summary>Lista todos os jogadores de um usuário.</summary>
    public Task<List<Jogador>> ListarPorUsuarioAsync(Guid usuarioId, CancellationToken cancelamento = default) =>
        contexto.Jogadores
                .Where(j => j.UsuarioId == usuarioId)
                .OrderBy(j => j.NomeExibicao)
                .ToListAsync(cancelamento);

    /// <summary>Lista os jogadores que venceram ao menos uma partida.</summary>
    public Task<List<Jogador>> ListarComVitoriaAsync(CancellationToken cancelamento = default) =>
        contexto.Jogadores
                .Where(j => j.Participacoes.Any(p => p.Vencedor))
                .Include(j => j.Participacoes)
                .OrderBy(j => j.NomeExibicao)
                .ToListAsync(cancelamento);

    /// <summary>Remove um jogador pelo Id.</summary>
    public async Task RemoverAsync(Guid id, CancellationToken cancelamento = default)
    {
        var jogador = await contexto.Jogadores.FindAsync([id], cancelamento);
        if (jogador is not null)
        {
            contexto.Jogadores.Remove(jogador);
            await contexto.SaveChangesAsync(cancelamento);
        }
    }
}
