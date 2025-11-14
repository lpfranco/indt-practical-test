using MediatR;
using ProposalService.Application.Commands;
using ProposalService.Application.Ports;
using ProposalService.Domain.Events;
using ProposalService.Domain.Exceptions;

namespace ProposalService.Application.Handlers
{
    public class ChangeProposalStatusHandler : IRequestHandler<ChangeProposalStatusCommand, Unit>
    {
        private readonly IProposalRepository _repository;
        private readonly IEventPublisher _eventPublisher;
        public ChangeProposalStatusHandler(IProposalRepository repository, IEventPublisher eventPublisher)
        {
            _repository = repository;
            _eventPublisher = eventPublisher;
        }

        public async Task<Unit> Handle(ChangeProposalStatusCommand request, CancellationToken cancellationToken)
        {
            var proposal = await _repository.GetByIdAsync(request.ProposalId);

            if (proposal is null)
                throw new DomainValidationException("Proposta não encontrada.");

            proposal.ChangeStatus(request.NewStatus);

            await _repository.SaveChangesAsync();

            await _eventPublisher.PublishAsync(
                new ProposalStatusChangedIntegrationEvent
                {
                    ProposalId = proposal.Id,
                    NewStatus = proposal.Status.ToString()
                },
                exchangeName: "proposal.status.changed");


            return Unit.Value;
        }
    }
}
