using FluentValidation;
using Foundatio.Skeleton.Domain.Models;

namespace Foundatio.Skeleton.Domain.Validators {
    public class CustomerValidator : AbstractValidator<Customer> {
        public CustomerValidator() {
            RuleFor(o => o.Code).NotEmpty().WithMessage("Please specify a valid code.");
            RuleFor(o => o.Name).NotEmpty().WithMessage("Please specify a valid name.");
        }
    }
}
