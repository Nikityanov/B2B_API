using Microsoft.EntityFrameworkCore;
using B2B_API.Models;

namespace B2B_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PriceList> PriceLists { get; set; }
        public DbSet<PriceListProduct> PriceListProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связи многие-ко-многим между User и PriceList
            modelBuilder.Entity<User>()
                .HasMany(u => u.AccessiblePriceLists)
                .WithMany(p => p.AllowedBuyers);

            // Настройка связи один-ко-многим между User и PriceList (владелец)
            modelBuilder.Entity<PriceList>()
                .HasOne(p => p.Seller)
                .WithMany(u => u.OwnedPriceLists)
                .HasForeignKey(p => p.SellerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка составного ключа для PriceListProduct
            modelBuilder.Entity<PriceListProduct>()
                .HasKey(plp => new { plp.PriceListId, plp.ProductId });

            // Настройка связей для PriceListProduct
            modelBuilder.Entity<PriceListProduct>()
                .HasOne(plp => plp.PriceList)
                .WithMany(pl => pl.Products)
                .HasForeignKey(plp => plp.PriceListId);

            modelBuilder.Entity<PriceListProduct>()
                .HasOne(plp => plp.Product)
                .WithMany()
                .HasForeignKey(plp => plp.ProductId);
        }
    }
} 