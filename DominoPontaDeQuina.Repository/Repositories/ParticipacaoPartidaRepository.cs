using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Context;
using DominoPontaDeQuina.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Repository.Repositories;

public class ParticipacaoPartidaRepository(DominoDbContext contexto) : IParticipacaoPartidaRepository
{
    public async Task<ParticipacaoPartida> AdicionarAsync(
        ParticipacaoPartida participacao,
        CancellationToken cancelamento = default)
    {
        ArgumentNullException.ThrowIfNull(participacao);
        await contexto.ParticipacoesPartida.AddAsync(participacao, cancelamento);
        await contexto.SaveChangesAsync(cancelamento);
        return participacao;
    }

    public Task<ParticipacaoPartida?> ObterPorIdAsync(
        Guid id,
        CancellationToken cancelamento = default) =>
        contexto.ParticipacoesPartida
                .Include(pp => pp.Partida)
                .Include(pp => pp.Jogador)
                .SingleOrDefaultAsync(pp => pp.Id == id, cancelamento);

    public Task<List<ParticipacaoPartida>> ListarPorPartidaAsync(
        Guid partidaId,
        CancellationToken cancelamento = default) =>
        contexto.ParticipacoesPartida
                .Where(pp => pp.PartidaId == partidaId)
                .Include(pp => pp.Jogador)
                .OrderBy(pp => pp.Posicao)
                .ToListAsync(cancelamento);

    public Task<List<ParticipacaoPartida>> ListarPorJogadorAsync(
        Guid jogadorId,
        CancellationToken cancelamento = default) =>
        contexto.ParticipacoesPartida
                .Where(pp => pp.JogadorId == jogadorId)
                .Include(pp => pp.Partida)
                .OrderByDescending(pp => pp.Partida.IniciadoEm)
                .ToListAsync(cancelamento);

    public async Task<bool> AtualizarResultadoAsync(
        Guid id,
        int pontuacao,
        bool vencedor,
        CancellationToken cancelamento = default)
    {
        var participacao = await contexto.ParticipacoesPartida
            .SingleOrDefaultAsync(pp => pp.Id == id, cancelamento);

        if (participacao is null) return false;

        participacao.Pontuacao = pontuacao;
        participacao.Vencedor = vencedor;
        await contexto.SaveChangesAsync(cancelamento);
        return true;
    }

    public async Task RemoverAsync(Guid id, CancellationToken cancelamento = default)
    {
        var participacao = await contexto.ParticipacoesPartida.FindAsync([id], cancelamento);
        if (participacao is not null)
        {
            contexto.ParticipacoesPartida.Remove(participacao);
            await contexto.SaveChangesAsync(cancelamento);
        }
    }
}