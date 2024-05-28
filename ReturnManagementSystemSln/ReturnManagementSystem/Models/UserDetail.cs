using System;
using System.Collections.Generic;

namespace ReturnManagementSystem.Models
{
    public partial class UserDetail
    {
        public int UserId { get; set; }
        public string? Username { get; set; }
        public byte[]? Password { get; set; }
        public byte[]? PasswordHashKey { get; set; }
        public string? Status { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
