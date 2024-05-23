using System;
using System.Collections.Generic;

namespace ReturnManagementSystem.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
            Payments = new HashSet<Payment>();
            ReturnRequests = new HashSet<ReturnRequest>();
        }

        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? OrderStatus { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<ReturnRequest> ReturnRequests { get; set; }
    }
}
