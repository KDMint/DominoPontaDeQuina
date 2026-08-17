# Domino Ponta de Quina

## Projetos

- `DominoPontaDeQuina.Core`: regras e fluxo do jogo.
- `DominoPontaDeQuina.Domain`: entidades e enums persistentes.
- `DominoPontaDeQuina.Repository`: `DominoDbContext`, repositórios e migrações EF Core.
- `DominoPontaDeQuina.Migrations`: aplicacao console usada como startup project para migrations e execucao de demonstracao.
- `DominoPontaDeQuina.Tests`: testes automatizados do nucleo do jogo.


## Modelo persistente

`Usuario` representa a conta do aplicativo cliente e pode possuir varios `Jogador`, que sao perfis de jogo.
`Jogo` representa uma partida armazenada para consulta de historico. `ParticipacaoJogo` liga um jogador a um jogo e registra sua posicao, pontuacao e resultado.

Esta etapa prepara a persistencia e o futuro fluxo de autenticacao. API, endpoints, autenticacao e JWT estao fora do escopo.

## Pre-requisitos

- .NET 8 SDK ou .NET 10 SDK
- Ferramenta `dotnet-ef` 8.x (`dotnet tool install --global dotnet-ef --version 8.*`)

## Restaurar e compilar

```bash
dotnet restore
dotnet build
```

## Executar a aplicacao

Para executar o projeto de inicialização/demonstração:

```bash
dotnet run --project DominoPontaDeQuina.Migrations
```


## Migrations

Os comandos devem usar `DominoPontaDeQuina.Migrations` como startup project e `DominoPontaDeQuina.Repository` como projeto do contexto:

```bash
dotnet ef migrations add Inicial \
  --project DominoPontaDeQuina.Repository \
  --startup-project DominoPontaDeQuina.Migrations

dotnet ef database update \
  --project DominoPontaDeQuina.Repository \
  --startup-project DominoPontaDeQuina.Migrations
```

O banco SQLite local `domino.db` e ignorado pelo Git.

## Implementações Realizadas (Laboratório EF Core)

- **Mapeamento com Data Annotations**:
  - `Usuario`: Configurado com `[Table("Usuarios")]`, `[Key]`, `[Required]`, `[MaxLength]` e `[EmailAddress]` para validação e tamanho de colunas (`Nome`, `Email`, `HashSenha`, `CriadoEm`).
  - `Jogador`: Configurado com `[Table("Jogadores")]`, `[Key]`, `[Required]`, `[MaxLength(100)]` e relacionamento com `Usuario` via `[ForeignKey(nameof(Usuario))]`.
- **Mapeamento por Convenções**:
  - `Jogo` e `ParticipacaoJogo`: Deixados sem anotações adicionais para resolução automática pelas convenções do EF Core (chaves primárias `Id`, tipos primitivos/enums e relacionamentos 1:N).
- **Contexto de Dados (`DominoDbContext`)**:
  - Declaração dos `DbSet`s necessários: `Usuarios`, `Jogadores`, `Jogos` e `ParticipacoesJogo`.
  - Configuração da conexão com SQLite (`Data Source=domino.db`) no método `OnConfiguring` com suporte a `DbContextOptions`.
- **Design-Time Factory (`DominoDbContextFactory`)**:
  - Implementação de `IDesignTimeDbContextFactory<DominoDbContext>` no projeto `DominoPontaDeQuina.Migrations` para viabilizar comandos da CLI do EF Core.
- **Repositório (`UsuarioRepository`)**:
  - Implementação das operações assíncronas `AdicionarAsync` e `ObterPorEmailAsync`.
- **Migrações e Banco de Dados**:
  - Criação da migration `Inicial` e execução do update no banco SQLite (`domino.db`).

