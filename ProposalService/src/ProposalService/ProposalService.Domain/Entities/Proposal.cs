using ProposalService.Domain.Enums;
using ProposalService.Domain.Events;
using ProposalService.Domain.Exceptions;

namespace ProposalService.Domain.Entities
{
    public class Proposal
    {
        public Guid Id { get; private set; }
        public string CustomerName { get; private set; }
        public decimal Amount { get; private set; }
        public ProposalStatus Status { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private readonly List<IDomainEvent> _events = new();
        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

        private Proposal() { }

        public Proposal(string customerName, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(customerName))
                throw new DomainValidationException("Nome do cliente é obrigatório.");

            if (amount <= 0)
                throw new DomainValidationException("Valor da proposta deve ser positivo.");

            Id = Guid.NewGuid();
            CustomerName = customerName;
            Amount = amount;
            Status = ProposalStatus.EmAnalise;
            CreatedAt = DateTime.UtcNow;

            AddEvent(new ProposalCreatedEvent(Id, CustomerName, Amount));
        }

        public void ChangeStatus(ProposalStatus newStatus)
        {
            if (Status == newStatus)
                throw new DomainValidationException("A proposta já está neste status.");

            Status = newStatus;
            AddEvent(new ProposalStatusChangedEvent(Id, newStatus));
        }

        private void AddEvent(IDomainEvent domainEvent) => _events.Add(domainEvent);
    }
}
