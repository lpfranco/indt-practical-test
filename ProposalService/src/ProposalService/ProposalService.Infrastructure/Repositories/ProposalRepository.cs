using Microsoft.EntityFrameworkCore;
using ProposalService.Application.Ports;
using ProposalService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProposalService.Infrastructure.Repositories
{
    public class ProposalRepository : IProposalRepository
    {
        private readonly ProposalDbContext _context;

        public ProposalRepository(ProposalDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Proposal proposal) => await _context.Proposals.AddAsync(proposal);

        public async Task<Proposal?> GetByIdAsync(Guid id) => await _context.Proposals.FindAsync(id);

        public async Task<IEnumerable<Proposal>> GetAllAsync() => await _context.Proposals.AsNoTracking().ToListAsync();

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
