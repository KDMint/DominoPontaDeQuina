using DominoPontaDeQuina.Migrations;
using DominoPontaDeQuina.Repository.Context;
using DominoPontaDeQuina.Repository.Interfaces;
using DominoPontaDeQuina.Repository.Repositories;
using DominoPontaDeQuina.Services.Interfaces;
using DominoPontaDeQuina.Services.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var servicos = new ServiceCollection();

// DbContext
servicos.AddDbContext<DominoDbContext>(opcoes => opcoes.UseSqlite("Data Source=domino.db"));

// Repositories
servicos.AddScoped<IUsuarioRepository, UsuarioRepository>();
servicos.AddScoped<IJogadorRepository, JogadorRepository>();
servicos.AddScoped<IPartidaRepository, PartidaRepository>();
servicos.AddScoped<IParticipacaoPartidaRepository, ParticipacaoPartidaRepository>();

// Services
servicos.AddScoped<IUsuarioService, UsuarioService>();
servicos.AddScoped<IJogadorService, JogadorService>();
servicos.AddScoped<IPartidaService, PartidaService>();

// Fluxo principal
servicos.AddScoped<AplicacaoConsole>();

await using var provedor = servicos.BuildServiceProvider(new ServiceProviderOptions
{
    ValidateScopes = true,
    ValidateOnBuild = true
});

await using var escopo = provedor.CreateAsyncScope();

// Aplica as migrations pendentes caso o banco ainda não exista
await escopo.ServiceProvider.GetRequiredService<DominoDbContext>().Database.MigrateAsync();

await escopo.ServiceProvider.GetRequiredService<AplicacaoConsole>().ExecutarAsync();
