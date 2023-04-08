# Dotnet Web API Template (.NET 6)

## Visão Geral

Esta API é um template [.NET Core 6](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-6.0) que serve como base e ponto de partida para a criação de outras APIs, seguindo padrões de boas práticas de arquitetura e desenvolvimento em .NET e C#. A API utiliza vários recursos e tecnologias, como CQRS, MediatR, DistributedCache, Polly, Entity Framework, Clean Architecture, AspNetCoreRateLimit, Serilog, FluentValidation, UnitOfWork, Mapster, entre outros.

O objetivo é facilitar o desenvolvimento de novas APIs e servir como uma referência para outros desenvolvedores.

## Arquitetura da Solução

A solução adota o padrão [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) (Arquitetura Limpa), que é composta por camadas que promovem a separação de responsabilidades, testabilidade e manutenibilidade do projeto. 

<img align="center" width="100%" height="auto" style="cursor: zoom-in; max-width: 100%" src="https://raw.githubusercontent.com/newtonCD/dotnet-webapi-template/master/.github/clean_architecture.webp" /><br/>

Abaixo os detalhes de cada camada da arquitetura e seus componentes:

### 1. Domain (Domínio)

A camada Domain contém as entidades e regras de negócio centrais da aplicação. É a camada mais importante, pois representa o coração do sistema e seu propósito. Os componentes que residem nesta camada incluem:

- **Entities**: são os objetos que representam os conceitos e dados do domínio.

- **Events**: são eventos que representam algo significativo que ocorreu no domínio e que podem ser usados para desacoplar componentes e manter a consistência.
- **Aggregates**: são agrupamentos de entidades e objetos de valor que garantem a consistência e a integridade do domínio.
- **Constants**: são valores imutáveis usados em todo o domínio.
- **Enums**: são tipos enumerados que representam um conjunto limitado de valores possíveis.
- **Value Objetcs**: são objetos imutáveis que representam características ou atributos do domínio, como um endereço ou uma data.

### 2. Application (Aplicação)

A camada de Aplicação contém a lógica da aplicação e coordena as interações entre as outras camadas. Ela é dependente da camada de Domínio, mas não possui dependências diretas com outras camadas ou projetos externos. Nesta camada, são definidas interfaces que serão implementadas por outras camadas. Os componentes que residem nesta camada incluem:

- **Abstractions/Contracts**: são interfaces que definem os serviços externos necessários para a camada de Aplicação. Essas interfaces são implementadas por outras camadas, como a camada de Infraestrutura, e permitem desacoplar a camada de Aplicação das implementações concretas.

- **Behaviours**: são classes que implementam a lógica comum e transversal à aplicação, como logging, caching e tratamento de erros entre outros. Eles são aplicados aos manipuladores de requisições e respostas utilizando o padrão "Pipeline Behavior" do MediatR.
- **Exceptions**: são classes personalizadas que representam erros específicos da aplicação. Elas são lançadas quando ocorre uma situação excepcional ou de erro durante a execução da lógica da aplicação.
- **Mappers**: são classes responsáveis por mapear objetos entre diferentes modelos e camadas da aplicação, como entidades, DTOs e ViewModels. Eles utilizam bibliotecas como Mapster e AutoMapper para simplificar e automatizar esse processo.
- **Models (DTOs)**: os DTOs (Data Transfer Objects) são objetos simples usados para transferir dados entre as camadas da aplicação e para o cliente. Eles são desacoplados das entidades do domínio e podem conter apenas os dados necessários para a operação específica.
- **Services/Handlers**: são classes que implementam a lógica da aplicação e orquestram as interações entre as entidades e os serviços externos. Eles são responsáveis por processar as requisições e gerar as respostas da aplicação.
- **Specifications**: são classes que encapsulam as consultas e filtros aplicados às entidades do domínio. Elas são usadas para simplificar e reutilizar a lógica de consulta e garantir a consistência entre as operações de leitura e escrita.
- **Validators**: são classes responsáveis por validar os objetos de domínio, garantindo a consistência e integridade dos dados. Eles utilizam bibliotecas como FluentValidation para implementar as regras de validação de forma expressiva e legível.

### 3. Infrastructure (Infraestrutura)

A camada de Infraestrutura é responsável por implementar as interfaces definidas na camada de Aplicação e fornecer acesso a serviços externos. Esta camada pode ser dividida em Infraestrutura e Persistência:

#### *Infrastructure*

A parte de Infraestrutura lida com os seguintes componentes:

- **Email Service**: o serviço de e-mail é responsável por enviar e-mails para os usuários, como confirmação de cadastro, recuperação de senha e comunicações de marketing.

- **File Storage**: o armazenamento de arquivos é responsável por armazenar e gerenciar arquivos, como imagens e documentos, em serviços de armazenamento externos, como o Azure Blob Storage ou o Amazon S3.
- **Identity Services**: os serviços de identidade gerenciam a autenticação e autorização dos usuários da aplicação, incluindo o registro, login, gerenciamento de roles e permissões.
- **Notifications**: o serviço de notificações é responsável por enviar notificações para os usuários, como alertas em tempo real, mensagens push e notificações na aplicação.
- **Payment Services**: os serviços de pagamento gerenciam as transações financeiras da aplicação, como processamento de pagamentos, emissão de faturas e integração com provedores de pagamento, como o PayPal.
- **Queue Storage**: o armazenamento de filas lida com a comunicação assíncrona entre diferentes partes da aplicação ou entre diferentes serviços, utilizando filas de mensagens como o Azure Service Bus ou o RabbitMQ.
- **Sms Service**: o serviço de SMS é responsável por enviar mensagens de texto para os telefones dos usuários, como códigos de verificação, alertas de segurança e promoções.
- **Third-party services**: os serviços de terceiros são integrações com APIs e serviços externos que fornecem funcionalidades adicionais à aplicação, como geolocalização, análise de dados, redes sociais entre outros.

#### *Persistence*

A parte de Persistência lida com os seguintes componentes:

- **Caching**: é uma técnica que armazena os resultados das operações de leitura em memória ou em serviços externos de cache, como o Redis, para melhorar o desempenho e reduzir a carga no banco de dados.

- **Data Contexts**: os contextos de dados são classes que representam a sessão com o banco de dados e permitem o acesso às entidades do domínio. Elas utilizam o Entity Framework ou outro ORM (Object-Relational Mapper) para realizar as operações de persistência.
- **Data Migrations**: são um conjunto de operações que modificam a estrutura do banco de dados para acompanhar as mudanças no modelo de domínio. Elas são geradas e aplicadas utilizando ferramentas como o Entity Framework Migrations.
- **Data Seeding**: é um processo que insere dados iniciais no banco de dados, como informações de configuração, categorias e usuários padrão.
- **Repositories**: os repositórios são classes que implementam o acesso aos dados e fornecem métodos para persistir, atualizar, excluir e consultar as entidades do domínio.

### 4. Presentation (Apresentação)

A camada de Apresentação, também chamada de User Interface, é responsável por interagir com o usuário e exibir os dados e ações disponíveis na aplicação. Ela pode ser uma aplicação Web, uma API ou uma interface de linha de comando. Os seguintes componentes podem estar presentes nesta camada:

- **Controllers**: são classes que gerenciam a lógica da interface de usuário, processam as requisições dos usuários e geram as respostas. Eles interagem com a camada de Aplicação para executar as ações solicitadas e atualizar a exibição.

- **Filters/Attributes**: são componentes que podem ser aplicados aos controladores ou métodos de ação para executar ações antes ou depois da execução do método. Eles são úteis para implementar funcionalidades comuns, como validação de entrada, tratamento de exceções e controle de acesso.
- **Middlewares**: são componentes que interceptam e processam as requisições e respostas antes que elas cheguem aos controladores ou retornem para o cliente. Eles podem ser usados para implementar funcionalidades como autenticação, autorização, logging e tratamento de erros.
- **Views**: são templates que definem a estrutura e a aparência da interface de usuário. Elas são preenchidas com os dados fornecidos pelos controladores e podem incluir elementos como tabelas, formulários, gráficos e mapas.
- **View Models**: são objetos que representam o estado e os dados necessários para renderizar uma view específica. Eles são usados para transferir dados entre os controladores e as views e podem incluir informações como listas de itens, estados de botões e mensagens de erro.
- **Worker Service**: é um tipo de aplicação .NET que executa tarefas em segundo plano de maneira contínua e independente, sem interação direta com o usuário. Essas tarefas podem incluir operações de longa duração, processamento de mensagens de filas, monitoramento de sistemas, execução de trabalhos agendados e integração com serviços externos. O Worker Service é uma ótima opção para executar atividades que não se encaixam no modelo de requisição-resposta típico de uma aplicação Web ou API. Ele pode ser configurado para iniciar automaticamente com o sistema operacional, executar em paralelo com outras aplicações e ser gerenciado por ferramentas de monitoramento e orquestração de contêineres, como o Kubernetes. Na camada de Apresentação, um Worker Service serve para lidar com tarefas assíncronas e offload das operações de processamento intensivo da aplicação principal, melhorando a escalabilidade, a disponibilidade e o desempenho geral do sistema.

## Recursos e Componentes

Abaixo está uma lista dos recursos e componentes utilizados neste template e uma breve explicação sobre cada um deles:

### CQRS (Command Query Responsibility Segregation)

[CQRS](https://learn.microsoft.com/en-us/azure/architecture/patterns/cqrs) é um padrão arquitetônico que separa as operações de leitura e escrita em diferentes modelos, promovendo a separação de responsabilidades e melhorando o desempenho e escalabilidade da aplicação.

### UnitOfWork

[UnitOfWork](https://www.devmedia.com.br/unit-of-work-o-padrao-de-unidade-de-trabalho-net/25811) é um padrão de design que gerencia a persistência dos dados de maneira consistente e simplifica a manipulação das transações.

### ProblemDetails

[ProblemDetails](https://github.com/khellang/Middleware) é um padrão de mensagens de erro no formato JSON que segue as diretrizes da RFC 7807. Ele permite padronizar as respostas de erro da API e facilitar o tratamento e a compreensão dos erros pelos clientes da API.

### AspNetCoreRateLimit

[AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit) é uma biblioteca para limitar a taxa de requisições às APIs, protegendo a aplicação contra ataques de negação de serviço (DoS) e garantindo a disponibilidade dos recursos.

### Distributed Cache

[Distributed Cache](https://github.com/dotnet/runtime) é um mecanismo de cache distribuído que permite armazenar dados em cache de maneira centralizada e compartilhada entre diferentes instâncias da aplicação, melhorando o desempenho e a escalabilidade.

### Entity Framework Core

[Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/) é um ORM (Object-Relational Mapper) para .NET que permite o mapeamento entre objetos de domínio e as entidades do banco de dados, simplificando o acesso e a manipulação dos dados.

### FluentValidation

[FluentValidation](https://fluentvalidation.net/) é uma biblioteca para validar os objetos de domínio, garantindo a consistência e integridade dos dados.

### Mapster

[Mapster](https://github.com/MapsterMapper/Mapster) é uma biblioteca de mapeamento de objetos para .NET, que facilita a conversão de objetos entre diferentes camadas e modelos da aplicação, como entidades, DTOs e ViewModels.

### MediatR

[MediatR](https://github.com/jbogard/MediatR) é uma biblioteca que permite a implementação do padrão de design "Mediator" em .NET, facilitando a comunicação entre os diferentes componentes da aplicação.

### Polly

[Polly](https://github.com/App-vNext/Polly) é uma biblioteca de resiliência e tolerância a falhas para .NET, que permite a implementação de políticas, como Retry e Circuit Breaker, para lidar com falhas temporárias e proteger a aplicação contra falhas.

### Serilog

[Serilog](https://serilog.net/) é uma biblioteca de logging estruturado para .NET que permite a gravação e monitoramento dos eventos e erros da aplicação, facilitando a análise e a solução de problemas. Possui uma vasta variedade de plugins permitindo gravar os logs em diferentes destinos, desde o simples console até num ELK ou banco de dados.

### Swagger

[Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) é uma ferramenta que facilita a documentação, a visualização e a interação com as APIs RESTful. Neste projeto, é usado para gerar documentação e interface gráfica com versionamento para a API.

### Throw

[Throw](https://github.com/amantinband/throw) é um componente do tipo Guard Clause que fornece uma abordagem simplificada e mais expressiva para lidar com exceções e erros em aplicativos .NET. Com ele, é possível criar exceções personalizadas, definir mensagens de erro detalhadas e incluir informações adicionais sobre o contexto do erro. Ele também permite encadear exceções, tornando mais fácil rastrear a origem do problema.

## Para os Testes Unitários

### xUnit

[xUnit](https://github.com/xunit/xunit) é um framework de testes unitários para a plataforma .NET. Ele é uma ferramenta popular e moderna para escrever e executar testes automatizados em projetos .NET e C#. O xUnit é projetado com uma sintaxe clara e fácil de ler, além de oferecer suporte a atributos e convenções de teste. Ele ajuda os desenvolvedores a garantir que o código funcione conforme o esperado, a detectar regressões e a validar a lógica de negócios em um ambiente isolado e controlado.

### FluentAssertions

[FluentAssertions](https://github.com/fluentassertions/fluentassertions) é uma biblioteca de asserções para .NET que oferece uma sintaxe fluente e fácil de ler para escrever asserções em testes unitários. A biblioteca fornece uma ampla variedade de métodos de extensão para validar o resultado das operações e comparar objetos complexos. Com o FluentAssertions, os desenvolvedores podem escrever asserções de teste mais expressivas e legíveis, melhorando a qualidade e a manutenção dos testes unitários. Além disso, a biblioteca fornece mensagens de erro detalhadas e formatadas quando uma asserção falha, facilitando a identificação e correção dos problemas.

### Moq

[Moq](https://github.com/moq/moq4) é uma biblioteca de simulação (mocking) para .NET que permite criar objetos fictícios (mocks) de classes e interfaces em tempo de execução. Esses objetos fictícios são úteis para simular o comportamento de dependências externas ou internas durante os testes unitários. Ao usar o Moq, os desenvolvedores podem substituir as partes reais do sistema por versões simuladas que imitam o comportamento esperado. Isso permite testar o código de maneira isolada, sem depender de recursos externos, como bancos de dados ou serviços da Web.

## C# Scripting

Este projeto inclui um script [CSX](https://visualstudiomagazine.com/articles/2021/06/14/csharp-scripting.aspx) para gerar novas entidades e toda a estrutura do CQRS, facilitando a consistência na estrutura de pastas e arquivos da solução. 

### Como utilizar o script CSX?

Para adicionar novas entidades e endpoints à API da solução, siga os passos abaixo:

1. Instale a ferramenta global <b>dotnet-script</b> executando no terminal o seguinte comando:  
>     dotnet tool install -g dotnet-script

2. Vá para a pasta <b>scripts</b> onde encontra-se o arquivo <b>class-generator.csx</b>

3. Para gerar uma nova classe de entidade e todos os seus arquivos derivados, execute o comando abaixo:  
>     dotnet script class-generator.csx -- "NOME_DA_ENTIDADE"

4. Como regra e recomendação para o nome da entidade, utilize nomes somente em inglês, sempre no singular e entre aspas.

5. O próprio script cuidará de pluralizar o nome da entidade quando for necessário e também ajustará os nomes no formato Pascal Case.

6. Como resultado da execução desse script, serão criados vários arquivos e pastas em locais distintos da solução. Serão criadas as classes de domínio, interfaces, repositório, CQRS, controller entre outras coisas.

7. Ao final da execução, e não ocorrendo nenhum erro, basta rodar o build da solução e você terá um CRUD básico 100% funcional rodando num banco de dados na memória.

8. Altere e adicione os campos e propriedades necessárias à nova entidade criada e personalize a solução do modo que atenda às necessidades do seu projeto.

9. Implemente as regras de negócio e validações conforme necessário.

10. Se necessário, atualize a documentação do Swagger.

## Configuração do banco de dados

O template está configurado para usar 2 bancos de dados na memória para fins de demonstração. Isso garante que todos os usuários possam executar a solução sem a necessidade de ter que configurar infraestrutura adicional (por exemplo, SQL Server). Você pode optar por desligar o banco de dados na memória editando a propriedade "UseInMemoryDatabase" que está no appsettings.

## Contribuindo

Agradeço sua contribuição para este projeto! Para contribuir, siga as etapas abaixo:

1. Faça um fork do repositório.
2. Crie uma branch com suas alterações.
3. Faça um pull request para a branch develop.
4. Aguarde a revisão e aprovação das suas alterações.

## Suporte e Comunidade
Caso você tenha dúvidas, sugestões ou queira contribuir para o projeto, entre em contato com newton.dantas@gmail.com
