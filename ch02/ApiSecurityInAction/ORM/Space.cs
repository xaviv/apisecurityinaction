using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiSecurityInAction.ORM
{
	public class Space
	{
		public int SpaceId { get; set; }
		[MaxLength(255), Required]
		public string Name { get; set; }
		[MaxLength(30), Required]
		public string Owner { get; set; }
	}
}