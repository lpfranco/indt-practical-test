using ContractService.Application.Ports;
using ContractService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContractService.Infrastructure.Repositories
{
    public class ContractRepository : IContractRepository
    {
        private readonly ContractDbContext _context;
        public ContractRepository(ContractDbContext context) => _context = context;
        public async Task AddAsync(Contract contract) => await _context.Contracts.AddAsync(contract);
        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task<IEnumerable<Contract>> GetAllAsync() => await _context.Contracts.ToListAsync();
    }
}
