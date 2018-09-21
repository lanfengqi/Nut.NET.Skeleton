using System;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Services {
    public interface IOrderReportService {

        Task<OrderByUserReportLine> GetOrderUserReportLine(string userId, DateTime? startDateUtc = null, DateTime? endDateUtc = null);
    }

    public partial class OrderByUserReportLine {
        public int OrderNum { get; set; }

        public decimal TotalOrder { get; set; }

        public decimal TotalOrderPaymentMoney { get; set; }

        public decimal OrderItemQuantity { get; set; }

        public decimal AvgEnteredPrice { get; set; }

        public decimal TotalEnteredWeight { get; set; }

        public decimal TotalOrderCost { get; set; }
    }
}
