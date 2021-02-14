using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction.Auth
{
	public class BasicAuthAttribute : AuthorizeAttribute
	{
		public BasicAuthAttribute()
		{
			Policy = "BasicAuthentication";
		}
	}
}
