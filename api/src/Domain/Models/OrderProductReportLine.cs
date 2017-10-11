namespace Foundatio.Skeleton.Domain.Models {
    public partial class OrderProductReportLine {
        public string ProductId { get; set; }

        public decimal QuantityTotal { get; set; }

        public decimal WeightTotal { get; set; }

        public decimal SaleTotal { get; set; }
    }
}
