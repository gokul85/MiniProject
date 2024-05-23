﻿using System;
using System.Collections.Generic;

namespace ReturnManagementSystem.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderProducts = new HashSet<OrderProduct>();
            Policies = new HashSet<Policy>();
            ProductItems = new HashSet<ProductItem>();
            ReturnRequests = new HashSet<ReturnRequest>();
        }

        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public string? ProductStatus { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
        public virtual ICollection<Policy> Policies { get; set; }
        public virtual ICollection<ProductItem> ProductItems { get; set; }
        public virtual ICollection<ReturnRequest> ReturnRequests { get; set; }
    }
}
