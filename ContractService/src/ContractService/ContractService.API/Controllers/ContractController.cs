using ContractService.Application.Commands;
using ContractService.Application.Ports;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ContractService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IContractRepository _repository;

        public ContractController(IMediator mediator, IContractRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        /// <summary>
        /// Cria um novo contrato.
        /// </summary>
        /// <param name="command">Dados necessários para criar o contrato.</param>
        /// <returns>Retorna o ID do contrato criado.</returns>
        /// <response code="201">Contrato criado com sucesso.</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContractCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        /// <summary>
        /// Retorna todos os contratos cadastrados.
        /// </summary>
        /// <returns>Lista de contratos.</returns>
        /// <response code="200">Lista retornada com sucesso.</response>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var contracts = await _repository.GetAllAsync();
            return Ok(contracts);
        }

        /// <summary>
        /// Obtém um contrato pelo ID.
        /// </summary>
        /// <param name="id">ID do contrato.</param>
        /// <returns>Contrato correspondente ao ID informado.</returns>
        /// <response code="200">Contrato encontrado.</response>
        /// <response code="404">Contrato não encontrado.</response>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var contracts = await _repository.GetAllAsync();
            var contract = contracts.FirstOrDefault(c => c.Id == id);
            return contract is null ? NotFound() : Ok(contract);
        }
    }
}
