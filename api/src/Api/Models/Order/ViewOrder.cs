using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;

namespace Foundatio.Skeleton.Api.Models {
    public class ViewOrder : IIdentity, IHaveDates {

        public string Id { get; set; }

        public string OrganizationId { get; set; }

        public string UserId { get; set; }

        public string CarId { get; set; }

        public string FarmerId { get; set; }

        public string CustomOrderNumber { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public string PaymentMethodSystemName { get; set; }

        public string PaymentCardNumber { get; set; }

        public decimal OrderTotal { get; set; }

        public decimal OrderPaymentMoney { get; set; }

        public bool Deleted { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedUtc { get; set; }

        public DateTime UpdatedUtc { get; set; }

        public ICollection<ViewOrderItem> OrderItems { get; set; }

        public ICollection<ViewOrderCost> OrderCosts { get; set; }
    }
}
