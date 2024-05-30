using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.OrderDTOs;

namespace ReturnManagementSystem.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrder(OrderDTO orderDTO);
        Task<IEnumerable<Order>> GetAllUserOrders(int userId);
        Task<Order> UpdateOrderStatus(int orderId, string orderStatus);
    }
}
