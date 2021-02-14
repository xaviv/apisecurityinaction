using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiSecurityInAction.ORM
{
	public class Space
	{
		/// <summary>
		/// Id of new space
		/// </summary>
		public int SpaceId { get; set; }
		/// <summary>
		/// Space name
		/// </summary>
		[MaxLength(255), Required]
		public string Name { get; set; }
		/// <summary>
		/// Owner username
		/// </summary>
		[MaxLength(30), Required]
		public string Owner { get; set; }
	}
}