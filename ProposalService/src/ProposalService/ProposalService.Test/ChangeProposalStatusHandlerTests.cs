using MediatR;
using Moq;
using ProposalService.Application.Commands;
using ProposalService.Application.Handlers;
using ProposalService.Application.Ports;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Domain.Events;
using ProposalService.Domain.Exceptions;

namespace ProposalService.Test
{
    public class ChangeProposalStatusHandlerTests
    {
        private readonly Mock<IProposalRepository> _repositoryMock;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly ChangeProposalStatusHandler _handler;

        public ChangeProposalStatusHandlerTests()
        {
            _repositoryMock = new Mock<IProposalRepository>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _handler = new ChangeProposalStatusHandler(_repositoryMock.Object, _eventPublisherMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenProposalNotFound()
        {
            // Arrange
            var proposalId = Guid.NewGuid();
            var command = new ChangeProposalStatusCommand(proposalId, ProposalStatus.Aprovada);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(proposalId))
                .ReturnsAsync((Proposal)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
                _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Proposta não encontrada.", ex.Message);
        }

        [Fact]
        public async Task Handle_ShouldChangeStatusAndPublishEvent()
        {
            // Arrange
            var proposal = new Proposal("Cliente Teste", 1000m);
            var newStatus = ProposalStatus.Aprovada;
            var command = new ChangeProposalStatusCommand(proposal.Id, newStatus);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(proposal.Id))
                .ReturnsAsync(proposal);

            _repositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            _eventPublisherMock
                .Setup(e => e.PublishAsync(It.IsAny<ProposalStatusChangedIntegrationEvent>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(newStatus, proposal.Status);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
            _eventPublisherMock.Verify(e => e.PublishAsync(
                It.Is<ProposalStatusChangedIntegrationEvent>(ev =>
                    ev.ProposalId == proposal.Id &&
                    ev.NewStatus == newStatus.ToString()
                ),
                "proposal.status.changed"
            ), Times.Once);
            Assert.Equal(Unit.Value, result);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenStatusIsSameAsCurrent()
        {
            // Arrange
            var proposal = new Proposal("Cliente Teste", 1000m);
            var currentStatus = proposal.Status;
            var command = new ChangeProposalStatusCommand(proposal.Id, currentStatus);

            _repositoryMock
                .Setup(r => r.GetByIdAsync(proposal.Id))
                .ReturnsAsync(proposal);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<DomainValidationException>(() =>
                _handler.Handle(command, CancellationToken.None));
            Assert.Equal("A proposta já está neste status.", ex.Message);
        }
    }
}
