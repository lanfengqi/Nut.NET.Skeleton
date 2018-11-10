using Foundatio.Logging;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Services {
    public class OrderProcessingService : IOrderProcessingService {
        private readonly ILogger _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly ITemplatedSmsService _templatedSmsService;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IPurchaseCarRepository _purchaseCarRepository;
        private readonly IFarmerRepository _farmerRepository;

        public OrderProcessingService(ILoggerFactory loggerFactory, IOrderRepository orderRepository, ITemplatedSmsService templatedSmsService
            , IAttendanceRepository attendanceRepository, IPurchaseCarRepository purchaseCarRepository,
            IFarmerRepository farmerRepository) {
            _logger = loggerFactory?.CreateLogger<OrderProcessingService>() ?? NullLogger.Instance;

            _orderRepository = orderRepository;
            _templatedSmsService = templatedSmsService;
            _attendanceRepository = attendanceRepository;
            _purchaseCarRepository = purchaseCarRepository;
            _farmerRepository = farmerRepository;
        }

        public async Task<PlaceOrderResult> PlaceOrder(ProcessOrderRequest request) {
            var result = new PlaceOrderResult();

            var currentDate = DateTime.UtcNow.Date;

            var userAttendances = await _attendanceRepository.SearchAttendanceAsync(userId: request.UserId, attendanceDate: currentDate);
            if (userAttendances == null || !userAttendances.Any())
                result.AddError("user attendance is required.");

            var cars = await _purchaseCarRepository.GetByUserIdAsync(userId: request.UserId);
            if (cars == null || !cars.Any())
                result.AddError("user purchase cars is required.");

            var farmer = await _farmerRepository.GetByIdAsync(request.FarmerId);
            if (farmer == null)
                result.AddError("farmer is required.");

            if (!result.Success)
                return result;

            try {
                var order = new Order {
                    Id = Guid.NewGuid().ToString("N"),
                    CarId = userAttendances.FirstOrDefault().CarId,
                    CreatedUtc = DateTime.UtcNow,
                    Deleted = false,
                    OrderStatus = OrderStatus.Complete,
                    FarmerId = request.FarmerId,
                    Notes = request.Notes,
                    OrganizationId = request.OrganizationId,
                    PaymentMethodSystemName = request.PaymentMethodSystemName,
                    PaymentCardNumber = request.PaymentCardNumber,
                    UpdatedUtc = DateTime.UtcNow,
                    UserId = request.UserId
                };
                order.CustomOrderNumber = order.GrenerteOrderCustomNumber();
                await _orderRepository.AddAsync(order);

                foreach (var car in cars) {
                    order.OrderItems.Add(new OrderItem {
                        CreatedUtc = DateTime.UtcNow,
                        EnteredGrossWeight = car.EnteredGrossWeight,
                        EnteredPrice = car.EnteredPrice,
                        EnteredWeight = car.EnteredWeight,
                        Id = Guid.NewGuid().ToString("N"),
                        Lat = car.Lat,
                        Lng = car.Lng,
                        OrderId = order.Id,
                        Order = order,
                        Quantity = car.Quantity,
                        TtemTotal = car.CarTotal,
                        UpdatedUtc = DateTime.UtcNow
                    });
                }
                order.OrderTotal = order.OrderItems.Sum(x => x.TtemTotal);
                await _orderRepository.SaveAsync(order);
                cars.ToList().ForEach(async car => await _purchaseCarRepository.RemoveAsync(car));

                order.OrderCosts.Add(new OrderCost {
                    CostSystemName = "OtherCost",
                    CreatedUtc = DateTime.UtcNow,
                    EnteredMoney = request.OtherCost,
                    Notes = request.OtherCostNotes,
                    OrderId = order.Id,
                    Order = order,
                    UpdatedUtc = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString("N"),
                });
                order.OrderPaymentMoney = order.OrderTotal + request.OtherCost;
                await _orderRepository.SaveAsync(order);

                _templatedSmsService.SendOrderCompletedNotification(order);
                result.PlacedOrder = order;
            } catch (Exception ex) {
                _logger.Error(ex.ToString());
                result.AddError(ex.ToString());
            }

            if (result.Success)
                return result;

            var logError = result.Errors.Aggregate("Error while placing order.",
                (current, next) => $"{current} Error {result.Errors.IndexOf(next) + 1}: {next}.");
            _logger.Error(logError);

            return result;
        }

        public async Task DeleteOrder(Order order) {
            if (order == null)
                throw new ArgumentNullException("order");

            order.Deleted = true;
            await _orderRepository.SaveAsync(order);
        }

        public bool CanCancelOrder(Order order) {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.OrderStatus == OrderStatus.Cancelled)
                return false;

            return true;
        }

        public async Task CancelOrder(Order order, bool NotifyFarmer = true) {
            if (order == null)
                throw new ArgumentNullException("order");

            if (!CanCancelOrder(order))
                throw new Exception("Cannot do cannel for order.");

            order.OrderStatus = OrderStatus.Cancelled;
            await _orderRepository.SaveAsync(order);

            if (NotifyFarmer) {
                _templatedSmsService.SendOrderCannelledNotification(order);
            }
        }
    }
}
