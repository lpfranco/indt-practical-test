using ProposalService.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProposalService.Domain.Events
{
    public record ProposalStatusChangedEvent(Guid ProposalId, ProposalStatus NewStatus) : IDomainEvent;
}
