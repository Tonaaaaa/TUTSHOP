using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TUTSHOP.Models.Entities
{
	public class ProductAttributeValue
	{
		[Key]
		public int ProductAttributeValueId { get; set; }
		[ForeignKey("ProductAttributes")]
		public int ProductAttributeId { get; set; }
		public virtual ProductAttribute? ProductAttribute { get; set; }
		[Required]
		public string Value { get; set; }
		public virtual ICollection<VariantsAttributes>? VariantsAttributes { get; set; }
	}
}
