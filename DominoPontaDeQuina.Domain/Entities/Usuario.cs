using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DominoPontaDeQuina.Domain.Entities;

[Table("Usuarios")]
public class Usuario
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [MaxLength(254)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string HashSenha { get; set; } = string.Empty;

    [Required]
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Jogador> Jogadores { get; set; } = new List<Jogador>();
}

