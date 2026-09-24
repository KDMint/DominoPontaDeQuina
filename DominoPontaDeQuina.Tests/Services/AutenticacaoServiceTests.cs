using DominoPontaDeQuina.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DominoPontaDeQuina.Tests.Services;

[Trait("Categoria", "Services")]
public class AutenticacaoServiceTests : IDisposable
{
    private readonly BancoEmMemoriaFixture banco = new();

    public void Dispose() => banco.Dispose();

    private async Task CadastrarAnaAsync(string senha = "segredo1")
    {
        using var escopo = banco.CriarEscopo();
        var hash = escopo.ServiceProvider.GetRequiredService<IGeradorHashSenha>().GerarHash(senha);
        await escopo.ServiceProvider.GetRequiredService<IUsuarioService>()
            .CadastrarAsync("Ana", "ana@exemplo.com", hash, ["Aninha"]);
    }

    [Fact]
    public async Task AutenticarAsync_ComCredenciaisCorretas_DeveRetornarUsuario()
    {
        await CadastrarAnaAsync();

        using var escopo = banco.CriarEscopo();
        var autenticacao = escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();

        var usuario = await autenticacao.AutenticarAsync(" ANA@exemplo.com ", "segredo1");

        Assert.NotNull(usuario);
        Assert.Equal("ana@exemplo.com", usuario.Email);
    }

    [Theory]
    [InlineData("ana@exemplo.com", "senhaErrada")]
    [InlineData("ninguem@exemplo.com", "segredo1")]
    [InlineData("", "segredo1")]
    [InlineData("ana@exemplo.com", "")]
    public async Task AutenticarAsync_ComCredenciaisInvalidas_DeveRetornarNulo(string email, string senha)
    {
        await CadastrarAnaAsync();

        using var escopo = banco.CriarEscopo();
        var autenticacao = escopo.ServiceProvider.GetRequiredService<IAutenticacaoService>();

        Assert.Null(await autenticacao.AutenticarAsync(email, senha));
    }

    [Fact]
    public async Task AutenticarAsync_ComHashLegadoForaDoFormato_DeveRetornarNuloSemLancarExcecao()
    {
        using (var escopo = banco.CriarEscopo())
        {
            await escopo.ServiceProvider.GetRequiredService<IUsuarioService>()
                .CadastrarAsync("Legado", "legado@exemplo.com", "hash_senha_criptografada", ["Velho"]);
        }

        using var consulta = banco.CriarEscopo();
        var autenticacao = consulta.ServiceProvider.GetRequiredService<IAutenticacaoService>();

        Assert.Null(await autenticacao.AutenticarAsync("legado@exemplo.com", "hash_senha_criptografada"));
    }
}
