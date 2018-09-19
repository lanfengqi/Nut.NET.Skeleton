using Foundatio.Skeleton.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Services {
    public interface IOrderProcessingService {

        Task<PlaceOrderResult> PlaceOrder(ProcessOrderRequest request);

        Task DeleteOrder(Order order);

        bool CanCancelOrder(Order order);

        Task CancelOrder(Order order, bool NotifyFarmer = true);
    }

    public partial class PlaceOrderResult {

        public PlaceOrderResult() {
            this.Errors = new List<string>();
        }

        public bool Success {
            get { return (!Errors.Any()); }
        }

        public void AddError(string error) {
            Errors.Add(error);
        }

        public List<string> Errors { get; set; }

        public Order PlacedOrder { get; set; }
    }

    public partial class ProcessOrderRequest {
        public string OrganizationId { get; set; }

        public string UserId { get; set; }

        public string FarmerId { get; set; }

        public string PaymentMethodSystemName { get; set; }

        public string PaymentCardNumber { get; set; }

        public decimal OtherCost { get; set; }

        public string Notes { get; set; }
    }
}
