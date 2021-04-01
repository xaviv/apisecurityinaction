using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiSecurityInAction.ORM
{
	public class Permission
	{
		[Required]
		public int SpaceId { get; set; }
		[MaxLength(30), Required]
		public string UserId { get; set; }
		/// <summary>
		/// Permissions allowed: r (read), w (write) and d (delete)
		/// </summary>
		[MaxLength(3), Required]
		public string Perms { get; set; }
	}
}