using System;
using System.Collections.Generic;

namespace ReturnManagementSystem.Models
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
            ReturnRequestClosedByNavigations = new HashSet<ReturnRequest>();
            ReturnRequestUsers = new HashSet<ReturnRequest>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Role { get; set; }

        public virtual UserDetail? UserDetail { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<ReturnRequest> ReturnRequestClosedByNavigations { get; set; }
        public virtual ICollection<ReturnRequest> ReturnRequestUsers { get; set; }
    }
}
