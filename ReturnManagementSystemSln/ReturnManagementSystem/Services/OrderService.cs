using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.OrderDTOs;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;

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
        public async Task<OrderReturnDTO> CreateOrder(OrderDTO orderDTO)
        {
            decimal ordertotal = 0;
            var order = new Order()
            {
                UserId = orderDTO.UserId,
                OrderDate = DateTime.Now,
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
                        ordertotal += (decimal)product.Price;
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
                aorder.TotalAmount = ordertotal;
                var updateorder = await _orderRepository.Update(aorder);
                var transactionDTO = new TransactionDTO
                {
                    OrderId = aorder.OrderId,
                    Amount = ordertotal,
                    TransactionId = Guid.NewGuid().ToString(),
                    PaymentDate = DateTime.Now,
                    TransactionType = "Payment"
                };
                await _paymentService.ProcessPayment(transactionDTO);
            }
            catch (Exception ex)
            {
                await _orderRepository.Delete(aorder);
                throw ex;
            }
            return MapOrderReturnDTO(aorder);
        }

        public async Task<IEnumerable<Order>> GetAllOrders()
        {
            var res = await _orderRepository.GetAllWithIncludes(o=>o.Transactions, o=>o.OrderProducts, o =>o.User);
            if (res == null || res.Count() == 0)
                throw new ObjectsNotFoundException("Orders Not Found");
            return res;
        }

        public async Task<IEnumerable<Order>> GetAllUserOrders(int userId)
        {
            var res = await _orderRepository.FindAllWithIncludes(o => o.UserId == userId, o => o.Transactions, o => o.OrderProducts);
            if (res == null || res.Count() == 0)
                throw new ObjectsNotFoundException("Orders Not Found For this User");
            return res;
        }

        public async Task<OrderReturnDTO> GetOrder(int orderid)
        {
            var res = await _orderRepository.FindAllWithIncludes(o=> o.OrderId == orderid, o => o.Transactions, o => o.OrderProducts);
            if (res == null)
                throw new ObjectNotFoundException("Order Not Found");
            return MapOrderReturnDTO(res.First());
        }

        private OrderReturnDTO MapOrderReturnDTO(Order res)
        {
            OrderReturnDTO orderReturnDTO = new OrderReturnDTO()
            {
                OrderId = res.OrderId,
                UserId = res.UserId,
                OrderDate = res.OrderDate,
                TotalAmount = res.TotalAmount,
                OrderStatus = res.OrderStatus,
                OrderProducts = res.OrderProducts.ToList(),
                Transactions = res.Transactions.ToList(),
            };
            return orderReturnDTO;
        }

        public async Task<Order> UpdateOrderStatus(UpdateOrderStatusDTO uosDTO)
        {
            var order = await _orderRepository.Get(uosDTO.OrderId);
            if (order == null)
            {
                throw new ObjectNotFoundException("Order not found");
            }

            order.OrderStatus = uosDTO.OrderStatus;

            return await _orderRepository.Update(order);
        }
    }
}
