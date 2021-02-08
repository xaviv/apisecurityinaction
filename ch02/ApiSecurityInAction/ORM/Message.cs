using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction.ORM
{
	public class Message
	{
		public int SpaceId { get; set; }
		public int MessageId { get; set; }
		[MaxLength(30), Required]
		public string Author { get; set; }
		[Required]
		public DateTime Time { get; set; }
		[MaxLength(1024), Required]
		public string Text { get; set; }
	}
}
