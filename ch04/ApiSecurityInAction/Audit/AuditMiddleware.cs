using ApiSecurityInAction.ORM;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction.Audit
{
	public class AuditMiddleware
	{
		private readonly RequestDelegate _next;

		public AuditMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			// Gather db service from request services. Note no need to dispose it
			NatterContext db = (NatterContext)context.RequestServices.GetService(typeof(NatterContext));
			// Before calling the endpoint
			await BeforeInvoke(context, db);
			// Continue the execution of middleware
			await _next.Invoke(context); 
			// After calling the endpoint
			await AfterInvoke(context, db);
		}

		private async Task BeforeInvoke(HttpContext context, NatterContext db)
		{
			int auditId = await db.AuditLog.MaxAsync(a => a.AuditId) + 1;
			context.Items.Add("auditId", auditId);
			var entry = new AuditItem()
			{
				AuditId = auditId,
				Method = context.Request.Method,
				Path = context.Request.Path,
				UserId = context.Request.HttpContext.User.Identity?.Name,
				AuditTime = DateTime.Now
			};
			db.AuditLog.Add(entry);
			await db.SaveChangesAsync();
		}

		private async Task AfterInvoke(HttpContext context, NatterContext db)
		{
			int auditId = (int)context.Items["auditId"];
			var entry = new AuditItem()
			{
				AuditId = auditId,
				Method = context.Request.Method,
				Path = context.Request.Path,
				Status = context.Response.StatusCode,
				UserId = context.Request.HttpContext.User.Identity?.Name,
				AuditTime = DateTime.Now
			};
			db.AuditLog.Add(entry);
			await db.SaveChangesAsync();
		}

	}
}
