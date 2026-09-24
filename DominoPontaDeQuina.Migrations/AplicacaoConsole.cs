using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Services.Interfaces;

namespace DominoPontaDeQuina.Migrations;

/// <summary>Fluxo principal de demonstração; recebe os services por injeção de dependência.</summary>
public class AplicacaoConsole(
    IUsuarioService usuarioService,
    IJogadorService jogadorService,
    IPartidaService partidaService)
{
    public async Task ExecutarAsync(CancellationToken cancelamento = default)
    {
        Console.WriteLine("=== Domino Ponta de Quina - EF Core ===");

        var jogadorA = await GarantirUsuarioAsync(
            "Jogador Exemplo", "jogador@dominopontadequina.com", "MestreDoDomino", cancelamento);
        var jogadorB = await GarantirUsuarioAsync(
            "Jogadora Exemplo", "jogadora@dominopontadequina.com", "RainhaDaQuina", cancelamento);

        await ListarUsuariosAsync(cancelamento);

        Console.WriteLine("\n--- Nova Partida ---");
        var partida = await partidaService.CriarAsync([jogadorA.Id, jogadorB.Id], cancelamento);
        Console.WriteLine($"[+] Partida {partida.Id} criada ({partida.Status}).");

        await partidaService.IniciarAsync(partida.Id, cancelamento);
        Console.WriteLine("[>] Partida iniciada.");

        var pontuacoes = new Dictionary<Guid, int>
        {
            [jogadorA.Id] = Random.Shared.Next(0, 200),
            [jogadorB.Id] = Random.Shared.Next(0, 200)
        };
        partida = await partidaService.FinalizarAsync(partida.Id, pontuacoes, cancelamento);
        Console.WriteLine($"[✓] Partida finalizada em {partida.FinalizadoEm:dd/MM/yyyy HH:mm:ss} (UTC).");

        foreach (var participacao in partida.Participacoes.OrderBy(pp => pp.Posicao))
        {
            var marcador = participacao.Vencedor ? " (vencedor)" : string.Empty;
            Console.WriteLine($"  {participacao.Posicao}. {participacao.Jogador.NomeExibicao}: {participacao.Pontuacao} pts{marcador}");
        }

        await ListarVencedoresAsync(cancelamento);
        await ListarHistoricoAsync(jogadorA, cancelamento);

        Console.WriteLine("\nExecução concluída com sucesso!");
    }

    private async Task<Jogador> GarantirUsuarioAsync(
        string nome,
        string email,
        string nomeJogador,
        CancellationToken cancelamento)
    {
        var (usuario, criado) = await usuarioService.ObterOuCadastrarAsync(
            nome, email, "hash_senha_criptografada", [nomeJogador], cancelamento);

        Console.WriteLine(criado
            ? $"[+] Novo usuário '{usuario.Nome}' inserido com sucesso."
            : $"[i] Usuário '{usuario.Nome}' já cadastrado no banco.");

        var jogadores = await jogadorService.ListarPorUsuarioAsync(usuario.Id, cancelamento);
        return jogadores.First();
    }

    private async Task ListarUsuariosAsync(CancellationToken cancelamento)
    {
        var usuarios = await usuarioService.ListarComJogadoresAsync(cancelamento);

        Console.WriteLine($"\n--- Usuários Cadastrados ({usuarios.Count}) ---");
        foreach (var u in usuarios)
        {
            var jogadores = u.Jogadores.Count > 0
                ? string.Join(", ", u.Jogadores.Select(j => j.NomeExibicao))
                : "Nenhum";
            Console.WriteLine($"• Nome: {u.Nome} | E-mail: {u.Email} | Jogadores: [{jogadores}]");
        }
    }

    private async Task ListarVencedoresAsync(CancellationToken cancelamento)
    {
        var vencedores = await jogadorService.ListarVencedoresAsync(cancelamento);

        Console.WriteLine($"\n--- Jogadores com Vitória ({vencedores.Count}) ---");
        foreach (var jogador in vencedores)
            Console.WriteLine($"• {jogador.NomeExibicao}: {jogador.Participacoes.Count(p => p.Vencedor)} vitória(s)");
    }

    private async Task ListarHistoricoAsync(Jogador jogador, CancellationToken cancelamento)
    {
        var historico = await partidaService.ListarHistoricoDoJogadorAsync(jogador.Id, cancelamento);

        Console.WriteLine($"\n--- Histórico de {jogador.NomeExibicao} ({historico.Count} partida(s)) ---");
        foreach (var participacao in historico.Take(5))
        {
            var resultado = participacao.Vencedor ? "Vitória" : "Derrota";
            Console.WriteLine(
                $"• {participacao.Partida.IniciadoEm:dd/MM/yyyy HH:mm} | {participacao.Partida.Status} | {participacao.Pontuacao} pts | {resultado}");
        }
    }
}
