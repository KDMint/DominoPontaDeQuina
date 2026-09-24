using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Interfaces;
using DominoPontaDeQuina.Services.Interfaces;

namespace DominoPontaDeQuina.Services.Services;

public class PartidaService(
    IPartidaRepository partidaRepository,
    IJogadorRepository jogadorRepository,
    IParticipacaoPartidaRepository participacaoRepository) : IPartidaService
{
    public const int MinimoJogadores = 2;
    public const int MaximoJogadores = 4;

    public async Task<Partida> CriarAsync(IReadOnlyList<Guid> jogadorIds, CancellationToken cancelamento = default)
    {
        ArgumentNullException.ThrowIfNull(jogadorIds);

        if (jogadorIds.Count is < MinimoJogadores or > MaximoJogadores)
            throw new ArgumentException(
                $"Uma partida deve ter entre {MinimoJogadores} e {MaximoJogadores} jogadores.", nameof(jogadorIds));

        if (jogadorIds.Distinct().Count() != jogadorIds.Count)
            throw new ArgumentException("Um jogador não pode participar duas vezes da mesma partida.", nameof(jogadorIds));

        foreach (var jogadorId in jogadorIds)
        {
            if (await jogadorRepository.ObterPorIdAsync(jogadorId, cancelamento) is null)
                throw new KeyNotFoundException($"Jogador '{jogadorId}' não encontrado.");
        }

        var partida = new Partida
        {
            Status = StatusPartida.Aguardando,
            Participacoes = jogadorIds
                .Select((jogadorId, indice) => new ParticipacaoPartida
                {
                    JogadorId = jogadorId,
                    Posicao = indice + 1
                })
                .ToList()
        };

        return await partidaRepository.AdicionarAsync(partida, cancelamento);
    }

    public async Task IniciarAsync(Guid partidaId, CancellationToken cancelamento = default)
    {
        var partida = await ObterObrigatoriaAsync(partidaId, cancelamento);

        if (partida.Status != StatusPartida.Aguardando)
            throw new InvalidOperationException(
                $"Somente partidas aguardando podem ser iniciadas (status atual: {partida.Status}).");

        await partidaRepository.AtualizarStatusAsync(partidaId, StatusPartida.EmAndamento, cancelamento);
    }

    public async Task<Partida> FinalizarAsync(
        Guid partidaId,
        IReadOnlyDictionary<Guid, int> pontuacaoPorJogador,
        CancellationToken cancelamento = default)
    {
        ArgumentNullException.ThrowIfNull(pontuacaoPorJogador);

        var partida = await ObterObrigatoriaAsync(partidaId, cancelamento);

        if (partida.Status != StatusPartida.EmAndamento)
            throw new InvalidOperationException(
                $"Somente partidas em andamento podem ser finalizadas (status atual: {partida.Status}).");

        var participacoes = await participacaoRepository.ListarPorPartidaAsync(partidaId, cancelamento);

        var jogadoresDaPartida = participacoes.Select(pp => pp.JogadorId).ToHashSet();
        if (!jogadoresDaPartida.SetEquals(pontuacaoPorJogador.Keys))
            throw new ArgumentException(
                "Informe a pontuação de todos os jogadores da partida, e somente deles.", nameof(pontuacaoPorJogador));

        if (pontuacaoPorJogador.Values.Any(p => p < 0))
            throw new ArgumentException("A pontuação não pode ser negativa.", nameof(pontuacaoPorJogador));

        var maiorPontuacao = pontuacaoPorJogador.Values.Max();

        foreach (var participacao in participacoes)
        {
            var pontuacao = pontuacaoPorJogador[participacao.JogadorId];
            await participacaoRepository.AtualizarResultadoAsync(
                participacao.Id, pontuacao, pontuacao == maiorPontuacao, cancelamento);
        }

        await partidaRepository.AtualizarStatusAsync(partidaId, StatusPartida.Finalizado, cancelamento);

        return (await partidaRepository.ObterPorIdAsync(partidaId, cancelamento))!;
    }

    public async Task CancelarAsync(Guid partidaId, CancellationToken cancelamento = default)
    {
        var partida = await ObterObrigatoriaAsync(partidaId, cancelamento);

        if (partida.Status is StatusPartida.Finalizado or StatusPartida.Cancelado)
            throw new InvalidOperationException(
                $"A partida não pode ser cancelada (status atual: {partida.Status}).");

        await partidaRepository.AtualizarStatusAsync(partidaId, StatusPartida.Cancelado, cancelamento);
    }

    public Task<Partida?> ObterPorIdAsync(Guid partidaId, CancellationToken cancelamento = default) =>
        partidaRepository.ObterPorIdAsync(partidaId, cancelamento);

    public Task<List<Partida>> ListarPorStatusAsync(StatusPartida status, CancellationToken cancelamento = default) =>
        partidaRepository.ListarPorStatusAsync(status, cancelamento);

    public Task<List<ParticipacaoPartida>> ListarHistoricoDoJogadorAsync(
        Guid jogadorId,
        CancellationToken cancelamento = default) =>
        participacaoRepository.ListarPorJogadorAsync(jogadorId, cancelamento);

    private async Task<Partida> ObterObrigatoriaAsync(Guid partidaId, CancellationToken cancelamento) =>
        await partidaRepository.ObterPorIdAsync(partidaId, cancelamento)
        ?? throw new KeyNotFoundException($"Partida '{partidaId}' não encontrada.");
}
