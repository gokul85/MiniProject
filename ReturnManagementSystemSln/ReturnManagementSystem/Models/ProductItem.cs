using System;
using System.Collections.Generic;

namespace ReturnManagementSystem.Models
{
    public partial class ProductItem
    {
        public ProductItem()
        {
            OrderProducts = new HashSet<OrderProduct>();
            ReturnRequests = new HashSet<ReturnRequest>();
        }

        public int ProductItemId { get; set; }
        public string SerialNumber { get; set; } = null!;
        public int? ProductId { get; set; }
        public string? Status { get; set; }

        public virtual Product? Product { get; set; }
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        public virtual ICollection<ReturnRequest> ReturnRequests { get; set; }
    }
}
