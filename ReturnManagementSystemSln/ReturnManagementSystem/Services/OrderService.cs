using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.OrderDTOs;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;
using ReturnManagementSystem.Repositories;

namespace ReturnManagementSystem.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<int, Order> _orderRepository;
        private readonly IRepository<int, OrderProduct> _orderProductRepository;
        private readonly IRepository<int, ProductItem> _productItemRepository;
        private readonly IRepository<int, Product> _productRepository;
        private readonly IPaymentService _paymentService;

        public OrderService(IRepository<int, Order> orderRepository, IRepository<int,ProductItem> pirepo, IRepository<int,Product> prepo, IRepository<int, OrderProduct> orderProductRepository, IPaymentService paymentService)
        {
            _orderRepository = orderRepository;
            _productItemRepository = pirepo;
            _productRepository = prepo;
            _orderProductRepository = orderProductRepository;
            _paymentService = paymentService;
        }
        public async Task<Order> CreateOrder(OrderDTO orderDTO)
        {
            var order = new Order()
            {
                UserId = orderDTO.UserId,
                OrderDate = DateTime.Now,
                TotalAmount = orderDTO.TotalAmount,
                OrderStatus = "Pending"
            };
            var aorder = await _orderRepository.Add(order);
            try
            {
                foreach (var op in orderDTO.OrderProducts)
                {
                    var product = await _productRepository.Get(op.ProductId);
                    if (product == null)
                    {
                        throw new ObjectNotFoundException("Product Not Found");
                    }
                    else if (product.Stock < op.Quantity)
                    {
                        throw new ObjectNotFoundException("Product Out of Stock");
                    }
                    for (int i = 0; i < op.Quantity; i++)
                    {
                        var asn = (await _productItemRepository.FindAll(pi => pi.ProductId == op.ProductId && pi.Status == "Available")).FirstOrDefault();
                        OrderProduct orderproduct = new OrderProduct()
                        {
                            ProductId = op.ProductId,
                            SerialNumber = asn.SerialNumber,
                            OrderId = aorder.OrderId,
                            Price = product.Price,
                        };
                        var res = await _orderProductRepository.Add(orderproduct);
                        asn.Status = "Ordered";
                        await _productItemRepository.Update(asn);
                    }
                }
                var paymentDTO = new PaymentDTO
                {
                    OrderId = aorder.OrderId,
                    Amount = orderDTO.TotalAmount,
                    TransactionId = Guid.NewGuid().ToString()
                };

                await _paymentService.ProcessPayment(paymentDTO);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return aorder;
        }

        public async Task<IEnumerable<Order>> GetAllUserOrders(int userId)
        {
            var res = await _orderRepository.FindAll(o => o.UserId == userId);
            if (res == null || res.Count() == 0)
                throw new ObjectsNotFoundException("Orders Not Found For this User");
            return res;
        }

        public async Task<Order> UpdateOrderStatus(int orderId, string orderStatus)
        {
            var order = await _orderRepository.Get(orderId);
            if (order == null)
            {
                throw new ObjectNotFoundException("Order not found");
            }

            order.OrderStatus = orderStatus;

            return await _orderRepository.Update(order);
        }
    }
}
