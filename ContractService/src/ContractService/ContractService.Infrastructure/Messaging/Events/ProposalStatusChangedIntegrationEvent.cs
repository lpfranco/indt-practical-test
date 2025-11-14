using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractService.Infrastructure.Messaging.Events
{
    public class ProposalStatusChangedIntegrationEvent
    {
        public Guid ProposalId { get; set; }
        public string NewStatus { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
    }
}
