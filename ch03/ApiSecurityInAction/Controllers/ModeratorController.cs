using ApiSecurityInAction.Auth;
using ApiSecurityInAction.ORM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction.Controllers
{
	[Route("api/spaces")]
	[ApiController]
	public class ModeratorController : ControllerBase
	{
		private readonly NatterContext _context;

		public ModeratorController(NatterContext context)
		{
			_context = context;
		}

		[HttpDelete("{spaceId}/Messages/{messageId}")]
		[BasicAuth]
		[Authorize(Policy = "RequireDeletePermission")]
		public async Task<ActionResult> deletePost([FromRoute] int spaceId, [FromRoute] int messageId)
		{
			var message = await _context.Messages.FindAsync(spaceId, messageId);
			if (message == null) return NotFound("Message not found");
			_context.Messages.Remove(message);
			await _context.SaveChangesAsync();
			return Ok();
		}

	}
}
