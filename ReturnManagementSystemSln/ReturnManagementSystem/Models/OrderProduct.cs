using System;
using System.Collections.Generic;

namespace ReturnManagementSystem.Models
{
    public partial class OrderProduct
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal? Price { get; set; }
        public string SerialNumber { get; set; } = null!;

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public virtual ProductItem SerialNumberNavigation { get; set; } = null!;
    }
}
