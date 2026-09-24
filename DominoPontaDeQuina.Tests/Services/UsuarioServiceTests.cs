using DominoPontaDeQuina.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DominoPontaDeQuina.Tests.Services;

[Trait("Categoria", "Services")]
public class UsuarioServiceTests : IDisposable
{
    private readonly BancoEmMemoriaFixture banco = new();

    public void Dispose() => banco.Dispose();

    private IUsuarioService CriarService(out IServiceScope escopo)
    {
        escopo = banco.CriarEscopo();
        return escopo.ServiceProvider.GetRequiredService<IUsuarioService>();
    }

    [Fact]
    public async Task CadastrarAsync_DevePersistirUsuarioComJogadoresEEmailNormalizado()
    {
        var cadastro = CriarService(out var escopoCadastro);
        using (escopoCadastro)
            await cadastro.CadastrarAsync(" Ana ", " Ana@Exemplo.COM ", "hash", ["Aninha", "  "]);

        // Novo escopo = novo DbContext: garante que a leitura vem do banco.
        var consulta = CriarService(out var escopoConsulta);
        using (escopoConsulta)
        {
            var usuarios = await consulta.ListarComJogadoresAsync();

            var usuario = Assert.Single(usuarios);
            Assert.Equal("Ana", usuario.Nome);
            Assert.Equal("ana@exemplo.com", usuario.Email);
            Assert.Equal("Aninha", Assert.Single(usuario.Jogadores).NomeExibicao);
        }
    }

    [Fact]
    public async Task CadastrarAsync_ComEmailDuplicado_DeveLancarExcecao()
    {
        var service = CriarService(out var escopo);
        using (escopo)
        {
            await service.CadastrarAsync("Ana", "ana@exemplo.com", "hash", ["Aninha"]);

            await Assert.ThrowsAsync<InvalidOperationException>(
                () => service.CadastrarAsync("Outra", "ANA@exemplo.com", "hash", ["Outra"]));
        }
    }

    [Fact]
    public async Task CadastrarAsync_SemJogadores_DeveLancarExcecao()
    {
        var service = CriarService(out var escopo);
        using (escopo)
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.CadastrarAsync("Ana", "ana@exemplo.com", "hash", []));
        }
    }

    [Fact]
    public async Task ObterOuCadastrarAsync_ChamadoDuasVezes_DeveCriarApenasUmUsuario()
    {
        var service = CriarService(out var escopo);
        using (escopo)
        {
            var (primeiro, criouPrimeiro) = await service.ObterOuCadastrarAsync("Ana", "ana@exemplo.com", "hash", ["Aninha"]);
            var (segundo, criouSegundo) = await service.ObterOuCadastrarAsync("Ana", "ana@exemplo.com", "hash", ["Aninha"]);

            Assert.True(criouPrimeiro);
            Assert.False(criouSegundo);
            Assert.Equal(primeiro.Id, segundo.Id);
            Assert.Single(await service.ListarComJogadoresAsync());
        }
    }
}
