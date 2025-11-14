using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProposalService.Application.Commands;
using ProposalService.Application.Ports;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;

namespace ProposalService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProposalController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IProposalRepository _repository;

        public ProposalController(IMediator mediator, IProposalRepository repository)
        {
            _mediator = mediator;
            _repository = repository;
        }

        /// <summary>
        /// Cria uma nova proposta.
        /// </summary>
        /// <param name="command">Objeto contendo os dados necessários para criar a proposta.</param>
        /// <returns>Retorna o Id da proposta criada.</returns>
        /// <response code="201">Proposta criada com sucesso.</response>
        /// <response code="400">Dados inválidos enviados na requisição.</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateProposalCommand command)
        {
            var id = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        /// <summary>
        /// Retorna todas as propostas cadastradas.
        /// </summary>
        /// <returns>Lista de propostas.</returns>
        /// <response code="200">Lista retornada com sucesso.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Proposal>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var proposals = await _repository.GetAllAsync();
            return Ok(proposals);
        }

        /// <summary>
        /// Retorna os dados de uma proposta específica pelo identificador único.
        /// </summary>
        /// <param name="id">Identificador da proposta.</param>
        /// <returns>Objeto da proposta encontrada, se existir.</returns>
        /// <response code="200">Proposta encontrada e retornada.</response>
        /// <response code="404">Nenhuma proposta encontrada com o Id informado.</response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(Proposal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var proposal = await _repository.GetByIdAsync(id);
            if (proposal is null)
                return NotFound();

            return Ok(proposal);
        }

        /// <summary>
        /// Altera o status de uma proposta existente.
        /// </summary>
        /// <param name="id">Identificador da proposta.</param>
        /// <param name="newStatus">Novo status desejado para a proposta.</param>
        /// <returns>Nenhum conteúdo.</returns>
        /// <response code="204">Status alterado com sucesso.</response>
        /// <response code="400">Status inválido ou dados inconsistentes.</response>
        [HttpPatch("{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] ProposalStatus newStatus)
        {
            await _mediator.Send(new ChangeProposalStatusCommand(id, newStatus));
            return NoContent();
        }
    }
}
