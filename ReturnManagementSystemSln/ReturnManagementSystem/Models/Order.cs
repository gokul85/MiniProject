using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ReturnManagementSystem.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderProducts = new HashSet<OrderProduct>();
            ReturnRequests = new HashSet<ReturnRequest>();
            Transactions = new HashSet<Transaction>();
        }

        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? OrderStatus { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        [JsonIgnore]
        public virtual ICollection<ReturnRequest> ReturnRequests { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
