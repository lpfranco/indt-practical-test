using Microsoft.EntityFrameworkCore;
using ProposalService.Domain.Entities;

namespace ProposalService.Infrastructure
{
    public class ProposalDbContext : DbContext
    {
        public DbSet<Proposal> Proposals => Set<Proposal>();
        public ProposalDbContext(DbContextOptions<ProposalDbContext> options) : base(options) { }
    }
}
