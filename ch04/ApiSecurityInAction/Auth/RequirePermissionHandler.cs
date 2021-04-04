using System;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiSecurityInAction.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ApiSecurityInAction.Auth
{
	public class RequirePermissionHandler : AuthorizationHandler<RequirePermission>
	{
		// Note this must be defined as a scoped service instead of singleton. Otherwise, context cannot be injected
		private readonly NatterContext _db;
		public RequirePermissionHandler(NatterContext db)
		{
			_db = db;
		}

		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RequirePermission requirement)
		{
			if (context.Resource is DefaultHttpContext ctx)
			{
				if (ctx.Request.RouteValues.ContainsKey("spaceId"))
				{
					if (int.TryParse(ctx.Request.RouteValues["spaceId"] as String, out int spaceId))
					{
						var permissions = _db.Permissions.Find(spaceId, context.User.Identity.Name);
						if (permissions != null)
						{
							if (permissions.Perms.Contains((char)requirement.Permission))
							{
								context.Succeed(requirement);
								return Task.CompletedTask;
							}
						}
					}
				}
			}
			context.Fail();
			return Task.CompletedTask;
		}
	}
}