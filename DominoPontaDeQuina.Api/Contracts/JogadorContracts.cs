using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Api.Contracts;

public record JogadorResumoResponse(Guid Id, string NomeExibicao)
{
    public static JogadorResumoResponse De(Jogador jogador) => new(jogador.Id, jogador.NomeExibicao);
}

public record JogadorResponse(Guid Id, string NomeExibicao, Guid UsuarioId, int Partidas, int Vitorias)
{
    public static JogadorResponse De(Jogador jogador) => new(
        jogador.Id,
        jogador.NomeExibicao,
        jogador.UsuarioId,
        jogador.Participacoes.Count,
        jogador.Participacoes.Count(p => p.Vencedor));
}

public record HistoricoPartidaResponse(
    Guid PartidaId,
    DateTime IniciadoEm,
    DateTime? FinalizadoEm,
    StatusPartida Status,
    int Posicao,
    int Pontuacao,
    bool Vencedor)
{
    public static HistoricoPartidaResponse De(ParticipacaoPartida participacao) => new(
        participacao.PartidaId,
        participacao.Partida.IniciadoEm,
        participacao.Partida.FinalizadoEm,
        participacao.Partida.Status,
        participacao.Posicao,
        participacao.Pontuacao,
        participacao.Vencedor);
}
