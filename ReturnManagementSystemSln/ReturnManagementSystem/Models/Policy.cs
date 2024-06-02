using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ReturnManagementSystem.Models
{
    public partial class Policy
    {
        public int PolicyId { get; set; }
        public int? ProductId { get; set; }
        public string? PolicyType { get; set; }
        public int? Duration { get; set; }
        [JsonIgnore]
        public virtual Product? Product { get; set; }
    }
}
