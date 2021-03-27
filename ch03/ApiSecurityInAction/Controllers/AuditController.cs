using ApiSecurityInAction.ORM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction.Controllers
{
	[Route("api/logs")]
	[ApiController]
	public class AuditController : ControllerBase
	{
		private readonly NatterContext _context;

		public AuditController(NatterContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Dumps audit log for the last hour
		/// </summary>
		/// <returns></returns>
		[HttpGet()]
		public async Task<ActionResult<IEnumerable<AuditItem>>> ReadAuditLog()
		{
			var entries = _context.AuditLog.Where(i => i.AuditTime >= DateTime.Now.AddHours(-1)); //.TakeLast(20);
			return await entries.ToListAsync();
		}

	}
}
