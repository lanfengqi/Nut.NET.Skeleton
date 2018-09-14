using FluentValidation;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Domain.Models;
using System;

namespace Foundatio.Skeleton.Domain.Validators {
    public class PurchaseCarValidator : AbstractValidator<PurchaseCar> {
        public PurchaseCarValidator() {
            RuleFor(t => t.Id).IsObjectId().When(p => !String.IsNullOrEmpty(p.Id)).WithMessage("Please specify a valid organization id.");

            RuleFor(o => o.UserId).NotEmpty().WithMessage("Please specify a valid User Id.");
        }
    }
}
