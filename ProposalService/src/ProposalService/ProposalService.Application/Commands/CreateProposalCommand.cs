using MediatR;

namespace ProposalService.Application.Commands
{
    public record CreateProposalCommand(string CustomerName, decimal Amount) : IRequest<Guid>;
}
