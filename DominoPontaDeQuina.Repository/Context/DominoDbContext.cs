using DominoPontaDeQuina.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DominoPontaDeQuina.Repository.Context;

public class DominoDbContext : DbContext
{
    public DominoDbContext()
    {
    }

    public DominoDbContext(DbContextOptions<DominoDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Jogador> Jogadores => Set<Jogador>();
    public DbSet<Partida> Partidas => Set<Partida>();
    public DbSet<ParticipacaoPartida> ParticipacoesPartida => Set<ParticipacaoPartida>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=domino.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Usuario ────────────────────────────────────────────────
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Nome)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(254);

            entity.HasIndex(u => u.Email)
                  .IsUnique();

            entity.Property(u => u.HashSenha)
                  .IsRequired()
                  .HasMaxLength(500);

            entity.Property(u => u.CriadoEm)
                  .IsRequired();

            // 1 Usuario → N Jogadores
            entity.HasMany(u => u.Jogadores)
                  .WithOne(j => j.Usuario)
                  .HasForeignKey(j => j.UsuarioId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Jogador ────────────────────────────────────────────────
        modelBuilder.Entity<Jogador>(entity =>
        {
            entity.ToTable("Jogadores");
            entity.HasKey(j => j.Id);

            entity.Property(j => j.NomeExibicao)
                  .IsRequired()
                  .HasMaxLength(100);

            // 1 Jogador → N ParticipacaoPartida
            entity.HasMany(j => j.Participacoes)
                  .WithOne(p => p.Jogador)
                  .HasForeignKey(p => p.JogadorId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ── Partida ────────────────────────────────────────────────
        modelBuilder.Entity<Partida>(entity =>
        {
            entity.ToTable("Partidas");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.IniciadoEm)
                  .IsRequired();

            entity.Property(p => p.Status)
                  .IsRequired()
                  .HasConversion<string>();

            // 1 Partida → N ParticipacaoPartida
            entity.HasMany(p => p.Participacoes)
                  .WithOne(pp => pp.Partida)
                  .HasForeignKey(pp => pp.PartidaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ParticipacaoPartida ────────────────────────────────────
        modelBuilder.Entity<ParticipacaoPartida>(entity =>
        {
            entity.ToTable("ParticipacoesPartida");
            entity.HasKey(pp => pp.Id);

            entity.Property(pp => pp.Posicao)
                  .IsRequired();

            entity.Property(pp => pp.Pontuacao)
                  .IsRequired();

            entity.Property(pp => pp.Vencedor)
                  .IsRequired();

            // Índice composto: um jogador não pode participar duas vezes da mesma partida
            entity.HasIndex(pp => new { pp.PartidaId, pp.JogadorId })
                  .IsUnique();
        });
    }
}
