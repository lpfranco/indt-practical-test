namespace ContractService.Application.Ports
{
    public interface IProposalStatusCacheRepository
    {
        Task UpdateStatusAsync(Guid proposalId, string status);
        Task<string?> GetStatusAsync(Guid proposalId);
    }
}
