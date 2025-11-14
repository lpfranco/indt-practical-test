namespace ContractService.Domain.Events
{
    public record ContractCreatedEvent(Guid ContractId, Guid ProposalId, DateTime ContractedAt) : IDomainEvent;
}
