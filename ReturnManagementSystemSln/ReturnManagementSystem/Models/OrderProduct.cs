using System;
using System.Collections.Generic;

namespace ReturnManagementSystem.Models
{
    public partial class OrderProduct
    {
        public int OrderProductId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public decimal? Price { get; set; }
        public string? SerialNumber { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
        public virtual ProductItem? SerialNumberNavigation { get; set; }
    }
}
