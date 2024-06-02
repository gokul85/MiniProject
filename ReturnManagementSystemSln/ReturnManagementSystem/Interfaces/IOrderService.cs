using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.OrderDTOs;

namespace ReturnManagementSystem.Interfaces
{
    public interface IOrderService
    {
        Task<OrderReturnDTO> CreateOrder(OrderDTO orderDTO);
        Task<IEnumerable<Order>> GetAllOrders();
        Task<OrderReturnDTO> GetOrder(int orderid);
        Task<IEnumerable<Order>> GetAllUserOrders(int userId);
        Task<Order> UpdateOrderStatus(UpdateOrderStatusDTO uosDTO);
    }
}
