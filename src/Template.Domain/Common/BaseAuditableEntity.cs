#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;

namespace Template.Domain.Common;

[ExcludeFromCodeCoverage]
public abstract class BaseAuditableEntity : BaseEntity
{
    protected BaseAuditableEntity(int id) => Id = id;

    protected BaseAuditableEntity()
    {
    }

    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }
}