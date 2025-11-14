using FluentValidation;
using ProposalService.Application.Commands;

namespace ProposalService.Application.Validators
{
    public class CreateProposalCommandValidator : AbstractValidator<CreateProposalCommand>
    {
        public CreateProposalCommandValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("O nome do cliente é obrigatório.")
                .MaximumLength(200).WithMessage("O nome do cliente não pode ter mais de 200 caracteres.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("O valor da proposta deve ser positivo.");
        }
    }
}
