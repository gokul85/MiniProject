using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ReturnManagementSystem.Models
{
    public partial class Transaction
    {
        public int TransactionId { get; set; }
        public int? OrderId { get; set; }
        public int? RequestId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? TransactionAmount { get; set; }
        public string? PaymentGatewayTransactionId { get; set; }
        public string? TransactionType { get; set; }
        [JsonIgnore]
        public virtual Order? Order { get; set; }
        [JsonIgnore]
        public virtual ReturnRequest? Request { get; set; }
    }
}
