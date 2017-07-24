using FluentValidation;
using Foundatio.Skeleton.Domain.Models;

namespace Foundatio.Skeleton.Domain.Validators {
    public class RoleValidator : AbstractValidator<Role> {
        public RoleValidator() {
            RuleFor(o => o.Name).NotEmpty().WithMessage("Please specify a valid name.");
            RuleFor(o => o.SystemName).NotEmpty().WithMessage("Please specify a valid System name.");

            RuleFor(t => t.CreatedUtc).NotEmpty().WithMessage("Please specify a valid created date.");
        }
    }
}
