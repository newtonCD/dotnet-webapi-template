<img align="center" width="100%" height="auto" style="cursor: zoom-in; max-width: 100%" src="https://raw.githubusercontent.com/newtonCD/dotnet-webapi-template/master/.github/clean_architecture.webp" />

# Dotnet Web API Template (.NET 6)

## Visão Geral <a name="visao-geral"></a>

Este projeto é um template de Web API criado para acelerar o desenvolvimento de APIs RESTful utilizando as melhores práticas e padrões recomendados. A API é 100% funcional e utiliza um banco de dados em memória, facilitando o teste e a implantação.

## Principais tecnologias e padrões utilizados: <a name="tecnologies"></a>

* [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
* [ASP.NET Core 6](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0)
* [CQRS](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs)
* [Entity Framework Core 6](https://docs.microsoft.com/en-us/ef/core/)
* [FluentValidation](https://fluentvalidation.net/)
* [Mapster](https://github.com/MapsterMapper/Mapster)
* [MediatR](https://github.com/jbogard/MediatR)
* [Polly](https://github.com/App-vNext/Polly)
* [AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit)
* [ProblemDetails](https://github.com/khellang/Middleware)
* [DistributedCache](https://github.com/dotnet/runtime)
* [Serilog](https://serilog.net/)
* [Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
* [Throw](https://github.com/amantinband/throw)
* [UnitOfWork](https://www.devmedia.com.br/unit-of-work-o-padrao-de-unidade-de-trabalho-net/25811)
* [xUnit](https://github.com/xunit/xunit), [FluentAssertions](https://fluentassertions.com/), [Moq](https://github.com/moq)

## Recursos: <a name="recursos"></a>

* <b>CRUD básico:</b> o template inclui operações básicas de criar, ler, atualizar e excluir (CRUD) para a entidade fictícia de clientes chamada <b>Customer</b>, que possui os campos Name e Email.
* <b>Paginação:</b> a API suporta consultas paginadas de todos os clientes cadastrados.
* <b>Script de geração automática:</b> a solução inclui um script .csx que permite gerar automaticamente novas entidades, seus arquivos derivados e toda a estrutura do CQRS para um CRUD.
* <b>MediatR Pipeline Behaviors:</b> diversos pipeline behaviors centralizando a lógica para Caching, Logging, Polly Retry e Validation.
* <b>Logging:</b> log estruturado com Serilog e plugado ao Logging Behavior. O Logging Behavior permite que você configure via appsettings se vocês quer ligar/desligar individualmente os logs de request e response.
* <b>Polly Retry:</b> bahavior do Polly para retentativas em caso de alguma exception de banco de dados. Também pode ser configurável via appsettings, além de poder ser estendido para outros tipos de cenários.
* <b>Caching:</b> behavior de cache tanto em memória como distribuído com o Redis. O tipo do cache é configurável via appsettings.
* <b>CQRS:</b> padrão CQRS+MediatR com separação de bancos de dados de leitura e escrita e eventos de domínio para sincronização das bases de dados.
* <b>Documentação interativa:</b> o Swagger é utilizado para fornecer uma documentação interativa da API, facilitando a exploração e teste dos endpoints disponíveis.

## Começando: <a name="start"></a>

Siga os passos abaixo para configurar e executar o projeto:

1. Certifique-se de que tenha instalado na sua máquina o runtime do .NET 6 e seu respectivo SDK.
2. Clone o repositório para o seu ambiente local.
3. Abra o projeto no Visual Studio ou em seu editor preferido.
4. Restaure os pacotes NuGet.
5. Execute o projeto.
6. Acesse a documentação Swagger no navegador em <b>http://localhost:PORTA/swagger</b>.

## Utilizando o Gerador de Classes <a name="generator"></a>

Para adicionar novas entidades e endpoints à API, siga os passos abaixo:

1. Instale a ferramenta global <b>dotnet-script</b> executando no terminal o seguinte comando:  
>     dotnet tool install -g dotnet-script

2. Vá para a pasta <b>scripts</b> onde encontra-se o arquivo <b>class-generator.csx</b>

3. Para gerar uma nova classe e todos os seus arquivos derivados, execute o comando abaixo:  
>     dotnet script class-generator.csx -- "NOME_DA_ENTIDADE"

4. Como regra e recomendação para o nome da entidade, utilize nomes em inglês, sempre no singular e entre aspas.

5. O próprio script cuidará de pluralizar o nome da entidade quando for necessário e também ajustará os nomes no formato Pascal Case.

6. Como resultado da execução desse script, serão criados vários arquivos e pastas em locais distintos da solução. Serão criadas as classes de domínio, interfaces, repositórios, CQRS, controller entre outras coisas.

7. Ao final da execução, e não ocorrendo nenhum erro, basta rodar o build da solução e você terá um CRUD básico 100% funcional rodando num banco de dados na memória.

8. Altere e adicione os campos e propriedades necessárias à nova entidade criada e personalize a solução do modo que atenda às necessidades do seu projeto.

9. Implemente as regras de negócio e validações conforme necessário.

10. Se necessário, atualize a documentação do Swagger.

## Configuração do banco de dados

O template está configurado para usar 2 bancos de dados na memória. Isso garante que todos os usuários possam executar a solução sem a necessidade de ter que configurar infraestrutura adicional (por exemplo, SQL Server). Você pode optar por desligar o banco de dados na memória editando a propriedade "UseInMemoryDatabase" que está no appsettings.

## Visão Geral da Arquitetura <a name="visao-geral"></a>

### Domain

Contém todas as entidades, agregados, enums, constantes, interfaces, tipos, valueObjects e lógica específicos da camada de domínio.

### Application

Camada que contém toda a lógica da aplicação. É dependente da camada de domínio, mas não possui dependências de nenhuma outra camada ou projeto. Essa camada define interfaces que são implementadas por camadas externas. Por exemplo, se a aplicação precisar acessar um serviço de notificação, uma nova interface seria adicionada em Application e uma implementação seria criada dentro da Infrastructure.

### Infrastructure

Camada que contém as classes que fazem acesso a recursos externos, como sistemas de arquivos, serviços/apis da web, banco de dados e assim por diante. Essas classes devem ser baseadas em interfaces definidas na camada Application.

### WebApi

Também chamada de UI, aqui será representada por uma Web Api. Essa camada depende das camadas Application e Infrastructure. No entanto, a dependência da Infrastructure é apenas para dar suporte à injeção de dependência. Portanto, somente o *<b>Program.cs</b>* e demais classes de extensão é quem devem fazer referência à Infrastructure.

## Contribuindo

Agradecemos sua contribuição para este projeto! Para contribuir, siga estas etapas:

1. Faça um fork do repositório.
2. Crie uma branch com suas alterações.
3. Faça um pull request para a branch develop.
4. Aguarde a revisão e aprovação das suas alterações.

## Suporte e Comunidade
Caso você tenha dúvidas, sugestões ou queira contribuir para o projeto, entre em contato com a squad <b>Plataforma Meios de Pagamento Online</b> da tribo de Payments.
