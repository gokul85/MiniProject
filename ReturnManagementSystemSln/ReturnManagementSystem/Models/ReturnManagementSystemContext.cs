using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ReturnManagementSystem.Models
{
    public partial class ReturnManagementSystemContext : DbContext
    {
        public ReturnManagementSystemContext()
        {
        }

        public ReturnManagementSystemContext(DbContextOptions<ReturnManagementSystemContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderProduct> OrderProducts { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Policy> Policies { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductItem> ProductItems { get; set; } = null!;
        public virtual DbSet<RefundTransaction> RefundTransactions { get; set; } = null!;
        public virtual DbSet<ReturnRequest> ReturnRequests { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserDetail> UserDetails { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data source=C1RBBX3\\SQLEXPRESS;Integrated Security=true;Initial catalog=ReturnManagementSystem");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.OrderStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Orders__UserId__44FF419A");
            });

            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.SerialNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__OrderProd__Order__47DBAE45");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__OrderProd__Produ__48CFD27E");

                entity.HasOne(d => d.SerialNumberNavigation)
                    .WithMany(p => p.OrderProducts)
                    .HasPrincipalKey(p => p.SerialNumber)
                    .HasForeignKey(d => d.SerialNumber)
                    .HasConstraintName("FK__OrderProd__Seria__49C3F6B7");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Payments__OrderI__4CA06362");
            });

            modelBuilder.Entity<Policy>(entity =>
            {
                entity.Property(e => e.PolicyType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Policies)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__Policies__Produc__4222D4EF");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Description).IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.ProductStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ProductItem>(entity =>
            {
                entity.HasIndex(e => e.SerialNumber, "UQ__ProductI__048A000810903ED4")
                    .IsUnique();

                entity.Property(e => e.SerialNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductItems)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ProductIt__Produ__3F466844");
            });

            modelBuilder.Entity<RefundTransaction>(entity =>
            {
                entity.Property(e => e.TransactionAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RefundTransactions)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK__RefundTra__Reque__5629CD9C");
            });

            modelBuilder.Entity<ReturnRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId)
                    .HasName("PK__ReturnRe__33A8517A2112EB0D");

                entity.Property(e => e.ClosedDate).HasColumnType("datetime");

                entity.Property(e => e.Feedback).IsUnicode(false);

                entity.Property(e => e.Process)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Reason).IsUnicode(false);

                entity.Property(e => e.RequestDate).HasColumnType("datetime");

                entity.Property(e => e.ReturnPolicy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SerialNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.ClosedByNavigation)
                    .WithMany(p => p.ReturnRequestClosedByNavigations)
                    .HasForeignKey(d => d.ClosedBy)
                    .HasConstraintName("FK__ReturnReq__Close__534D60F1");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.ReturnRequests)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__ReturnReq__Order__5070F446");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ReturnRequests)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ReturnReq__Produ__5165187F");

                entity.HasOne(d => d.SerialNumberNavigation)
                    .WithMany(p => p.ReturnRequests)
                    .HasPrincipalKey(p => p.SerialNumber)
                    .HasForeignKey(d => d.SerialNumber)
                    .HasConstraintName("FK__ReturnReq__Seria__52593CB8");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ReturnRequestUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__ReturnReq__UserI__4F7CD00D");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserDetail>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__UserDeta__1788CC4C59FC1D40");

                entity.Property(e => e.UserId).ValueGeneratedNever();

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Username).IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithOne(p => p.UserDetail)
                    .HasForeignKey<UserDetail>(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UserDetai__UserI__398D8EEE");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
