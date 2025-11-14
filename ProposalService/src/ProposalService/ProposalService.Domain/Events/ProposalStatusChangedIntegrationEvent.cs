using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProposalService.Domain.Events
{
    public class ProposalStatusChangedIntegrationEvent
    {
        public Guid ProposalId { get; set; }
        public string NewStatus { get; set; }
        public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    }
}
