using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistance;

namespace Infrastructure.Repositories;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppCommandDbContext commandContext, AppQueryDbContext queryContext)
        : base(commandContext, queryContext)
    {
    }
}