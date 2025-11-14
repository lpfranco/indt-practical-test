using ContractService.Application.Commands;
using ContractService.Application.Ports;
using ContractService.Domain.Entities;
using ContractService.Domain.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractService.Application.Handlers
{
    public class CreateContractHandler : IRequestHandler<CreateContractCommand, Guid>
    {
        private readonly IProposalStatusCacheRepository _statusRepository;
        private readonly IContractRepository _repository;

        public CreateContractHandler(IProposalStatusCacheRepository statusRepository, IContractRepository repository)
        {
            _statusRepository = statusRepository;
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateContractCommand request, CancellationToken cancellationToken)
        {
            var status = await _statusRepository.GetStatusAsync(request.ProposalId);

            if (status is null)
                throw new DomainValidationException("Status da proposta desconhecido. Aguarde sincronização.");

            if (!status.Equals("Aprovada", StringComparison.OrdinalIgnoreCase))
                throw new DomainValidationException("A proposta precisa estar aprovada para ser contratada.");

            var contract = new Contract(request.ProposalId);
            await _repository.AddAsync(contract);
            await _repository.SaveChangesAsync();

            return contract.Id;
        }
    }
}
