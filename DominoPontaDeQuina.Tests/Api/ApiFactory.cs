using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DominoPontaDeQuina.Tests.Api;

/// <summary>Sobe a API real em memória, com um banco SQLite temporário e exclusivo por instância.</summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string caminhoBanco = Path.Combine(Path.GetTempPath(), $"domino-teste-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:Domino", $"Data Source={caminhoBanco};Pooling=False");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        foreach (var arquivo in new[] { caminhoBanco, $"{caminhoBanco}-wal", $"{caminhoBanco}-shm" })
            File.Delete(arquivo);
    }
}
