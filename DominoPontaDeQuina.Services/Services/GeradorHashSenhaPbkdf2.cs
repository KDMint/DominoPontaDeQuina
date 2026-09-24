using System.Security.Cryptography;
using DominoPontaDeQuina.Services.Interfaces;

namespace DominoPontaDeQuina.Services.Services;

/// <summary>Gera hashes PBKDF2 (SHA-256) no formato "iteracoes.salt.hash", em Base64.</summary>
public class GeradorHashSenhaPbkdf2 : IGeradorHashSenha
{
    private const int TamanhoSalt = 16;
    private const int TamanhoHash = 32;
    private const int Iteracoes = 100_000;
    private static readonly HashAlgorithmName Algoritmo = HashAlgorithmName.SHA256;

    public string GerarHash(string senha)
    {
        ArgumentException.ThrowIfNullOrEmpty(senha);

        var salt = RandomNumberGenerator.GetBytes(TamanhoSalt);
        var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iteracoes, Algoritmo, TamanhoHash);

        return $"{Iteracoes}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public bool Verificar(string senha, string hash)
    {
        ArgumentNullException.ThrowIfNull(senha);
        ArgumentNullException.ThrowIfNull(hash);

        var partes = hash.Split('.');
        if (partes.Length != 3 || !int.TryParse(partes[0], out var iteracoes) || iteracoes <= 0)
            return false;

        byte[] salt, esperado;
        try
        {
            salt = Convert.FromBase64String(partes[1]);
            esperado = Convert.FromBase64String(partes[2]);
        }
        catch (FormatException)
        {
            return false;
        }

        if (esperado.Length == 0)
            return false;

        var calculado = Rfc2898DeriveBytes.Pbkdf2(senha, salt, iteracoes, Algoritmo, esperado.Length);

        return CryptographicOperations.FixedTimeEquals(calculado, esperado);
    }
}
