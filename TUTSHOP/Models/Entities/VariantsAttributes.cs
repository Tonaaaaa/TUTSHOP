

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TUTSHOP.Models.Entities
{
	public class VariantsAttributes
	{
        [Key]
        public int Id { get; set; }
        [ForeignKey("ProductVariants")]
        public int ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        [ForeignKey("ProductAttributeValues")]
        public int ProductAttributeValueId { get; set; }
        public ProductAttributeValue? ProductAttributeValue { get; set; }
    }
}
