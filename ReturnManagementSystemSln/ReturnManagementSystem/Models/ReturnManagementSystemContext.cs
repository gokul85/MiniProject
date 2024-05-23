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
                optionsBuilder.UseSqlServer("Data Source=C1RBBX3\\SQLEXPRESS;Integrated Security=true;Initial Catalog=ReturnManagementSystem");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderId).ValueGeneratedNever();

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.OrderStatus)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Orders__UserId__4316F928");
            });

            modelBuilder.Entity<OrderProduct>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.ProductId, e.SerialNumber })
                    .HasName("PK__OrderPro__00D41DA300D468CA");

                entity.Property(e => e.SerialNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderProd__Order__45F365D3");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderProd__Produ__46E78A0C");

                entity.HasOne(d => d.SerialNumberNavigation)
                    .WithMany(p => p.OrderProducts)
                    .HasForeignKey(d => d.SerialNumber)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderProd__Seria__47DBAE45");
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.PaymentId).ValueGeneratedNever();

                entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.Payments)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__Payments__OrderI__4AB81AF0");
            });

            modelBuilder.Entity<Policy>(entity =>
            {
                entity.Property(e => e.PolicyId).ValueGeneratedNever();

                entity.Property(e => e.Policy1)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("Policy");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Policies)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__Policies__Produc__403A8C7D");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId).ValueGeneratedNever();

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
                entity.HasKey(e => e.SerialNumber)
                    .HasName("PK__ProductI__048A0009322ABF19");

                entity.Property(e => e.SerialNumber)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductItems)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ProductIt__Produ__3D5E1FD2");
            });

            modelBuilder.Entity<RefundTransaction>(entity =>
            {
                entity.Property(e => e.RefundTransactionId).ValueGeneratedNever();

                entity.Property(e => e.TransactionAmount).HasColumnType("decimal(18, 2)");

                entity.Property(e => e.TransactionDate).HasColumnType("datetime");

                entity.Property(e => e.TransactionId)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.Request)
                    .WithMany(p => p.RefundTransactions)
                    .HasForeignKey(d => d.RequestId)
                    .HasConstraintName("FK__RefundTra__Reque__5441852A");
            });

            modelBuilder.Entity<ReturnRequest>(entity =>
            {
                entity.HasKey(e => e.RequestId)
                    .HasName("PK__ReturnRe__33A8517A2D02D8F2");

                entity.Property(e => e.RequestId).ValueGeneratedNever();

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
                    .HasConstraintName("FK__ReturnReq__Close__5165187F");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.ReturnRequests)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK__ReturnReq__Order__4E88ABD4");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ReturnRequests)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK__ReturnReq__Produ__4F7CD00D");

                entity.HasOne(d => d.SerialNumberNavigation)
                    .WithMany(p => p.ReturnRequests)
                    .HasForeignKey(d => d.SerialNumber)
                    .HasConstraintName("FK__ReturnReq__Seria__5070F446");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ReturnRequestUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__ReturnReq__UserI__4D94879B");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

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
                entity.HasNoKey();

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserDetai__UserI__38996AB5");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
