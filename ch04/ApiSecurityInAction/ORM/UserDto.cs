using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction.ORM
{
	public class UserDto
	{
		// User name
		[MaxLength(30), Required]
		public String UserName { get; set; }
		// Password
		[Required]
		public String Password { get; set; }

	}
}
