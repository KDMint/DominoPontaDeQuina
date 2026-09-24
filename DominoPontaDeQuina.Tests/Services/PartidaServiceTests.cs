using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DominoPontaDeQuina.Tests.Services;

[Trait("Categoria", "Services")]
public class PartidaServiceTests : IDisposable
{
    private readonly BancoEmMemoriaFixture banco = new();

    public void Dispose() => banco.Dispose();

    private async Task<(Guid JogadorA, Guid JogadorB)> CriarDoisJogadoresAsync()
    {
        using var escopo = banco.CriarEscopo();
        var usuarios = escopo.ServiceProvider.GetRequiredService<IUsuarioService>();

        var ana = await usuarios.CadastrarAsync("Ana", "ana@exemplo.com", "hash", ["Aninha"]);
        var bia = await usuarios.CadastrarAsync("Bia", "bia@exemplo.com", "hash", ["Bibi"]);

        return (ana.Jogadores.Single().Id, bia.Jogadores.Single().Id);
    }

    [Fact]
    public async Task FluxoCompleto_CriarIniciarFinalizar_DevePersistirResultadoEHistorico()
    {
        var (jogadorA, jogadorB) = await CriarDoisJogadoresAsync();
        Guid partidaId;

        using (var escopo = banco.CriarEscopo())
        {
            var partidas = escopo.ServiceProvider.GetRequiredService<IPartidaService>();

            var partida = await partidas.CriarAsync([jogadorA, jogadorB]);
            partidaId = partida.Id;
            Assert.Equal(StatusPartida.Aguardando, partida.Status);

            await partidas.IniciarAsync(partidaId);
            await partidas.FinalizarAsync(partidaId, new Dictionary<Guid, int>
            {
                [jogadorA] = 120,
                [jogadorB] = 45
            });
        }

        // Novo escopo = novo DbContext: garante que os dados vêm do banco, não do change tracker.
        using (var escopo = banco.CriarEscopo())
        {
            var partidas = escopo.ServiceProvider.GetRequiredService<IPartidaService>();
            var jogadores = escopo.ServiceProvider.GetRequiredService<IJogadorService>();

            var partida = await partidas.ObterPorIdAsync(partidaId);
            Assert.NotNull(partida);
            Assert.Equal(StatusPartida.Finalizado, partida.Status);
            Assert.NotNull(partida.FinalizadoEm);

            var vencedora = Assert.Single(partida.Participacoes, pp => pp.Vencedor);
            Assert.Equal(jogadorA, vencedora.JogadorId);
            Assert.Equal(120, vencedora.Pontuacao);
            Assert.Equal(1, vencedora.Posicao);

            Assert.Equal(jogadorA, Assert.Single(await jogadores.ListarVencedoresAsync()).Id);
            Assert.Single(await partidas.ListarPorStatusAsync(StatusPartida.Finalizado));
            Assert.Empty(await partidas.ListarPorStatusAsync(StatusPartida.Aguardando));

            var historico = await partidas.ListarHistoricoDoJogadorAsync(jogadorB);
            var participacao = Assert.Single(historico);
            Assert.False(participacao.Vencedor);
            Assert.Equal(45, participacao.Pontuacao);
        }
    }

    [Fact]
    public async Task FinalizarAsync_ComEmpate_DeveMarcarTodosOsLideresComoVencedores()
    {
        var (jogadorA, jogadorB) = await CriarDoisJogadoresAsync();

        using var escopo = banco.CriarEscopo();
        var partidas = escopo.ServiceProvider.GetRequiredService<IPartidaService>();

        var partida = await partidas.CriarAsync([jogadorA, jogadorB]);
        await partidas.IniciarAsync(partida.Id);
        var finalizada = await partidas.FinalizarAsync(partida.Id, new Dictionary<Guid, int>
        {
            [jogadorA] = 50,
            [jogadorB] = 50
        });

        Assert.All(finalizada.Participacoes, pp => Assert.True(pp.Vencedor));
    }

    [Fact]
    public async Task CriarAsync_ComJogadorInexistente_DeveLancarExcecao()
    {
        var (jogadorA, _) = await CriarDoisJogadoresAsync();

        using var escopo = banco.CriarEscopo();
        var partidas = escopo.ServiceProvider.GetRequiredService<IPartidaService>();

        await Assert.ThrowsAsync<KeyNotFoundException>(() => partidas.CriarAsync([jogadorA, Guid.NewGuid()]));
    }

    [Fact]
    public async Task CriarAsync_ComJogadorRepetidoOuQuantidadeInvalida_DeveLancarExcecao()
    {
        var (jogadorA, _) = await CriarDoisJogadoresAsync();

        using var escopo = banco.CriarEscopo();
        var partidas = escopo.ServiceProvider.GetRequiredService<IPartidaService>();

        await Assert.ThrowsAsync<ArgumentException>(() => partidas.CriarAsync([jogadorA, jogadorA]));
        await Assert.ThrowsAsync<ArgumentException>(() => partidas.CriarAsync([jogadorA]));
    }

    [Fact]
    public async Task FinalizarAsync_PartidaNaoIniciada_DeveLancarExcecao()
    {
        var (jogadorA, jogadorB) = await CriarDoisJogadoresAsync();

        using var escopo = banco.CriarEscopo();
        var partidas = escopo.ServiceProvider.GetRequiredService<IPartidaService>();

        var partida = await partidas.CriarAsync([jogadorA, jogadorB]);

        await Assert.ThrowsAsync<InvalidOperationException>(() => partidas.FinalizarAsync(
            partida.Id, new Dictionary<Guid, int> { [jogadorA] = 10, [jogadorB] = 5 }));
    }

    [Fact]
    public async Task FinalizarAsync_SemPontuacaoDeTodosOsJogadores_DeveLancarExcecao()
    {
        var (jogadorA, jogadorB) = await CriarDoisJogadoresAsync();

        using var escopo = banco.CriarEscopo();
        var partidas = escopo.ServiceProvider.GetRequiredService<IPartidaService>();

        var partida = await partidas.CriarAsync([jogadorA, jogadorB]);
        await partidas.IniciarAsync(partida.Id);

        await Assert.ThrowsAsync<ArgumentException>(() => partidas.FinalizarAsync(
            partida.Id, new Dictionary<Guid, int> { [jogadorA] = 10 }));
    }

    [Fact]
    public async Task CancelarAsync_PartidaFinalizada_DeveLancarExcecao()
    {
        var (jogadorA, jogadorB) = await CriarDoisJogadoresAsync();

        using var escopo = banco.CriarEscopo();
        var partidas = escopo.ServiceProvider.GetRequiredService<IPartidaService>();

        var partida = await partidas.CriarAsync([jogadorA, jogadorB]);
        await partidas.IniciarAsync(partida.Id);
        await partidas.FinalizarAsync(partida.Id, new Dictionary<Guid, int> { [jogadorA] = 1, [jogadorB] = 0 });

        await Assert.ThrowsAsync<InvalidOperationException>(() => partidas.CancelarAsync(partida.Id));
    }
}
