using System;

namespace Template.Application.Features.Customers.Queries;

public sealed record CustomerResponse(int Id, string Name, string Email, DateTime Created, string CreatedBy);
