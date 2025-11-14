using ContractService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContractService.Infrastructure
{
    public class ContractDbContext : DbContext
    {
        public ContractDbContext(DbContextOptions<ContractDbContext> options) : base(options) { }

        public DbSet<Contract> Contracts => Set<Contract>();
    }
}
