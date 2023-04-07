using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace Domain.Interfaces;

[System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1515:Single-line comment should be preceded by blank line", Justification = "<Pending>")]
public interface IAppDbContextBase
{
    DbSet<Customer> Customers { get; }
// TEMPLATE - nao remover ou alterar essa linha

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
