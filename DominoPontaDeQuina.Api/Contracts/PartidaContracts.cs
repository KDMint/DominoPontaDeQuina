using System.ComponentModel.DataAnnotations;
using DominoPontaDeQuina.Domain.Entities;

namespace DominoPontaDeQuina.Api.Contracts;

public record CriarPartidaRequest([Required] List<Guid> JogadorIds);

public record PontuacaoJogadorRequest(Guid JogadorId, int Pontuacao);

public record FinalizarPartidaRequest([Required] List<PontuacaoJogadorRequest> Pontuacoes);

public record ParticipacaoResponse(Guid JogadorId, string? NomeExibicao, int Posicao, int Pontuacao, bool Vencedor)
{
    public static ParticipacaoResponse De(ParticipacaoPartida participacao) => new(
        participacao.JogadorId,
        participacao.Jogador?.NomeExibicao,
        participacao.Posicao,
        participacao.Pontuacao,
        participacao.Vencedor);
}

public record PartidaResponse(
    Guid Id,
    DateTime IniciadoEm,
    DateTime? FinalizadoEm,
    StatusPartida Status,
    List<ParticipacaoResponse> Participacoes)
{
    public static PartidaResponse De(Partida partida) => new(
        partida.Id,
        partida.IniciadoEm,
        partida.FinalizadoEm,
        partida.Status,
        partida.Participacoes.OrderBy(pp => pp.Posicao).Select(ParticipacaoResponse.De).ToList());
}
