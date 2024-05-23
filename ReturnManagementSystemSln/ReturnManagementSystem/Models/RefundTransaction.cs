using System;
using System.Collections.Generic;

namespace ReturnManagementSystem.Models
{
    public partial class RefundTransaction
    {
        public int RefundTransactionId { get; set; }
        public int? RequestId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public decimal? TransactionAmount { get; set; }
        public string? TransactionId { get; set; }

        public virtual ReturnRequest? Request { get; set; }
    }
}
