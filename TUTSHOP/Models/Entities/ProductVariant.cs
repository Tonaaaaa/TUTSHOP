using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace TUTSHOP.Models.Entities
{
	public class ProductVariant
	{
		[Key]
		public int ProductVariantId { get; set; }
		[Required]
		public float Price { get; set; }
		[Required]
		public int Quantity { get; set; }
		[ForeignKey("Products")]
		public int ProductId { get; set; }
		public virtual Product? Product { get; set; }
		[Required]
		public string Color { get; set; }
		public virtual ICollection<VariantsAttributes>? VariantsAttributes { get; set; }
		
	}
}
