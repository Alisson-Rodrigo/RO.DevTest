using FluentValidation;

namespace RO.DevTest.Application.Features.Product.Commands.CreatedProductCommand
{
    public class CreatedProductCommandValidator : AbstractValidator<CreatedProductCommand>
    {
        public CreatedProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("O produto necessita de um nome.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("O produto necessita de uma descrição.");
            RuleFor(x => x.Price).NotEmpty().WithMessage("O produto necessita de um preço.");
            RuleFor(x => x.Stock).NotEmpty().WithMessage("O produto necessita de uma quantidade em estoque.");
            RuleFor(x => x.Categories).NotEmpty().WithMessage("O produto necessita de uma categória.");
        }
    }
}
