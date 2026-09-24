using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DominoPontaDeQuina.Tests.Api;

[Trait("Categoria", "Api")]
public class AutenticacaoApiTests : IDisposable
{
    private readonly ApiFactory fabrica = new();
    private readonly HttpClient cliente;

    public AutenticacaoApiTests() => cliente = fabrica.CreateClient();

    public void Dispose()
    {
        cliente.Dispose();
        fabrica.Dispose();
    }

    private async Task CadastrarAsync(string email = "ana@exemplo.com", string senha = "segredo1")
    {
        var resposta = await cliente.PostAsJsonAsync("/api/usuarios",
            new { nome = "Ana", email, senha, jogadores = new[] { "Aninha" } });
        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
    }

    private async Task<string> LoginAsync(string email = "ana@exemplo.com", string senha = "segredo1")
    {
        var resposta = await cliente.PostAsJsonAsync("/api/auth/login", new { email, senha });
        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);

        var corpo = await resposta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("Bearer", corpo.GetProperty("tipo").GetString());
        return corpo.GetProperty("token").GetString()!;
    }

    private HttpRequestMessage Requisicao(HttpMethod metodo, string rota, string? token)
    {
        var requisicao = new HttpRequestMessage(metodo, rota);
        if (token is not null)
            requisicao.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return requisicao;
    }

    [Theory]
    [InlineData("/api/usuarios")]
    [InlineData("/api/auth/me")]
    [InlineData("/api/jogadores/vencedores")]
    [InlineData("/api/partidas?status=Aguardando")]
    public async Task RotaProtegida_SemToken_DeveRetornar401(string rota)
    {
        var resposta = await cliente.GetAsync(rota);

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveLiberarRotasProtegidas()
    {
        await CadastrarAsync();
        var token = await LoginAsync();

        var me = await cliente.SendAsync(Requisicao(HttpMethod.Get, "/api/auth/me", token));
        Assert.Equal(HttpStatusCode.OK, me.StatusCode);
        var usuario = await me.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("ana@exemplo.com", usuario.GetProperty("email").GetString());
        Assert.Equal("Ana", usuario.GetProperty("nome").GetString());

        var usuarios = await cliente.SendAsync(Requisicao(HttpMethod.Get, "/api/usuarios", token));
        Assert.Equal(HttpStatusCode.OK, usuarios.StatusCode);
    }

    [Theory]
    [InlineData("ana@exemplo.com", "senhaErrada")]
    [InlineData("ninguem@exemplo.com", "segredo1")]
    public async Task Login_ComCredenciaisInvalidas_DeveRetornar401(string email, string senha)
    {
        await CadastrarAsync();

        var resposta = await cliente.PostAsJsonAsync("/api/auth/login", new { email, senha });

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task RotaProtegida_ComTokenAdulterado_DeveRetornar401()
    {
        await CadastrarAsync();
        var token = await LoginAsync();

        // Troca o último caractere da assinatura: o payload continua legível, mas a assinatura deixa de bater.
        var adulterado = token[..^1] + (token[^1] == 'A' ? 'B' : 'A');
        var resposta = await cliente.SendAsync(Requisicao(HttpMethod.Get, "/api/auth/me", adulterado));

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task Cadastro_DevePermanecerPublico()
    {
        await CadastrarAsync("publico@exemplo.com");
    }
}
