using System;
using System.Collections.Generic;

namespace ReturnManagementSystem.Models
{
    public partial class Policy
    {
        public int PolicyId { get; set; }
        public int? ProductId { get; set; }
        public string? Policy1 { get; set; }
        public int? Duration { get; set; }

        public virtual Product? Product { get; set; }
    }
}
