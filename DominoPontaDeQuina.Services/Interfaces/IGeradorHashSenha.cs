namespace DominoPontaDeQuina.Services.Interfaces;

public interface IGeradorHashSenha
{
    string GerarHash(string senha);
    bool Verificar(string senha, string hash);
}
