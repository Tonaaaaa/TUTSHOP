using System.ComponentModel.DataAnnotations;

namespace TUTSHOP.Models.Entities
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int SizeId { get; set; }
        

        public string? ImageUrl { get; set; }
        public List<ProductImage>? Images { get; set; }
    }
}
