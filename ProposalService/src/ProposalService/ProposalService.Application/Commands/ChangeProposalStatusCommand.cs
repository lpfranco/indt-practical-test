using MediatR;
using ProposalService.Domain.Enums;

namespace ProposalService.Application.Commands
{
    public record ChangeProposalStatusCommand(Guid ProposalId, ProposalStatus NewStatus) : IRequest<Unit>;
}
