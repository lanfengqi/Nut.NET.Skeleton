using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Validators {
    public class ProductValidator : AbstractValidator<Models.Product> {
        public ProductValidator() {
            RuleFor(t => t.Id).NotEmpty().WithMessage("Please specify a valid id.");
            RuleFor(t => t.Code).NotEmpty().WithMessage("Please specify a valid code.");
            RuleFor(t => t.Name).NotEmpty().WithMessage("Please specify a valid name.");
            RuleFor(t => t.CreatedUtc).NotEmpty().WithMessage("Please specify a valid created date.");
            RuleFor(t => t.UpdatedUtc).NotEmpty().WithMessage("Please specify a valid modified date.");
        }
    }
}
