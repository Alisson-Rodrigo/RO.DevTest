using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.User.Commands.UpdateUserCommand
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .NotNull()
                .WithMessage("O campo nome deve ser preenchido");
            RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .WithMessage("O campo e-mail precisa ser preenchido");
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("O campo e-mail precisa ser um e-mail válido");
        }
    }
}
