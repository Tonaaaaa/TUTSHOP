using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TUTSHOP.Models.Entities
{
    public partial class Category
    {
        public int CategoryId { get; set; }
        [DisplayName("Tên danh mục")]
        public string? CategoryName { get; set; }
        [DisplayName("Hiển thị")]
        public bool Display { get; set; }
        [DisplayName("Mã giảm giá")]
        public int DiscountId { get; set; }
        /*public virtual ICollection<Product>? Products { get; set; }*/
		public List<Product>? Products { get; set; }
	}
}
