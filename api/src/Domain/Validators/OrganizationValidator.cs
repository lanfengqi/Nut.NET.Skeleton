using FluentValidation;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Domain.Models;
using System;

namespace Foundatio.Skeleton.Domain.Validators {
    public class OrganizationValidator : AbstractValidator<Organization> {
        public OrganizationValidator() {
            RuleFor(t => t.Id).IsObjectId().When(p => !String.IsNullOrEmpty(p.Id)).WithMessage("Please specify a valid organization id.");

            RuleFor(o => o.Name).NotEmpty().WithMessage("Please specify a valid name.");
        }
    }
}