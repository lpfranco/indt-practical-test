using MediatR;

namespace ContractService.Application.Commands
{
    public record CreateContractCommand(Guid ProposalId) : IRequest<Guid>;
}
