using Foundatio.Skeleton.Repositories.Model;
using System;
using System.Collections.Generic;

namespace Foundatio.Skeleton.Domain.Models {
    public class Order : IOwnedByOrganizationWithIdentity, IHaveDates {

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


        public virtual Organization Organization { get; set; }

        public virtual User User { get; set; }

        public virtual Car Car { get; set; }

        public virtual Farmer Farmer { get; set; }

        private ICollection<OrderItem> _orderItems;

        public virtual ICollection<OrderItem> OrderItems {
            get { return _orderItems ?? (_orderItems = new List<OrderItem>()); }
            protected set { _orderItems = value; }
        }

        private ICollection<OrderCost> _orderCosts;

        public virtual ICollection<OrderCost>  OrderCosts {
            get { return _orderCosts ?? (_orderCosts = new List<OrderCost>()); }
            protected set { _orderCosts = value; }
        }
    }
}

