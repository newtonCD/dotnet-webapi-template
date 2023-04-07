using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Domain.Common;

public abstract class BaseEntity
{
    protected BaseEntity(int id) => Id = id;

    protected BaseEntity()
    {
    }

    [Key]
    public int Id { get; protected set; }
}