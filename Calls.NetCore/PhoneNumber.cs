using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Calls
{
	[Table("PhoneNumbers")]
	public class PhoneNumber
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[MaxLength(400)]
		public string Number { get; set; }
	}
}
