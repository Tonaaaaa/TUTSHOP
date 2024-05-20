using System.ComponentModel.DataAnnotations;

namespace TUTSHOP.Models.ViewModels
{
	public class ProductVariantViewModel
	{
		public int ProductVariantId { get; set; }

		[Required]
		public float Price { get; set; }

		[Required]
		public int Quantity { get; set; }

		[Required]
		public int ProductId { get; set; }

		[Required]
		public string Color { get; set; } // Thuộc tính màu cố định

		public List<AttributeViewModel> Attributes { get; set; }
	}

	public class AttributeViewModel
	{
		public int AttributeId { get; set; }
		public string Value { get; set; }
	}
}

