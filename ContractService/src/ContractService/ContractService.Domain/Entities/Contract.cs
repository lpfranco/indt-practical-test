using ContractService.Domain.Events;
using ContractService.Domain.Exceptions;

namespace ContractService.Domain.Entities
{
    public class Contract
    {
        public Guid Id { get; private set; }
        public Guid ProposalId { get; private set; }
        public DateTime ContractedAt { get; private set; }

        private readonly List<IDomainEvent> _events = new();
        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

        private Contract() { }

        public Contract(Guid proposalId)
        {
            if (proposalId == Guid.Empty)
                throw new DomainValidationException("O ID da proposta é obrigatório.");

            Id = Guid.NewGuid();
            ProposalId = proposalId;
            ContractedAt = DateTime.UtcNow;

            AddEvent(new ContractCreatedEvent(Id, ProposalId, ContractedAt));
        }

        private void AddEvent(IDomainEvent @event) => _events.Add(@event);
    }
}
