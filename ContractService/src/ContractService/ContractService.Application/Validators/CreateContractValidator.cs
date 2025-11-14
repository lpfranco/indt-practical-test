using ContractService.Application.Commands;
using FluentValidation;


namespace ContractService.Application.Validators
{
    public class CreateContractValidator : AbstractValidator<CreateContractCommand>
    {
        public CreateContractValidator()
        {
            RuleFor(x => x.ProposalId)
                .NotEmpty().WithMessage("O ID da proposta é obrigatório.");
        }
    }
}
