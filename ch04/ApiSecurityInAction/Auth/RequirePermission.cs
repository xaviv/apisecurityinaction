using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction.Auth
{
	public enum Permissions
	{
		Write = 'w',
		Read = 'r',
		Delete = 'd'
	}

	public class RequirePermission: IAuthorizationRequirement
	{
		public readonly Permissions Permission;

		public RequirePermission(Permissions Perm)
		{
			Permission = Perm;
		}
	}
}
