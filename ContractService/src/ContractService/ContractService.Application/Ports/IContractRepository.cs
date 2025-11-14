using ContractService.Domain.Entities;

namespace ContractService.Application.Ports
{
    public interface IContractRepository
    {
        Task AddAsync(Contract contract);
        Task SaveChangesAsync();
        Task<IEnumerable<Contract>> GetAllAsync();
    }
}
