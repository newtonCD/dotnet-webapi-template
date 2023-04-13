using Template.Domain.Entities;
using Template.Domain.Interfaces;
using Template.Infrastructure.Persistance;

namespace Template.Infrastructure.Repositories;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppCommandDbContext commandContext, AppQueryDbContext queryContext)
        : base(commandContext, queryContext)
    {
    }
}