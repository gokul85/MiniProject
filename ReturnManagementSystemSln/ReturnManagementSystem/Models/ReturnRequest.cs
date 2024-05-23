using System;
using System.Collections.Generic;

namespace ReturnManagementSystem.Models
{
    public partial class ReturnRequest
    {
        public ReturnRequest()
        {
            RefundTransactions = new HashSet<RefundTransaction>();
        }

        public int RequestId { get; set; }
        public int? UserId { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public string? SerialNumber { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? ReturnPolicy { get; set; }
        public string? Process { get; set; }
        public string? Feedback { get; set; }
        public string? Reason { get; set; }
        public string? Status { get; set; }
        public int? ClosedBy { get; set; }
        public DateTime? ClosedDate { get; set; }

        public virtual User? ClosedByNavigation { get; set; }
        public virtual Order? Order { get; set; }
        public virtual Product? Product { get; set; }
        public virtual ProductItem? SerialNumberNavigation { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<RefundTransaction> RefundTransactions { get; set; }
    }
}
