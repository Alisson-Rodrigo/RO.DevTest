

using FluentValidation;

namespace RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("O produto necessita de um nome.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("O produto necessita de uma descrição.");
            RuleFor(x => x.Price).NotEmpty().WithMessage("O produto necessita de um preço.");
            RuleFor(x => x.Category).NotEmpty().WithMessage("O produto necessita de uma categória.");
        }
    }
}
