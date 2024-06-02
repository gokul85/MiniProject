using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public virtual Product? Product { get; set; }
        [JsonIgnore]
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        [JsonIgnore]
        public virtual ICollection<ReturnRequest> ReturnRequests { get; set; }
    }
}
