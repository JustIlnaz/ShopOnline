using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ShopOnline.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Basket> Baskets { get; set; }

    public virtual DbSet<BasketItem> BasketItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Login> Logins { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=sql-ser-larisa\\serv1215;Database= ShopOnline;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(e => e.IdBasket);

            entity.ToTable("Basket");

            entity.Property(e => e.IdBasket).HasColumnName("Id_Basket");
            entity.Property(e => e.DateStart)
                .HasColumnType("datetime")
                .HasColumnName("Date_Start");
            entity.Property(e => e.UsersId).HasColumnName("Users_Id");

            entity.HasOne(d => d.Users).WithMany(p => p.Baskets)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Basket_Users");
        });

        modelBuilder.Entity<BasketItem>(entity =>
        {
            entity.HasKey(e => e.IdBasketItems);

            entity.Property(e => e.IdBasketItems).HasColumnName("Id_BasketItems");
            entity.Property(e => e.BasketId).HasColumnName("Basket_Id");
            entity.Property(e => e.Count).HasColumnType("text");
            entity.Property(e => e.ProductsId).HasColumnName("Products_Id");

            entity.HasOne(d => d.Basket).WithMany(p => p.BasketItems)
                .HasForeignKey(d => d.BasketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BasketItems_Basket");

            entity.HasOne(d => d.Products).WithMany(p => p.BasketItems)
                .HasForeignKey(d => d.ProductsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BasketItems_Products");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.IdCategories);

            entity.Property(e => e.IdCategories).HasColumnName("Id_Categories");
            entity.Property(e => e.NameCategories).HasColumnType("text");
        });

        modelBuilder.Entity<Login>(entity =>
        {
            entity.HasKey(e => e.IdLogins);

            entity.Property(e => e.IdLogins).HasColumnName("Id_Logins");
            entity.Property(e => e.Login1).HasMaxLength(256);
            entity.Property(e => e.Password).HasColumnType("text");
            entity.Property(e => e.UserId).HasColumnName("User_Id");

            entity.HasOne(d => d.User).WithMany(p => p.Logins)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Logins_Users");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.IdOrders);

            entity.Property(e => e.IdOrders).HasColumnName("Id_Orders");
            entity.Property(e => e.DateZakaza)
                .HasColumnType("datetime")
                .HasColumnName("Date_Zakaza");
            entity.Property(e => e.Status).HasColumnType("text");
            entity.Property(e => e.SumAll)
                .HasColumnType("text")
                .HasColumnName("Sum_all");
            entity.Property(e => e.UsersId).HasColumnName("Users_Id");

            entity.HasOne(d => d.Users).WithMany(p => p.Orders)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Orders_Users");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.IdOrderItems);

            entity.Property(e => e.IdOrderItems).HasColumnName("Id_OrderItems");
            entity.Property(e => e.Count).HasColumnType("text");
            entity.Property(e => e.OrdersId).HasColumnName("Orders_Id");
            entity.Property(e => e.PriceZaEd).HasColumnType("text");
            entity.Property(e => e.ProductsId).HasColumnName("Products_Id");

            entity.HasOne(d => d.Orders).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrdersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Orders");

            entity.HasOne(d => d.Products).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_Products");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.IdProducts);

            entity.Property(e => e.IdProducts).HasColumnName("Id_Products");
            entity.Property(e => e.CategoryId).HasColumnName("Category_Id");
            entity.Property(e => e.DateSave).HasColumnType("datetime");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.NameProducts).HasColumnType("text");
            entity.Property(e => e.Price).HasColumnType("text");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Products_Categories");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRoles);

            entity.Property(e => e.IdRoles).HasColumnName("Id_Roles");
            entity.Property(e => e.NameRole).HasColumnType("text");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUsers);

            entity.Property(e => e.IdUsers).HasColumnName("Id_Users");
            entity.Property(e => e.Description).HasColumnType("text");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasColumnType("text");
            entity.Property(e => e.PhoneNumber).HasMaxLength(20);
            entity.Property(e => e.RoleId).HasColumnName("Role_Id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
