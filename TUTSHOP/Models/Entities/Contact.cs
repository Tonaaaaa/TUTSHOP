using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TUTSHOP.Models.Entities
{
	public class Contact
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ContactId { get; set; }

		public string ContactName { get; set; }

		public string Phone { get; set; }
		public string Mail { get; set; }

		public string Message { get; set; }
		public DateTime? DateCreated { get; set; }
	}
}
