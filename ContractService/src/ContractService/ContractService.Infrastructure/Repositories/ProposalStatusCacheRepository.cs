using ContractService.Application.Ports;

namespace ContractService.Infrastructure.Repositories
{
    public class ProposalStatusCacheRepository : IProposalStatusCacheRepository
    {
        private readonly Dictionary<Guid, string> _cache = new();

        public Task UpdateStatusAsync(Guid proposalId, string status)
        {
            _cache[proposalId] = status;
            return Task.CompletedTask;
        }

        public Task<string?> GetStatusAsync(Guid proposalId)
        {
            _cache.TryGetValue(proposalId, out var status);
            return Task.FromResult(status);
        }
    }

}
