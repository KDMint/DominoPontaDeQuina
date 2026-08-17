using DominoPontaDeQuina.Domain.Entities;
using DominoPontaDeQuina.Repository.Context;
using DominoPontaDeQuina.Repository.Repositories;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("=== Domino Ponta de Quina - EF Core ===");

using var contexto = new DominoDbContext();

// Aplica as migrations pendentes caso o banco ainda não exista
await contexto.Database.MigrateAsync();

var usuarioRepo = new UsuarioRepository(contexto);
var email = "jogador@dominopontadequina.com";

var usuarioExistente = await usuarioRepo.ObterPorEmailAsync(email);

if (usuarioExistente == null)
{
    var novoUsuario = new Usuario
    {
        Nome = "Jogador Exemplo",
        Email = email,
        HashSenha = "hash_senha_criptografada",
        Jogadores = new List<Jogador>
        {
            new Jogador { NomeExibicao = "MestreDoDomino" }
        }
    };

    await usuarioRepo.AdicionarAsync(novoUsuario);
    Console.WriteLine($"[+] Novo usuário '{novoUsuario.Nome}' inserido com sucesso.");
}
else
{
    Console.WriteLine($"[i] Usuário '{usuarioExistente.Nome}' já cadastrado no banco.");
}

var usuarios = await contexto.Usuarios
    .Include(u => u.Jogadores)
    .ToListAsync();

Console.WriteLine($"\n--- Usuários Cadastrados ({usuarios.Count}) ---");
foreach (var u in usuarios)
{
    var jogadores = u.Jogadores.Count > 0 
        ? string.Join(", ", u.Jogadores.Select(j => j.NomeExibicao)) 
        : "Nenhum";
    Console.WriteLine($"• Nome: {u.Nome} | E-mail: {u.Email} | Jogadores: [{jogadores}]");
}

Console.WriteLine("\nExecução concluída com sucesso!");

