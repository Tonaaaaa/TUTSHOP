using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TUTSHOP.Models.Entities
{
	public class ProductAttribute
	{
		[Key]
		public int ProductAttributeId { get; set; }
		public string? ProductAttributeName { get; set; }
		public virtual ICollection<ProductAttributeValue>? ProductAttributeValues { get; set; }
	}
}

