using FluentValidation;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Domain.Models;
using System;

namespace Foundatio.Skeleton.Domain.Validators {
    public class OrderValidator : AbstractValidator<Order> {

        public OrderValidator() {
            RuleFor(t => t.Id).IsObjectId().When(p => !String.IsNullOrEmpty(p.Id)).WithMessage("Please specify a valid organization id.");
            RuleFor(o => o.FarmerId).IsObjectId().When(p => !String.IsNullOrEmpty(p.Id)).WithMessage("Please specify a valid farmer Id.");
            RuleFor(o => o.UserId).IsObjectId().When(p => !String.IsNullOrEmpty(p.Id)).WithMessage("Please specify a valid User Id.");
            RuleFor(o => o.CarId).IsObjectId().When(p => !String.IsNullOrEmpty(p.Id)).WithMessage("Please specify a valid Car Id.");

            RuleFor(o => o.PaymentMethodSystemName).NotEmpty().WithMessage("Please specify a valid Payment Method System Name.");
        }
    }
}
