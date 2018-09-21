namespace Foundatio.Skeleton.Api.Models {
    public class OrderByUserReportLineModel {

        public int OrderNum { get; set; }

        public decimal TotalOrder { get; set; }

        public decimal TotalOrderPaymentMoney { get; set; }

        public decimal OrderItemQuantity { get; set; }

        public decimal AvgEnteredPrice { get; set; }

        public decimal TotalEnteredWeight { get; set; }

        public decimal TotalOrderCost { get; set; }
    }
}
