using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction.ORM
{
	public class AuditItem
	{
		public int AuditId { get; set; }
		[MaxLength(10), Required]
		public String Method { get; set; }
		[MaxLength(100), Required]
		public string Path { get; set; }
		[MaxLength(30)]
		public String UserId { get; set; }
		public int Status { get; set; }
		public DateTime AuditTime { get; set; }

	}
}
