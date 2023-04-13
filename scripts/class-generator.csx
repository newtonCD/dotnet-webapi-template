#r "Humanizer.dll"

using System;
using System.IO;
using Humanizer;

string searchString = "// TEMPLATE";

string entityNamePlularized = "";
string entityNameLower = "";
string entityNamePlularizedLower = "";

if (Args.Count() > 0)
{
    string className = Args[0];

    className = className.Singularize(inputIsKnownToBePlural: false);
    className = className.Pascalize();

    if (File.Exists($"../src/Domain/Entities/{className}.cs"))
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Já existe uma entidade de domínio com o nome {className}. Tente outro nome.");
        Console.ResetColor();
        return;
    }

    entityNamePlularized = className.Pluralize(inputIsKnownToBeSingular: false);
    entityNameLower = className.ToLower();
    entityNamePlularizedLower = entityNamePlularized.ToLower();

    GenerateFolders(className);
    GenerateEntity(className);
    GenerateCreatedEventNotification(className);
    GenerateDeletedEventNotification(className);
    GenerateUpdatedEventNotification(className);
    GenerateDomainInterface(className);
    GenerateDTO(className);
    GenerateRepository(className);
    GenerateController(className);
    GenerateCQRSFiles(className);

    string IAppDbContextBaseInsertionString = $@"    DbSet<{className}> {entityNamePlularized} {{ get; }}";
    UpdateFile("../src/Domain/Interfaces/IAppDbContextBase.cs", searchString, IAppDbContextBaseInsertionString);

    string appDbContextBaseInsertionString = $@"    public DbSet<{className}> {entityNamePlularized} => Set<{className}>();";
    UpdateFile("../src/Infrastructure/Persistance/AppDbContextBase.cs", searchString, appDbContextBaseInsertionString);

    string testAppQueryDbContextInsertionString = $@"    public DbSet<{className}> {entityNamePlularized} {{ get; set; }}";
    UpdateFile("../tests/Template.Application.Tests/TestAppQueryDbContext.cs", searchString, testAppQueryDbContextInsertionString);
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("Por favor, forneça o nome da classe como argumento.");
    Console.ResetColor();
}

void GenerateFolders(string entityName)
{
    var entityNamePlularized = entityName.Pluralize(inputIsKnownToBeSingular: false);

    if (!System.IO.Directory.Exists($"../src/Domain/Events/{entityName}Events"))
    {
        System.IO.Directory.CreateDirectory($"../src/Domain/Events/{entityName}Events");
    }

    if (!System.IO.Directory.Exists($"../src/Application/Features/{entityNamePlularized}/Commands"))
    {
        System.IO.Directory.CreateDirectory($"../src/Application/Features/{entityNamePlularized}/Commands");
    }

    if (!System.IO.Directory.Exists($"../src/Application/Features/{entityNamePlularized}/EventHandlers"))
    {
        System.IO.Directory.CreateDirectory($"../src/Application/Features/{entityNamePlularized}/EventHandlers");
    }

    if (!System.IO.Directory.Exists($"../src/Application/Features/{entityNamePlularized}/Queries"))
    {
        System.IO.Directory.CreateDirectory($"../src/Application/Features/{entityNamePlularized}/Queries");
    }

    Console.WriteLine("Criada a estrutura de pastas.", entityName);
}

void GenerateEntity(string entityName)
{
    var entityCode = $@"using Template.Domain.Common;
using Template.Domain.Events.{entityName}Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Template.Domain.Entities;

public class {entityName} : BaseAuditableEntity
{{
    private readonly List<INotification> _domainEvents = new();

    public {entityName}(string campo01, string campo02)
        : base()
    {{
        Campo01 = campo01;
        Campo02 = campo02;
    }}

    public {entityName}(int id, string campo01, string campo02)
     : base(id)
    {{
        Campo01 = campo01;
        Campo02 = campo02;
    }}

    public {entityName}()
    {{
    }}

    public IReadOnlyCollection<INotification> DomainEvents => new ReadOnlyCollection<INotification>(_domainEvents);

    public string Campo01 {{ get; private set; }}
    public string Campo02 {{ get; private set; }}

    public void Update(string campo01, string campo02)
    {{
        Campo01 = campo01;
        Campo02 = campo02;
    }}

    public void Add{entityName}CreatedEvent()
    {{
        _domainEvents.Add(new {entityName}CreatedEventNotification(Id, Campo01, Campo02, DateTime.UtcNow));
    }}

    public void Add{entityName}DeletedEvent()
    {{
        _domainEvents.Add(new {entityName}DeletedEventNotification(Id, DateTime.UtcNow));
    }}

    public void Add{entityName}UpdatedEvent()
    {{
        _domainEvents.Add(new {entityName}UpdatedEventNotification(Id, Campo01, Campo02, DateTime.UtcNow));
    }}
}}";
    File.WriteAllText($"../src/Domain/Entities/{entityName}.cs", entityCode);
    Console.WriteLine("Arquivo criado: {0}.cs", entityName);
}

void GenerateCreatedEventNotification(string entityName)
{
    var createdEventNotificationCode = $@"using MediatR;
using System;

namespace Template.Domain.Events.{entityName}Events;

public class {entityName}CreatedEventNotification : INotification
{{
    public {entityName}CreatedEventNotification(int {entityNameLower}Id, string campo01, string campo02, DateTime eventDateTime)
    {{
        {entityName}Id = {entityNameLower}Id;
        Campo01 = campo01;
        Campo02 = campo02;
        EventDateTime = eventDateTime;
    }}

    public int {entityName}Id {{ get; }}
    public string Campo01 {{ get; }}
    public string Campo02 {{ get; }}
    public DateTime EventDateTime {{ get; }}
}}";
    File.WriteAllText($"../src/Domain/Events/{entityName}Events/{entityName}CreatedEventNotification.cs", createdEventNotificationCode);
    Console.WriteLine("Arquivo criado: {0}CreatedEventNotification.cs", entityName);
}

void GenerateDeletedEventNotification(string entityName)
{
    var deletedEventNotificationCode = $@"using MediatR;
using System;

namespace Template.Domain.Events.{entityName}Events;

public class {entityName}DeletedEventNotification : INotification
{{
    public {entityName}DeletedEventNotification(int {entityNameLower}Id, DateTime occurredOn)
    {{
        {entityName}Id = {entityNameLower}Id;
        EventDateTime = occurredOn;
    }}

    public int {entityName}Id {{ get; }}
    public DateTime EventDateTime {{ get; }}
}}";
    File.WriteAllText($"../src/Domain/Events/{entityName}Events/{entityName}DeletedEventNotification.cs", deletedEventNotificationCode);
    Console.WriteLine("Arquivo criado: {0}DeletedEventNotification.cs", entityName);
}

void GenerateUpdatedEventNotification(string entityName)
{
    var updatedEventNotificationCode = $@"using MediatR;
using System;

namespace Template.Domain.Events.{entityName}Events;

public class {entityName}UpdatedEventNotification : INotification
{{
    public {entityName}UpdatedEventNotification(int {entityNameLower}Id, string campo01, string campo02, DateTime eventDateTime)
    {{
        {entityName}Id = {entityNameLower}Id;
        Campo01 = campo01;
        Campo02 = campo02;
        EventDateTime = eventDateTime;
    }}

    public int {entityName}Id {{ get; }}
    public string Campo01 {{ get; }}
    public string Campo02 {{ get; }}
    public DateTime EventDateTime {{ get; }}
}}";
    File.WriteAllText($"../src/Domain/Events/{entityName}Events/{entityName}UpdatedEventNotification.cs", updatedEventNotificationCode);
    Console.WriteLine("Arquivo criado: {0}UpdatedEventNotification.cs", entityName);
}

void GenerateDomainInterface(string entityName)
{
    var domainInterfaceCode = $@"using Template.Domain.Entities;

namespace Template.Domain.Interfaces;

public interface I{entityName}Repository : IBaseRepository<{entityName}>
{{
}}";
    File.WriteAllText($"../src/Domain/Interfaces/I{entityName}Repository.cs", domainInterfaceCode);
    Console.WriteLine("Arquivo criado: I{0}Repository.cs", entityName);
}

void GenerateDTO(string entityName)
{
    var dtoCode = $@"namespace Template.Application.Common.Models.DTOs;

public class {entityName}Dto
{{
    public int Id {{ get; set; }}
    public string Campo01 {{ get; set; }}
    public string Campo02 {{ get; set; }}
}}";
    File.WriteAllText($"../src/Application/Common/Models/DTOs/{entityName}Dto.cs", dtoCode);
    Console.WriteLine("Arquivo criado: {0}Dto.cs", entityName);
}

void GenerateConfiguration(string entityName)
{
    var configurationsCode = $@"using Template.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Template.Infrastructure.Configurations;

public class {entityName}Configuration : IEntityTypeConfiguration<{entityName}>
{{
    /// <summary>
    /// Este método será chamado automaticamente pelo Entity Framework Core quando se utiliza a função
    /// modelBuilder.ApplyConfigurationsFromAssembly no método OnModelCreating da classe de contexto.
    /// </summary>
    /// <param name=""builder""></param>
    public void Configure(EntityTypeBuilder<{entityName}> builder)
    {{
        builder?.HasKey(c => c.Id);
        builder.Property(c => c.Campo01).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Campo02).IsRequired().HasMaxLength(100);
    }}
}}";
    File.WriteAllText($"../src/Infrastructure/Configurations/{entityName}Configuration.cs", configurationsCode);
    Console.WriteLine("Arquivo criado: {0}Configuration.cs", entityName);
}

void GenerateRepository(string entityName)
{
    var repositoryCode = $@"using Template.Domain.Entities;
using Template.Domain.Interfaces;
using Template.Infrastructure.Persistance;

namespace Template.Infrastructure.Repositories;

public class {entityName}Repository : BaseRepository<{entityName}>, I{entityName}Repository
{{
    public {entityName}Repository(AppCommandDbContext commandContext, AppQueryDbContext queryContext)
        : base(commandContext, queryContext)
    {{
    }}
}}";
    File.WriteAllText($"../src/Infrastructure/Repositories/{entityName}Repository.cs", repositoryCode);
    Console.WriteLine("Arquivo criado: {0}Repository.cs", entityName);
}

void GenerateController(string entityName)
{
    var controllerCode = $@"using Template.Application.Common.Exceptions;
using Template.Application.Common.Models;
using Template.Application.Features.{entityNamePlularized}.Commands;
using Template.Application.Features.{entityNamePlularized}.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Throw;
using Template.WebApi.Presenters;

namespace Template.WebApi.Controllers.V1;

public sealed class {entityName}Controller : ApiControllerBase
{{
    public {entityName}Controller(ISender sender)
        : base(sender)
    {{
    }}

    /// <summary>
    /// Obter um {entityNameLower} pelo seu ID.
    /// </summary>
    /// <param name=""{entityNameLower}Id"">O ID do {entityNameLower} a ser obtido.</param>
    /// <param name=""cancellationToken""></param>
    /// <returns>O {entityNameLower} com o ID especificado.</returns>
    [HttpGet(""{{{entityNameLower}Id:int}}"")]
    [ProducesResponseType(typeof({entityName}Response), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int {entityNameLower}Id, CancellationToken cancellationToken)
    {{
        {entityName}Response {entityNameLower} = await Sender.Send(new Get{entityName}ByIdQuery({entityNameLower}Id), cancellationToken).ConfigureAwait(false);

        return Ok({entityNameLower});
    }}

    /// <summary>
    /// Cadastrar um novo {entityNameLower}.
    /// </summary>
    /// <param name=""command"">Dados do {entityNameLower} a ser criado.</param>
    /// <param name=""cancellationToken""></param>
    /// <returns>O ID do {entityNameLower} criado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create([FromBody] Create{entityName}Command command, CancellationToken cancellationToken)
    {{
        Result<int> result = await Sender.Send(command, cancellationToken).ConfigureAwait(false);

        return result.Succeeded
            ? CreatedAtAction(nameof(Create), new {{ {entityNameLower}Id = result.Data }}, result)
            : UnprocessableEntity(new CustomProblemDetails(result.Errors));
    }}

    /// <summary>
    /// Deletar um {entityNameLower} existente.
    /// </summary>
    /// <param name=""{entityNameLower}Id"">O ID do {entityNameLower} a ser deletado.</param>
    /// <param name=""cancellationToken""></param>
    /// <returns>Confirmação de que o {entityNameLower} foi deletado com sucesso.</returns>
    [HttpDelete(""{{{entityNameLower}Id:int}}"")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int {entityNameLower}Id, CancellationToken cancellationToken)
    {{
        await Sender.Send(new Delete{entityName}Command({entityNameLower}Id), cancellationToken).ConfigureAwait(false);
        return NoContent();
    }}

    /// <summary>
    /// Atualizar um {entityNameLower} existente.
    /// </summary>
    /// <param name=""command"">Um objeto json com os dados atualizados do {entityNameLower}.</param>
    /// <param name=""cancellationToken""></param>
    /// <returns>Confirmação de que o {entityNameLower} foi atualizado com sucesso.</returns>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult> Update([FromBody] Update{entityName}Command command, CancellationToken cancellationToken)
    {{
        await Sender.Send(command, cancellationToken).ConfigureAwait(false);
        return NoContent();
    }}

    /// <summary>
    /// Obtém todos os {entityNamePlularizedLower} com paginação.
    /// </summary>
    /// <param name=""pageNumber"">O número da página a ser recuperado (inicia em 1).</param>
    /// <param name=""pageSize"">A quantidade de itens por página (padrão é 10).</param>
    /// <returns>Uma ActionResult contendo um objeto Paged{entityName}Response com a lista paginada de {entityNamePlularizedLower} e informações de paginação.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(Paged{entityName}Response), StatusCodes.Status200OK)]
    public async Task<ActionResult<Paged{entityName}Response>> GetAll{entityNamePlularized}([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {{
        Get{entityNamePlularized}Query query = new Get{entityNamePlularized}Query(pageNumber, pageSize);
        Paged{entityName}Response result = await Sender.Send(query).ConfigureAwait(false);

        return Ok(result);
    }}

    /// <summary>
    /// Obtém todos os {entityNamePlularizedLower} com base no valor do campo01.
    /// </summary>
    /// <param name=""campo01"">O valor do campo01 a ser usado como critério de pesquisa.</param>
    /// <returns>Uma ActionResult contendo uma lista de objetos {entityName}SummaryResponse com os {entityNamePlularizedLower} que correspondem a valor do campo01 fornecido.</returns>
    [HttpGet(""search/by-campo01"")]
    [ProducesResponseType(typeof(IEnumerable<{entityName}SummaryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<{entityName}SummaryResponse>>> Get{entityNamePlularized}ByCampo01(string campo01)
    {{
        campo01.ThrowIfNull(() => throw new NotFoundException(""O campo01 não foi informado.""));

        Get{entityNamePlularized}ByCampo02Query query = new Get{entityNamePlularized}ByCampo02Query(campo01);
        IEnumerable<{entityName}SummaryResponse> {entityNamePlularizedLower} = await Sender.Send(query).ConfigureAwait(false);

        return Ok({entityNamePlularizedLower});
    }}
}}";
    File.WriteAllText($"../src/WebApi/Controllers/V1/{entityName}Controller.cs", controllerCode);
    Console.WriteLine("Arquivo criado: {0}Controller.cs", entityName);
}

void GenerateCQRSFiles(string entityName)
{
    // Commands

    var createCommandCode = $@"using Template.Application.Common.Models;
using MediatR;

namespace Template.Application.Features.{entityNamePlularized}.Commands;

/// <summary>
/// Create{entityName}Command representa um comando para criar um novo {entityNameLower}.
/// </summary>
public sealed record Create{entityName}Command(string Campo01, string Campo02) : IRequest<Result<int>>;
";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Commands/Create{entityName}Command.cs", createCommandCode);
    Console.WriteLine("Arquivo criado: Create{0}Command.cs", entityName);

    var createCommandHandlerCode = $@"using Template.Application.Common.Models;
using Template.Domain.Entities;
using Template.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Throw;

namespace Template.Application.Features.{entityNamePlularized}.Commands;

/// <summary>
/// Create{entityName}CommandHandler é responsável por lidar com a criação de um novo {entityNameLower}.
/// </summary>
public sealed class Create{entityName}CommandHandler : IRequestHandler<Create{entityName}Command, Result<int>>
{{
    private readonly I{entityName}Repository _{entityNameLower}Repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    /// <summary>
    /// Inicializa uma nova instância da classe Create{entityName}CommandHandler.
    /// </summary>
    /// <param name=""{entityNameLower}Repository"">Repositório de {entityNamePlularizedLower}.</param>
    /// <param name=""unitOfWork"">Unidade de trabalho.</param>
    /// <param name=""mediator"">Mediador.</param>
    public Create{entityName}CommandHandler(I{entityName}Repository {entityNameLower}Repository, IUnitOfWork unitOfWork, IMediator mediator)
    {{
        _{entityNameLower}Repository = {entityNameLower}Repository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }}

    /// <summary>
    /// Trata o comando de criação de um novo {entityNameLower}.
    /// </summary>
    /// <param name=""request"">Comando para criar um novo {entityNameLower}.</param>
    /// <param name=""cancellationToken"">Token de cancelamento.</param>
    /// <returns>Retorna o ID do {entityNameLower} criado.</returns>
    public async Task<Result<int>> Handle(Create{entityName}Command request, CancellationToken cancellationToken)
    {{
        request.ThrowIfNull(() => throw new ArgumentNullException(nameof(request)));

        {entityName} {entityNameLower} = new {entityName}(request.Campo01, request.Campo02);
        {entityNameLower}.Add{entityName}CreatedEvent();

        await _{entityNameLower}Repository.AddAsync({entityNameLower}).ConfigureAwait(false);

        if ({entityNameLower}.DomainEvents != null)
        {{
            foreach (var domainEvent in {entityNameLower}.DomainEvents)
            {{
                await _mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
            }}
        }}

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

        return ResultFactory.Success({entityNameLower}.Id);
    }}
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Commands/Create{entityName}CommandHandler.cs", createCommandHandlerCode);
    Console.WriteLine("Arquivo criado: Create{0}CommandHandler.cs", entityName);

    var createCommandValidatorCode = $@"using Template.Domain.Constants;
using FluentValidation;

namespace Template.Application.Features.{entityNamePlularized}.Commands;

public sealed class Create{entityName}CommandValidator : AbstractValidator<Create{entityName}Command>
{{
    public Create{entityName}CommandValidator()
    {{
        RuleFor(x => x.Campo01).NotEmpty().WithMessage(ValidationMessages.RequiredField);
        RuleFor(x => x.Campo02).NotEmpty().WithMessage(ValidationMessages.RequiredField);
    }}
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Commands/Create{entityName}CommandValidator.cs", createCommandValidatorCode);
    Console.WriteLine("Arquivo criado: Create{0}CommandValidator.cs", entityName);

    var deleteCommandCode = $@"using MediatR;

namespace Template.Application.Features.{entityNamePlularized}.Commands;

/// <summary>
/// Delete{entityName}Command é responsável por representar a requisição de exclusão de um {entityNameLower}.
/// </summary>
public sealed record Delete{entityName}Command(int Id) : IRequest;
";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Commands/Delete{entityName}Command.cs", deleteCommandCode);
    Console.WriteLine("Arquivo criado: Delete{0}Command.cs", entityName);

    var deleteCommandHandlerCode = $@"using Template.Application.Common.Exceptions;
using Template.Domain.Entities;
using Template.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Throw;

namespace Template.Application.Features.{entityNamePlularized}.Commands;

/// <summary>
/// Delete{entityName}CommandHandler é responsável por processar o comando Delete{entityName}Command.
/// </summary>
public sealed class Delete{entityName}CommandHandler : IRequestHandler<Delete{entityName}Command>
{{
    private readonly I{entityName}Repository _{entityNameLower}Repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    /// <summary>
    /// Inicializa uma nova instância da classe Delete{entityName}CommandHandler.
    /// </summary>
    /// <param name=""{entityNameLower}Repository"">Repositório de {entityNamePlularizedLower}.</param>
    /// <param name=""unitOfWork"">Unidade de trabalho.</param>
    /// <param name=""mediator"">Mediador.</param>
    public Delete{entityName}CommandHandler(I{entityName}Repository {entityNameLower}Repository, IUnitOfWork unitOfWork, IMediator mediator)
    {{
        _{entityNameLower}Repository = {entityNameLower}Repository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }}

    /// <summary>
    /// Processa a requisição de exclusão de um {entityNameLower}.
    /// </summary>
    /// <param name=""request"">O comando Delete{entityName}Command contendo as informações do {entityNameLower} a ser excluído.</param>
    /// <param name=""cancellationToken"">Um token de cancelamento que pode ser usado para cancelar a operação.</param>
    public async Task Handle(Delete{entityName}Command request, CancellationToken cancellationToken)
    {{
        request.ThrowIfNull(() => throw new ArgumentNullException(nameof(request)));

        {entityName} {entityNameLower} = await _{entityNameLower}Repository.GetByIdAsync(request.Id).ConfigureAwait(false);
        {entityNameLower}.ThrowIfNull(() => throw new NotFoundException($""Cliente com o ID {{request.Id}} não encontrado.""));
        {entityNameLower}.Add{entityName}DeletedEvent();

        _{entityNameLower}Repository.Delete({entityNameLower});

        if ({entityNameLower}.DomainEvents != null)
        {{
            foreach (var domainEvent in {entityNameLower}.DomainEvents)
            {{
                await _mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
            }}
        }}

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }}
}}
";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Commands/Delete{entityName}CommandHandler.cs", deleteCommandHandlerCode);
    Console.WriteLine("Arquivo criado: Delete{0}CommandHandler.cs", entityName);

    var updateCommandCode = $@"using MediatR;

namespace Template.Application.Features.{entityNamePlularized}.Commands;

/// <summary>
/// Update{entityName}Command é responsável por representar a requisição de atualização de um {entityNameLower}.
/// </summary>
public sealed record Update{entityName}Command(int Id, string Campo01, string Campo02) : IRequest;
";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Commands/Update{entityName}Command.cs", updateCommandCode);
    Console.WriteLine("Arquivo criado: Update{0}Command.cs", entityName);

    var updateCommandHandlerCode = $@"using Template.Application.Common.Exceptions;
using Template.Domain.Entities;
using Template.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Throw;

namespace Template.Application.Features.{entityNamePlularized}.Commands;

/// <summary>
/// Update{entityName}CommandHandler é responsável por processar o comando Update{entityName}Command.
/// </summary>
public sealed class Update{entityName}CommandHandler : IRequestHandler<Update{entityName}Command>
{{
    private readonly I{entityName}Repository _{entityNameLower}Repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    /// <summary>
    /// Inicializa uma nova instância da classe Update{entityName}CommandHandler.
    /// </summary>
    /// <param name=""{entityNameLower}Repository"">Repositório de {entityNamePlularizedLower}.</param>
    /// <param name=""unitOfWork"">Unidade de trabalho.</param>
    /// <param name=""mediator"">Mediador.</param>
    public Update{entityName}CommandHandler(I{entityName}Repository {entityNameLower}Repository, IUnitOfWork unitOfWork, IMediator mediator)
    {{
        _{entityNameLower}Repository = {entityNameLower}Repository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }}

    /// <summary>
    /// Processa a requisição de atualização de um {entityNameLower}.
    /// </summary>
    /// <param name=""request"">O comando Update{entityName}Command contendo as informações do {entityNameLower} a ser atualizado.</param>
    /// <param name=""cancellationToken"">Um token de cancelamento que pode ser usado para cancelar a operação.</param>
    /// <returns>Um objeto Result que representa a conclusão bem-sucedida da operação.</returns>
    public async Task Handle(Update{entityName}Command request, CancellationToken cancellationToken)
    {{
        request.ThrowIfNull(() => throw new ArgumentNullException(nameof(request)));

        {entityName} {entityNameLower} = await _{entityNameLower}Repository.GetByIdAsync(request.Id).ConfigureAwait(false);
        {entityNameLower}.ThrowIfNull(() => throw new NotFoundException($""Cliente com o ID {{request.Id}} não encontrado.""));
        {entityNameLower}.Update(request.Campo01, request.Campo02);
        {entityNameLower}.Add{entityName}UpdatedEvent();

        _{entityNameLower}Repository.Update({entityNameLower});

        if ({entityNameLower}.DomainEvents != null)
        {{
            foreach (var domainEvent in {entityNameLower}.DomainEvents)
            {{
                await _mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
            }}
        }}

        await _unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }}
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Commands/Update{entityName}CommandHandler.cs", updateCommandHandlerCode);
    Console.WriteLine("Arquivo criado: Update{0}CommandHandler.cs", entityName);

    var updateCommandValidatorCode = $@"using Template.Domain.Constants;
using FluentValidation;

namespace Template.Application.Features.{entityNamePlularized}.Commands;

/// <summary>
/// Update{entityName}CommandValidator é responsável por validar o comando Update{entityName}Command.
/// </summary>
public sealed class Update{entityName}CommandValidator : AbstractValidator<Update{entityName}Command>
{{
    /// <summary>
    /// Inicializa uma nova instância da classe Update{entityName}CommandValidator.
    /// </summary>
    public Update{entityName}CommandValidator()
    {{
        RuleFor(x => x.Campo01).NotEmpty().WithMessage(ValidationMessages.RequiredField);
        RuleFor(x => x.Campo02).NotEmpty().WithMessage(ValidationMessages.RequiredField);
    }}
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Commands/Update{entityName}CommandValidator.cs", updateCommandValidatorCode);
    Console.WriteLine("Arquivo criado: Update{0}CommandValidator.cs", entityName);

    // Event Handlers

    var createNotificationHandlerCode = $@"using Template.Application.Common.Interfaces;
using Template.Domain.Entities;
using Template.Domain.Events.{entityName}Events;
using Template.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Template.Application.Features.{entityNamePlularized}.EventHandlers;

/// <summary>
/// Classe responsável por tratar a notificação do evento de {entityNameLower} criado.
/// </summary>
public class {entityName}CreatedNotificationHandler : INotificationHandler<{entityName}CreatedEventNotification>
{{
    private readonly IAppQueryDbContext _queryDbContext;
    private readonly IDbOperationConfiguration _dbOperationConfiguration;

    public {entityName}CreatedNotificationHandler(
        IAppQueryDbContext queryDbContext,
        IDbOperationConfiguration dbOperationConfiguration)
    {{
        _queryDbContext = queryDbContext;
        _dbOperationConfiguration = dbOperationConfiguration;
    }}

    /// <summary>
    /// Método que trata a notificação do evento de {entityNameLower} criado.
    /// </summary>
    /// <param name=""notification"">A notificação contendo informações do evento de {entityNameLower} criado.</param>
    /// <param name=""cancellationToken"">Um token de cancelamento que pode ser usado para cancelar a operação.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    public async Task Handle({entityName}CreatedEventNotification notification, CancellationToken cancellationToken)
    {{
        if (_dbOperationConfiguration.UseSingleDatabase()) return;

#pragma warning disable CA1062 // Validate arguments of public methods
        {entityName} {entityNameLower} = new {entityName}(notification.Campo01, notification.Campo02)
        {{
            Created = notification.EventDateTime
        }};
#pragma warning restore CA1062 // Validate arguments of public methods

        // Insere o {entityNameLower} no contexto de leitura
        await _queryDbContext.{entityNamePlularized}.AddAsync({entityNameLower}, cancellationToken).ConfigureAwait(false);
        await _queryDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }}
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/EventHandlers/{entityName}CreatedNotificationHandler.cs", createNotificationHandlerCode);
    Console.WriteLine("Arquivo criado: {0}CreatedNotificationHandler.cs", entityName);

    var deleteNotificationHandlerCode = $@"using Template.Application.Common.Interfaces;
using Template.Domain.Entities;
using Template.Domain.Events.{entityName}Events;
using Template.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Template.Application.Features.{entityNamePlularized}.EventHandlers;

/// <summary>
/// Classe responsável por tratar a notificação do evento de {entityNameLower} excluído.
/// </summary>
public class {entityName}DeletedNotificationHandler : INotificationHandler<{entityName}DeletedEventNotification>
{{
    private readonly IAppQueryDbContext _queryDbContext;
    private readonly IDbOperationConfiguration _dbConfiguration;
    private readonly ICacheService _cacheService;

    public {entityName}DeletedNotificationHandler(
        IAppQueryDbContext readDbContext,
        IDbOperationConfiguration dbConfiguration,
        ICacheService cacheService)
    {{
        _queryDbContext = readDbContext;
        _dbConfiguration = dbConfiguration;
        _cacheService = cacheService;
    }}

    /// <summary>
    /// Método que trata a notificação do evento de {entityNameLower} excluído.
    /// </summary>
    /// <param name=""notification"">A notificação contendo informações do evento de {entityNameLower} excluído.</param>
    /// <param name=""cancellationToken"">Um token de cancelamento que pode ser usado para cancelar a operação.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    public async Task Handle({entityName}DeletedEventNotification notification, CancellationToken cancellationToken)
    {{
        if (_dbConfiguration.UseSingleDatabase()) return;

#pragma warning disable CA1062 // Validate arguments of public methods
        {entityName} {entityNameLower} = await _queryDbContext.{entityNamePlularized}.FindAsync(new object[] {{ notification.{entityName}Id }}, cancellationToken: cancellationToken).ConfigureAwait(false);
#pragma warning restore CA1062 // Validate arguments of public methods

        if ({entityNameLower} != null)
        {{
            // Exclui o {entityNameLower} no contexto de leitura
            _queryDbContext.{entityNamePlularized}.Remove({entityNameLower});
            await _queryDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Invalida o cache para o registro excluído
            string cacheKey = $""{entityName}:{{notification.{entityName}Id}}"";
            await _cacheService.RemoveAsync(cacheKey, cancellationToken).ConfigureAwait(false);
        }}
    }}
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/EventHandlers/{entityName}DeletedNotificationHandler.cs", deleteNotificationHandlerCode);
    Console.WriteLine("Arquivo criado: {0}DeletedNotificationHandler.cs", entityName);

    var updateNotificationHandlerCode = $@"using Template.Application.Common.Interfaces;
using Template.Domain.Entities;
using Template.Domain.Events.{entityName}Events;
using Template.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Template.Application.Features.{entityNamePlularized}.EventHandlers;

/// <summary>
/// Classe responsável por tratar a notificação do evento de {entityNameLower} atualizado.
/// </summary>
public class {entityName}UpdatedNotificationHandler : INotificationHandler<{entityName}UpdatedEventNotification>
{{
    private readonly IAppQueryDbContext _queryDbContext;
    private readonly IDbOperationConfiguration _dbConfiguration;
    private readonly ICacheService _cacheService;

    public {entityName}UpdatedNotificationHandler(
        IAppQueryDbContext queryDbContext,
        IDbOperationConfiguration dbConfiguration,
        ICacheService cacheService)
    {{
        _queryDbContext = queryDbContext;
        _dbConfiguration = dbConfiguration;
        _cacheService = cacheService;
    }}

    /// <summary>
    /// Método que trata a notificação do evento de {entityNameLower} atualizado.
    /// </summary>
    /// <param name=""notification"">A notificação contendo informações do evento de {entityNameLower} atualizado.</param>
    /// <param name=""cancellationToken"">Um token de cancelamento que pode ser usado para cancelar a operação.</param>
    /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
    public async Task Handle({entityName}UpdatedEventNotification notification, CancellationToken cancellationToken)
    {{
        if (_dbConfiguration.UseSingleDatabase()) return;

#pragma warning disable CA1062 // Validate arguments of public methods
        {entityName} {entityNameLower} = await _queryDbContext.{entityNamePlularized}.FindAsync(new object[] {{ notification.{entityName}Id }}, cancellationToken: cancellationToken).ConfigureAwait(false);
#pragma warning restore CA1062 // Validate arguments of public methods

        if ({entityNameLower} != null)
        {{
            {entityNameLower}.Update(notification.Campo01, notification.Campo02);

            // Atualize o {entityNameLower} no contexto de leitura
            _queryDbContext.{entityNamePlularized}.Update({entityNameLower});
            await _queryDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Invalida o cache para o registro atualizado
            string cacheKey = $""{entityName}:{{notification.{entityName}Id}}"";
            await _cacheService.RemoveAsync(cacheKey, cancellationToken).ConfigureAwait(false);
        }}
    }}
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/EventHandlers/{entityName}UpdatedNotificationHandler.cs", updateNotificationHandlerCode);
    Console.WriteLine("Arquivo criado: {0}UpdatedNotificationHandler.cs", entityName);

    // Queries

    var responseCode = $@"using System;

namespace Template.Application.Features.{entityNamePlularized}.Queries;

public sealed record {entityName}Response(int Id, string Campo01, string Campo02, DateTime Created, string CreatedBy);
";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Queries/{entityName}Response.cs", responseCode);
    Console.WriteLine("Arquivo criado: {0}Response.cs", entityName);

    var summaryResponseCode = $@"namespace Template.Application.Features.{entityNamePlularized}.Queries;

/// <summary>
/// Classe que representa a resposta resumida de um {entityNameLower}.
/// </summary>
public sealed record {entityName}SummaryResponse(int Id, string Campo01, string Campo02);
";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Queries/{entityName}SummaryResponse.cs", summaryResponseCode);
    Console.WriteLine("Arquivo criado: {0}SummaryResponse.cs", entityName);

    var pagedResponseCode = $@"using System.Collections.ObjectModel;

namespace Template.Application.Features.{entityNamePlularized}.Queries;

/// <summary>
/// Classe que representa a resposta paginada de {entityNamePlularizedLower}.
/// </summary>
public sealed record Paged{entityName}Response(int PageNumber, int PageSize, int TotalPages, int TotalItems, Collection<{entityName}SummaryResponse> Items);
";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Queries/Paged{entityName}Response.cs", pagedResponseCode);
    Console.WriteLine("Arquivo criado: Paged{0}Response.cs", entityName);

    var getByIdQueryCode = $@"using Template.Application.Common.Cache;
using Template.Application.Common.Interfaces;
using MediatR;

namespace Template.Application.Features.{entityNamePlularized}.Queries;

public sealed class Get{entityName}ByIdQuery : CacheableQueryBase, IRequest<{entityName}Response>, ICacheable
{{
    public Get{entityName}ByIdQuery(int {entityNameLower}Id)
    {{
        {entityName}Id = {entityNameLower}Id;
    }}

    public int {entityName}Id {{ get; }}

    public override string CacheKey => $""{entityName}:{{{entityName}Id}}"";
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Queries/Get{entityName}ByIdQuery.cs", getByIdQueryCode);
    Console.WriteLine("Arquivo criado: Get{0}ByIdQuery.cs", entityName);

    var getByIdQueryHandlerCode = $@"using Template.Application.Common.Exceptions;
using Template.Domain.Entities;
using Template.Domain.Interfaces;
using Mapster;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Throw;

namespace Template.Application.Features.{entityNamePlularized}.Queries;

public sealed class Get{entityName}ByIdQueryHandler : IRequestHandler<Get{entityName}ByIdQuery, {entityName}Response>
{{
    private readonly IBaseRepository<{entityName}> _{entityNameLower}Repository;

    public Get{entityName}ByIdQueryHandler(IBaseRepository<{entityName}> {entityNameLower}Repository)
    {{
        _{entityNameLower}Repository = {entityNameLower}Repository;
    }}

    public async Task<{entityName}Response> Handle(Get{entityName}ByIdQuery request, CancellationToken cancellationToken)
    {{
#pragma warning disable CA1062 // Validate arguments of public methods
        {entityName} {entityNameLower} = await _{entityNameLower}Repository.GetByIdAsync(request.{entityName}Id).ConfigureAwait(false);
#pragma warning restore CA1062 // Validate arguments of public methods
        {entityNameLower}.ThrowIfNull(() => throw new NotFoundException($""{entityName} com o ID {{request.{entityName}Id}} não encontrado.""));

        return {entityNameLower}.Adapt<{entityName}Response>();
    }}
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Queries/Get{entityName}ByIdQueryHandler.cs", getByIdQueryHandlerCode);
    Console.WriteLine("Arquivo criado: Get{0}ByIdQueryHandler.cs", entityName);

    var getByCampo02QueryCode = $@"using Template.Application.Common.Cache;
using Template.Application.Common.Interfaces;
using MediatR;
using System.Collections.Generic;

namespace Template.Application.Features.{entityNamePlularized}.Queries;

public sealed class Get{entityNamePlularized}ByCampo02Query : CacheableQueryBase, IRequest<IEnumerable<{entityName}SummaryResponse>>, ICacheable
{{
    public Get{entityNamePlularized}ByCampo02Query(string campo02)
    {{
        Campo02 = campo02;
    }}

    public string Campo02 {{ get; }}

    public override string CacheKey => $""{entityName}:Campo02:{{Campo02}}"";
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Queries/Get{entityNamePlularized}ByCampo02Query.cs", getByCampo02QueryCode);
    Console.WriteLine("Arquivo criado: Get{0}ByCampo02Query.cs", entityNamePlularized);

    var getByCampo02QueryHandlerCode = $@"using Template.Domain.Entities;
using Template.Domain.Interfaces;
using Mapster;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Template.Application.Features.{entityNamePlularized}.Queries;

public sealed class Get{entityNamePlularized}ByCampo02QueryHandler : IRequestHandler<Get{entityNamePlularized}ByCampo02Query, IEnumerable<{entityName}SummaryResponse>>
{{
    private readonly IBaseRepository<{entityName}> _{entityNameLower}Repository;

    public Get{entityNamePlularized}ByCampo02QueryHandler(IBaseRepository<{entityName}> {entityNameLower}Repository)
    {{
        _{entityNameLower}Repository = {entityNameLower}Repository;
    }}

    public async Task<IEnumerable<{entityName}SummaryResponse>> Handle(Get{entityNamePlularized}ByCampo02Query request, CancellationToken cancellationToken)
    {{
        Expression<Func<{entityName}, bool>> predicate = {entityNameLower} => {entityNameLower}.Campo02 == request.Campo02;
        IEnumerable<{entityName}> {entityNamePlularizedLower} = await _{entityNameLower}Repository.FindAsync(predicate).ConfigureAwait(false);

        return {entityNamePlularizedLower}.Adapt<IEnumerable<{entityName}SummaryResponse>>();
    }}
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Queries/Get{entityNamePlularized}ByCampo02QueryHandler.cs", getByCampo02QueryHandlerCode);
    Console.WriteLine("Arquivo criado: Get{0}ByCampo02QueryHandler.cs", entityNamePlularized);

    var getQueryCode = $@"using Template.Application.Common.Cache;
using Template.Application.Common.Interfaces;
using Template.Application.Features.Base;
using MediatR;

namespace Template.Application.Features.{entityNamePlularized}.Queries;

/// <summary>
/// Classe de consulta que representa a solicitação para obter todos os {entityNamePlularizedLower}.
/// </summary>
public sealed class Get{entityNamePlularized}Query : CacheableQueryBase, IRequest<Paged{entityName}Response>, IGetPagingQuery, ICacheable
{{
    public Get{entityNamePlularized}Query(int pageNumber, int pageSize)
    {{
        PageNumber = pageNumber;
        PageSize = pageSize;
    }}

    public int PageNumber {{ get; }}
    public int PageSize {{ get; }}

    public override string CacheKey => $""{entityName}:Paging:{{PageNumber}}_{{PageSize}}"";
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Queries/Get{entityNamePlularized}Query.cs", getQueryCode);
    Console.WriteLine("Arquivo criado: Get{0}Query.cs", entityNamePlularized);

    var getQueryHandlerCode = $@"using Template.Application.Features.Base;
using Template.Domain.Entities;
using Template.Domain.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;

namespace Template.Application.Features.{entityNamePlularized}.Queries;

/// <summary>
/// Classe responsável por tratar a consulta para obter todos os {entityNamePlularizedLower}.
/// </summary>
public sealed class Get{entityNamePlularized}QueryHandler : PagedQueryHandlerBase<Get{entityNamePlularized}Query, {entityName}, {entityName}SummaryResponse, Paged{entityName}Response>
{{
    public Get{entityNamePlularized}QueryHandler(IBaseRepository<{entityName}> {entityNameLower}Repository)
        : base({entityNameLower}Repository)
    {{
    }}

    /// <summary>
    /// Retorna o número da página da consulta.
    /// </summary>
    /// <param name=""request"">A consulta para obter todos os {entityNamePlularizedLower}.</param>
    /// <returns>O número da página.</returns>
    protected override int GetPageNumber(Get{entityNamePlularized}Query request)
    {{
        return request.PageNumber;
    }}

    /// <summary>
    /// Retorna o tamanho da página da consulta.
    /// </summary>
    /// <param name=""request"">A consulta para obter todos os {entityNamePlularizedLower}.</param>
    /// <returns>O tamanho da página.</returns>
    protected override int GetPageSize(Get{entityNamePlularized}Query request)
    {{
        return request.PageSize;
    }}

    /// <summary>
    /// Cria a resposta paginada para a consulta.
    /// </summary>
    /// <param name=""pageNumber"">O número da página.</param>
    /// <param name=""pageSize"">O tamanho da página.</param>
    /// <param name=""totalPages"">O número total de páginas.</param>
    /// <param name=""totalItems"">O número total de itens.</param>
    /// <param name=""items"">A coleção de itens resumidos dos {entityNamePlularizedLower}.</param>
    /// <returns>A resposta paginada de {entityNamePlularizedLower}.</returns>
    protected override Paged{entityName}Response CreatePagedResponse(int pageNumber, int pageSize, int totalPages, int totalItems, ReadOnlyCollection<{entityName}SummaryResponse> items)
    {{
        return new Paged{entityName}Response(pageNumber, pageSize, totalPages, totalItems, new Collection<{entityName}SummaryResponse>(items.ToList()));
    }}
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Queries/Get{entityNamePlularized}QueryHandler.cs", getQueryHandlerCode);
    Console.WriteLine("Arquivo criado: Get{0}QueryHandler.cs", entityNamePlularized);

    var getQueryValidatorCode = $@"using Template.Application.Features.Base;

namespace Template.Application.Features.{entityNamePlularized}.Queries;

/// <summary>
/// Classe que valida a consulta para obter todos os {entityNamePlularizedLower}.
/// </summary>
public sealed class Get{entityNamePlularized}QueryValidator : GetPagingQueryValidator<Get{entityNamePlularized}Query>
{{
}}";
    File.WriteAllText($"../src/Application/Features/{entityNamePlularized}/Queries/Get{entityNamePlularized}QueryValidator.cs", getQueryValidatorCode);
    Console.WriteLine("Arquivo criado: Get{0}QueryValidator.cs", entityNamePlularized);
}

public static void UpdateFile(string filePath, string searchString, string insertionString)
{
    string[] lines = File.ReadAllLines(filePath);
    using (StreamWriter sw = new StreamWriter(filePath))
    {
        foreach (string line in lines)
        {
            if (line.Contains(searchString))
            {
                sw.WriteLine(insertionString);
            }
            sw.WriteLine(line);
        }
    }
}