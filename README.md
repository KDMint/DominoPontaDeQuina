# Domino Ponta de Quina

## Projetos

- `DominoPontaDeQuina.Core`: regras e fluxo do jogo.
- `DominoPontaDeQuina.Domain`: entidades e enums persistentes.
- `DominoPontaDeQuina.Repository`: `DominoDbContext`, interfaces e implementações dos repositórios e migrações EF Core.
- `DominoPontaDeQuina.Services`: camada de serviços que orquestra as regras de uso sobre os repositórios.
- `DominoPontaDeQuina.Migrations`: aplicacao console usada como startup project para migrations e execucao de demonstracao.
- `DominoPontaDeQuina.Tests`: testes automatizados do nucleo do jogo.


## Modelo persistente

`Usuario` representa a conta do aplicativo cliente e pode possuir varios `Jogador`, que sao perfis de jogo.
`Partida` representa uma partida armazenada para consulta de historico. `ParticipacaoPartida` liga um jogador a uma partida e registra sua posicao, pontuacao e resultado.

Esta etapa prepara a persistencia e o futuro fluxo de autenticacao. API, endpoints, autenticacao e JWT estao fora do escopo.

## Pre-requisitos

- .NET 8 SDK ou .NET 10 SDK
- Ferramenta `dotnet-ef` 9.x (`dotnet tool install --global dotnet-ef --version 9.*`)

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
- **Mapeamento com Fluent API**:
  - `DominoDbContext` configura tabelas, chaves, limites, índice único de e-mail, conversão do status para texto e relacionamentos 1:N com exclusão em cascata.
  - `ParticipacaoPartida` possui índice único composto por `PartidaId` e `JogadorId`.
- **Contexto de Dados (`DominoDbContext`)**:
  - Declaração dos `DbSet`s necessários: `Usuarios`, `Jogadores`, `Partidas` e `ParticipacoesPartida`.
  - Configuração da conexão com SQLite (`Data Source=domino.db`) no método `OnConfiguring` com suporte a `DbContextOptions`.
- **Design-Time Factory (`DominoDbContextFactory`)**:
  - Implementação de `IDesignTimeDbContextFactory<DominoDbContext>` no projeto `DominoPontaDeQuina.Migrations` para viabilizar comandos da CLI do EF Core.
- **Repositórios**:
  - `UsuarioRepository`, `JogadorRepository`, `PartidaRepository` e `ParticipacaoPartidaRepository` implementam operações assíncronas de persistência e consultas LINQ.
- **Migrações e Banco de Dados**:
  - A migration `RenomearJogoParaPartida` renomeia as tabelas legadas, converte o status para texto e preserva os dados existentes no SQLite (`domino.db`).


## Injeção de Dependência e Camada de Services

- **Interfaces dos repositórios** (`DominoPontaDeQuina.Repository/Interfaces`): `IUsuarioRepository`, `IJogadorRepository`, `IPartidaRepository` e `IParticipacaoPartidaRepository`. As consultas LINQ continuam nas implementações dos repositórios.
- **Services** (`DominoPontaDeQuina.Services`):
  - `UsuarioService`: cadastro com e-mail normalizado e único, exige ao menos um jogador; `ObterOuCadastrarAsync` torna o fluxo idempotente.
  - `JogadorService`: consultas de jogadores por usuário e de vencedores.
  - `PartidaService`: cria partidas (2 a 4 jogadores distintos e existentes), controla as transições `Aguardando → EmAndamento → Finalizado/Cancelado`, registra pontuações e marca o(s) vencedor(es) pela maior pontuação.
- **Composição** (`DominoPontaDeQuina.Migrations/Program.cs`): `DbContext`, repositórios, services e `AplicacaoConsole` são registrados em um `ServiceCollection` com escopo `Scoped`. O fluxo principal (`AplicacaoConsole`) recebe os services pelo construtor; nenhuma classe de aplicação é instanciada com `new`.
- **Testes** (`DominoPontaDeQuina.Tests/Services`): montam o mesmo container de DI sobre SQLite em memória e validam o fluxo completo de usuários e partidas.

```bash
dotnet test DominoPontaDeQuina.Tests --filter "Categoria=Services"
```
