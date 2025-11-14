using ContractService.Domain.Entities;
using ContractService.Domain.Events;
using ContractService.Domain.Exceptions;
using System;
using Xunit;

public class ContractTests
{
    [Fact]
    public void Constructor_ShouldThrow_WhenProposalIdIsEmpty()
    {
        // Arrange
        var proposalId = Guid.Empty;

        // Act & Assert
        Assert.Throws<DomainValidationException>(() =>
            new Contract(proposalId));
    }

    [Fact]
    public void Constructor_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var proposalId = Guid.NewGuid();

        // Act
        var contract = new Contract(proposalId);

        // Assert
        Assert.NotEqual(Guid.Empty, contract.Id);
        Assert.Equal(proposalId, contract.ProposalId);

        // tolerância de tempo: 2 segundos
        Assert.True(
            (DateTime.UtcNow - contract.ContractedAt).TotalSeconds < 2,
            "ContractedAt não está próximo do horário atual."
        );
    }

    [Fact]
    public void Constructor_ShouldAddContractCreatedEvent()
    {
        // Arrange
        var proposalId = Guid.NewGuid();

        // Act
        var contract = new Contract(proposalId);

        // Assert
        Assert.NotNull(contract.Events);
        Assert.Single(contract.Events);

        var createdEvent = Assert.IsType<ContractCreatedEvent>(contract.Events.First());

        Assert.Equal(contract.Id, createdEvent.ContractId);
        Assert.Equal(contract.ProposalId, createdEvent.ProposalId);

        // validar ContractedAt do evento
        Assert.True(
            (contract.ContractedAt - createdEvent.ContractedAt).TotalSeconds < 1,
            "ContractedAt do evento deve ser igual ao da entidade."
        );
    }

    [Fact]
    public void Events_ShouldBeReadOnly()
    {
        // Arrange
        var proposalId = Guid.NewGuid();
        var contract = new Contract(proposalId);

        // Act
        var events = contract.Events;

        // Assert
        Assert.IsAssignableFrom<IReadOnlyCollection<IDomainEvent>>(events);

        // Tenta modificar através da lista interna (lança InvalidCastException se tentar forçar)
        var list = events as List<IDomainEvent>;
        Assert.Null(list); // não deve ser List, garantindo que não é modificável diretamente
    }
}
