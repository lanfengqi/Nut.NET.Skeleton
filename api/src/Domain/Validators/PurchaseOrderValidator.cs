using FluentValidation;
using Foundatio.Skeleton.Domain.Models;

namespace Foundatio.Skeleton.Domain.Validators {
    public class PurchaseOrderValidator : AbstractValidator<PurchaseOrder> {
        public PurchaseOrderValidator() {
            RuleFor(o => o.PurchaseOrderNo).NotEmpty().WithMessage("Please specify a valid Purchase Order No.");
            RuleFor(o => o.PurchaseOrderItems).NotEmpty().WithMessage("Please specify a valid Purchase Order Items.");
        }
    }
}
