using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProposalService.Domain.Events
{
    public record ProposalCreatedEvent(Guid ProposalId, string CustomerName, decimal Amount) : IDomainEvent;
}
