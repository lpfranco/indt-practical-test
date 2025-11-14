using ProposalService.Domain.Entities;

namespace ProposalService.Application.Ports
{
    public interface IProposalRepository
    {
        Task AddAsync(Proposal proposal);
        Task<Proposal?> GetByIdAsync(Guid id);
        Task<IEnumerable<Proposal>> GetAllAsync();
        Task SaveChangesAsync();
    }
}
