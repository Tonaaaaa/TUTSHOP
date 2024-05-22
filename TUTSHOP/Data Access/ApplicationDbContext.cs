using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TUTSHOP.Models.Entities;
namespace TUTSHOP.Data_Access
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }

        public DbSet<ProductVariant> ProductVariants { get; set; }
		public DbSet<ProductAttribute> ProductAttributes { get; set; }
		public DbSet<ProductAttributeValue> ProductAttributeValues { get; set; }
		public DbSet<VariantsAttributes> VariantsAttributes { get; set; }


		public DbSet<Contact> Contacts { get; set; }

		public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
