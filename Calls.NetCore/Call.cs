using System.ComponentModel.DataAnnotations.Schema;
using Dapper.Contrib.Extensions;

namespace Calls
{
	[System.ComponentModel.DataAnnotations.Schema.Table("Calls")]
	public class Call
	{
		[System.ComponentModel.DataAnnotations.Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		//[Required]
		[NotMapped]
		[Write(false)]
		public PhoneNumber Caller { get; set; }
        public int? CallerId { get; set; }

		//[Required]
        [NotMapped]
		[Write(false)]
		public PhoneNumber Receiver { get; set; }
        public int? ReceiverId { get; set; }

		public long Duration { get; set; }
	}
}
