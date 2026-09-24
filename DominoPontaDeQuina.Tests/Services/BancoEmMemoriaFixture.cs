using DominoPontaDeQuina.Repository.Context;
using DominoPontaDeQuina.Repository.Interfaces;
using DominoPontaDeQuina.Repository.Repositories;
using DominoPontaDeQuina.Services.Interfaces;
using DominoPontaDeQuina.Services.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DominoPontaDeQuina.Tests.Services;

/// <summary>
/// Monta o mesmo grafo de dependências do Program.cs sobre um SQLite em memória,
/// para exercitar services e consultas LINQ dos repositories contra um banco real.
/// </summary>
public sealed class BancoEmMemoriaFixture : IDisposable
{
    private readonly SqliteConnection conexao;
    private readonly ServiceProvider provedor;

    public BancoEmMemoriaFixture()
    {
        conexao = new SqliteConnection("Data Source=:memory:");
        conexao.Open();

        var servicos = new ServiceCollection();
        servicos.AddDbContext<DominoDbContext>(opcoes => opcoes.UseSqlite(conexao));

        servicos.AddScoped<IUsuarioRepository, UsuarioRepository>();
        servicos.AddScoped<IJogadorRepository, JogadorRepository>();
        servicos.AddScoped<IPartidaRepository, PartidaRepository>();
        servicos.AddScoped<IParticipacaoPartidaRepository, ParticipacaoPartidaRepository>();

        servicos.AddScoped<IUsuarioService, UsuarioService>();
        servicos.AddScoped<IJogadorService, JogadorService>();
        servicos.AddScoped<IPartidaService, PartidaService>();

        provedor = servicos.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });

        using var escopo = provedor.CreateScope();
        escopo.ServiceProvider.GetRequiredService<DominoDbContext>().Database.EnsureCreated();
    }

    /// <summary>Cria um escopo novo, equivalente a uma nova unidade de trabalho (novo DbContext).</summary>
    public IServiceScope CriarEscopo() => provedor.CreateScope();

    public void Dispose()
    {
        provedor.Dispose();
        conexao.Dispose();
    }
}
