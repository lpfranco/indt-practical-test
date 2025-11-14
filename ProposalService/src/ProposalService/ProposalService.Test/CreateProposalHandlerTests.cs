using Moq;
using ProposalService.Application.Commands;
using ProposalService.Application.Handlers;
using ProposalService.Application.Ports;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Exceptions;

namespace ProposalService.Test
{
    public class CreateProposalHandlerTests
    {
        private readonly Mock<IProposalRepository> _proposalRepositoryMock;
        private readonly CreateProposalHandler _handler;

        public CreateProposalHandlerTests()
        {
            _proposalRepositoryMock = new Mock<IProposalRepository>();
            _handler = new CreateProposalHandler(_proposalRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCreateProposalSuccessfully()
        {
            // Arrange
            var customerName = "Cliente Teste";
            var amount = 1000m;
            var command = new CreateProposalCommand(customerName, amount);

            Proposal capturedProposal = null!;
            _proposalRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Proposal>()))
                .Callback<Proposal>(p => capturedProposal = p)
                .Returns(Task.CompletedTask);

            _proposalRepositoryMock
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _proposalRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Proposal>()), Times.Once);
            _proposalRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);

            Assert.NotNull(capturedProposal);
            Assert.Equal(customerName, capturedProposal.CustomerName);
            Assert.Equal(amount, capturedProposal.Amount);
            Assert.Equal(capturedProposal.Id, result); // id retornado é o mesmo da entidade
            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenCustomerNameIsEmpty()
        {
            // Arrange
            var command = new CreateProposalCommand("", 1000m);

            // Act & Assert
            await Assert.ThrowsAsync<DomainValidationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_ShouldThrow_WhenAmountIsZeroOrNegative()
        {
            // Arrange
            var command = new CreateProposalCommand("Cliente", 0m);

            // Act & Assert
            await Assert.ThrowsAsync<DomainValidationException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }
    }

}
