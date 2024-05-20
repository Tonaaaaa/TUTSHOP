using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TUTSHOP.Models.Entities
{
    public class Product
    {
        [Display(Name ="ID")]
        public int ProductId { get; set; }
        [Required]
        [Display(Name ="Tên")]
        public string? ProductName { get; set; }
        [Display(Name = "Hiển thị")]
        public bool Display { get; set; }
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }
        [Display(Name = "Hình ảnh chính")]
        public string? ImageUrl { get; set; }
        [Required]
        [Display(Name = "Giá hiển thị")]
        public float Price { get; set; }
        [Required]
        [Display(Name = "Giá cũ")]
        public float OldPrice { get; set; }
        [Display(Name = "Bán chạy")]
        public bool HotSeller { get; set; }
        [Display(Name = "Danh mục")]
        [ForeignKey("Categories")]
        public int CategoryId { get; set; }
        [Display(Name = "Giảm giá")]
        public int? DiscountId { get; set; }
        [Display(Name = "Ngày tạo")]
        public DateTime? DateCreated { get; set; }
        [Display(Name = "Ngày sửa")]
        public DateTime? DateModified { get; set; }
        [Display(Name = "Sản phẩm đại diện")]
		public bool IsFeatured { get; set; }
		public List<ProductImage>? Images { get; set; }
        [Display(Name = "Danh mục")]
        public Category? Category { get; set; }

		public virtual ICollection<ProductVariant>? ProductVariants { get; set; }


	}

}
