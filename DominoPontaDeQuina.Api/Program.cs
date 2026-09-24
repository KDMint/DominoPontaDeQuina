using System.Text.Json.Serialization;
using DominoPontaDeQuina.Api.Infrastructure;
using DominoPontaDeQuina.Repository.Context;
using DominoPontaDeQuina.Repository.Interfaces;
using DominoPontaDeQuina.Repository.Repositories;
using DominoPontaDeQuina.Services.Interfaces;
using DominoPontaDeQuina.Services.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<DominoDbContext>(opcoes =>
    opcoes.UseSqlite(builder.Configuration.GetConnectionString("Domino")));

// Repositories
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IJogadorRepository, JogadorRepository>();
builder.Services.AddScoped<IPartidaRepository, PartidaRepository>();
builder.Services.AddScoped<IParticipacaoPartidaRepository, ParticipacaoPartidaRepository>();

// Services
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IJogadorService, JogadorService>();
builder.Services.AddScoped<IPartidaService, PartidaService>();
builder.Services.AddSingleton<IGeradorHashSenha, GeradorHashSenhaPbkdf2>();

// Web API
builder.Services
    .AddControllers()
    .AddJsonOptions(opcoes => opcoes.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ManipuladorExcecoes>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Aplica as migrations pendentes ao iniciar
using (var escopo = app.Services.CreateScope())
{
    await escopo.ServiceProvider.GetRequiredService<DominoDbContext>().Database.MigrateAsync();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
