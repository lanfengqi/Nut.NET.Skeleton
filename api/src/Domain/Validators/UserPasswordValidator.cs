using FluentValidation;
using Foundatio.Skeleton.Domain.Models;

namespace Foundatio.Skeleton.Domain.Validators {
    public class UserPasswordValidator : AbstractValidator<UserPassword> {
        public UserPasswordValidator() {
            RuleFor(o => o.Salt).NotEmpty().WithMessage("Please specify a valid Salt.");
            RuleFor(o => o.Password).NotEmpty().WithMessage("Please specify a valid Password.");

            RuleFor(t => t.CreatedUtc).NotEmpty().WithMessage("Please specify a valid created date.");
        }

    }
}
