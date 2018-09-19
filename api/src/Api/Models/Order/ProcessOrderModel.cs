namespace Foundatio.Skeleton.Api.Models {
    public class ProcessOrderModel {
        public string FarmerId { get; set; }

        public string PaymentMethodSystemName { get; set; }

        public string PaymentCardNumber { get; set; }

        public decimal OtherCost { get; set; }

        public string Notes { get; set; }
    }
}
