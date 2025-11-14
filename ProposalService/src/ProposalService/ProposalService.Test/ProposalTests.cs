using ProposalService.Domain.Entities;
using ProposalService.Domain.Enums;
using ProposalService.Domain.Events;
using ProposalService.Domain.Exceptions;

namespace ProposalService.Test
{
    public class ProposalTests
    {
        [Fact]
        public void Constructor_ShouldThrow_WhenCustomerNameIsEmpty()
        {
            // Arrange
            var customerName = "";
            var amount = 1000m;

            // Act & Assert
            var ex = Assert.Throws<DomainValidationException>(() =>
                new Proposal(customerName, amount));
            Assert.Equal("Nome do cliente é obrigatório.", ex.Message);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void Constructor_ShouldThrow_WhenAmountIsZeroOrNegative(decimal amount)
        {
            // Arrange
            var customerName = "Cliente Teste";

            // Act & Assert
            var ex = Assert.Throws<DomainValidationException>(() =>
                new Proposal(customerName, amount));
            Assert.Equal("Valor da proposta deve ser positivo.", ex.Message);
        }

        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            // Arrange
            var customerName = "Cliente Teste";
            var amount = 1000m;

            // Act
            var proposal = new Proposal(customerName, amount);

            // Assert
            Assert.NotEqual(Guid.Empty, proposal.Id);
            Assert.Equal(customerName, proposal.CustomerName);
            Assert.Equal(amount, proposal.Amount);
            Assert.Equal(ProposalStatus.EmAnalise, proposal.Status);
            Assert.True((DateTime.UtcNow - proposal.CreatedAt).TotalSeconds < 2);
        }

        [Fact]
        public void Constructor_ShouldAddProposalCreatedEvent()
        {
            // Arrange
            var customerName = "Cliente Teste";
            var amount = 1000m;

            // Act
            var proposal = new Proposal(customerName, amount);

            // Assert
            var events = proposal.Events;
            Assert.Single(events);

            var createdEvent = Assert.IsType<ProposalCreatedEvent>(events.First());
            Assert.Equal(proposal.Id, createdEvent.ProposalId);
            Assert.Equal(proposal.CustomerName, createdEvent.CustomerName);
            Assert.Equal(proposal.Amount, createdEvent.Amount);
        }

        [Fact]
        public void ChangeStatus_ShouldUpdateStatusAndAddEvent()
        {
            // Arrange
            var proposal = new Proposal("Cliente Teste", 1000m);
            var newStatus = ProposalStatus.Aprovada;

            // Act
            proposal.ChangeStatus(newStatus);

            // Assert
            Assert.Equal(newStatus, proposal.Status);

            var statusChangedEvent = proposal.Events.OfType<ProposalStatusChangedEvent>().FirstOrDefault();
            Assert.NotNull(statusChangedEvent);
            Assert.Equal(proposal.Id, statusChangedEvent.ProposalId);
            Assert.Equal(newStatus, statusChangedEvent.NewStatus);
        }

        [Fact]
        public void ChangeStatus_ShouldThrow_WhenSameStatus()
        {
            // Arrange
            var proposal = new Proposal("Cliente Teste", 1000m);
            var currentStatus = proposal.Status;

            // Act & Assert
            var ex = Assert.Throws<DomainValidationException>(() =>
                proposal.ChangeStatus(currentStatus));
            Assert.Equal("A proposta já está neste status.", ex.Message);
        }

        [Fact]
        public void Events_ShouldBeReadOnly()
        {
            // Arrange
            var proposal = new Proposal("Cliente Teste", 1000m);

            // Act
            var events = proposal.Events;

            // Assert
            Assert.IsAssignableFrom<IReadOnlyCollection<IDomainEvent>>(events);

            // Garantia de que não há Add/Remove disponíveis
            Assert.False(events is List<IDomainEvent>, "Events não deve expor a lista interna.");
        }
    }
}
