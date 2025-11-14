using MediatR;
using ProposalService.Application.Commands;
using ProposalService.Application.Ports;
using ProposalService.Domain.Entities;

namespace ProposalService.Application.Handlers
{
    public class CreateProposalHandler : IRequestHandler<CreateProposalCommand, Guid>
    {
        private readonly IProposalRepository _repository;

        public CreateProposalHandler(IProposalRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateProposalCommand request, CancellationToken cancellationToken)
        {
            var proposal = new Proposal(request.CustomerName, request.Amount);
            await _repository.AddAsync(proposal);
            await _repository.SaveChangesAsync();
            return proposal.Id;
        }
    }
}
