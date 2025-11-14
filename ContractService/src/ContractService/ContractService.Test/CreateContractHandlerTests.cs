using ContractService.Application.Commands;
using ContractService.Application.Handlers;
using ContractService.Application.Ports;
using ContractService.Domain.Entities;
using ContractService.Domain.Exceptions;
using Moq;

namespace ContractService.Test
{
    public class CreateContractHandlerTests
    {
        private readonly Mock<IProposalStatusCacheRepository> _statusRepositoryMock;
        private readonly Mock<IContractRepository> _contractRepositoryMock;
        private readonly CreateContractHandler _handler;

        public CreateContractHandlerTests()
        {
            _statusRepositoryMock = new Mock<IProposalStatusCacheRepository>();
            _contractRepositoryMock = new Mock<IContractRepository>();

            _handler = new CreateContractHandler(
                _statusRepositoryMock.Object,
                _contractRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenStatusIsNull()
        {
            // Arrange
            var proposalId = Guid.NewGuid();
            var command = new CreateContractCommand(proposalId);

            _statusRepositoryMock
                .Setup(repo => repo.GetStatusAsync(proposalId))
                .ReturnsAsync((string)null);

            // Act & Assert
            await Assert.ThrowsAsync<DomainValidationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenStatusIsNotAprovada()
        {
            // Arrange
            var proposalId = Guid.NewGuid();
            var command = new CreateContractCommand(proposalId);

            _statusRepositoryMock
                .Setup(repo => repo.GetStatusAsync(proposalId))
                .ReturnsAsync("Reprovada");

            // Act & Assert
            await Assert.ThrowsAsync<DomainValidationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldCreateContract_WhenStatusIsAprovada()
        {
            // Arrange
            var proposalId = Guid.NewGuid();
            var command = new CreateContractCommand(proposalId);

            _statusRepositoryMock
                .Setup(repo => repo.GetStatusAsync(proposalId))
                .ReturnsAsync("Aprovada");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert: SaveChangesAsync deve ser chamado
            _contractRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Contract>()), Times.Once);
            _contractRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.NotEqual(Guid.Empty, result);
        }
    }
}