using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiSecurityInAction.ORM;
using ApiSecurityInAction.Services;
using ApiSecurityInAction.Auth;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

namespace ApiSecurityInAction.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SpacesController : ControllerBase
	{
		private readonly NatterContext _context;
		private readonly UserValidatorService _userValidatorService;

		public SpacesController(NatterContext context, UserValidatorService userValidatorService)
		{
			_context = context;
			_userValidatorService = userValidatorService;
		}

		// POST: api/Spaces
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		/// <summary>
		/// Creates a new space
		/// </summary>
		/// <param name="space"></param>
		/// <returns></returns>
		[HttpPost]
		[BasicAuth] // Anybody may create a space so you just enforce that the user is logged in
		public async Task<ActionResult<Space>> CreateSpace([FromBody] Space space)
		{
			if (space.Name.Length > 255) return BadRequest("Space name too long");

			if (!_userValidatorService.Validate(space.Owner)) return BadRequest("Invalid username");

			// chapter 3: Check owner equals identity
			if (!space.Owner.Equals(Request.HttpContext.User.Identity.Name)) return BadRequest("Owner must match authenticated user");

			//transactions are not supported at inmemory database
			//using var transaction = _context.Database.BeginTransaction();

			space.SpaceId = await _context.Spaces.MaxAsync(s => s.SpaceId) + 1; // InMemory database has no sequence support
			_context.Spaces.Add(space);

			// assign full permissions to space owner
			_context.Permissions.Add(new Permission() { SpaceId = space.SpaceId, UserId = space.Owner, Perms = "rwd" });

			await _context.SaveChangesAsync();

			//transaction.Commit();

			return CreatedAtAction("CreateSpace", new { id = space.SpaceId }, space);
		}

		/// <summary>
		/// Read an specific message
		/// </summary>
		/// <param name="spaceId">Space identificator</param>
		/// <param name="messageId">Message identificator</param>
		/// <returns></returns>
		[HttpGet("{spaceId}/Messages/{messageId}")]
		[BasicAuth]
		[Authorize(Policy = "RequireReadPermission")]
		public async Task<ActionResult<Message>> ReadMessage([FromRoute] int spaceId, [FromRoute] int messageId)
		{
			var message = await _context.Messages.FindAsync(spaceId, messageId);
			if (message == null) return NotFound("Message not found");
			return message;
		}

		/// <summary>
		/// Post a message
		/// </summary>
		/// <returns></returns>
		[HttpPost("{spaceId}/Messages")]
		[BasicAuth]
		[Authorize(Policy = "RequireWritePermission")]
		public async Task<ActionResult<Message>> PostMessage(int spaceId, String author, String text)
		{
			if (!_userValidatorService.Validate(author)) return BadRequest("Invalid username");
			if (!author.Equals(Request.HttpContext.User.Identity.Name)) return BadRequest("Author must match authenticated user");
			var message = new Message()
			{
				SpaceId = spaceId,
				MessageId = await _context.Messages.MaxAsync(m => m.MessageId) + 1,
				Author = author,
				Time = DateTime.Now,
				Text = text
			};
			_context.Messages.Add(message);
			await _context.SaveChangesAsync();
			return CreatedAtAction("PostMessage", new { spaceId = message.SpaceId, messageId = message.MessageId }, message);
		}

		/// <summary>
		/// Add a user to a space
		/// </summary>
		/// <param name="spaceId">Space id</param>
		/// <param name="perm">Permissions to be granted</param>
		/// <returns></returns>
		[HttpPost("{spaceId}/Members")]
		[BasicAuth]
		[Authorize(Policy = "RequireReadPermission")]
		public async Task<ActionResult<Permission>> AddMember([FromRoute] int spaceId, [FromBody] Permission perm)
		{
			var permRegex = new Regex(@"r?w?d?");
			var matchResult = permRegex.Match(perm.Perms);
			if (matchResult.Length != perm.Perms.Length) return BadRequest("Invalid permissions.");

			perm.SpaceId = spaceId;
			_context.Permissions.Add(perm); // Note the book sample does not check duplicates or perform further validations
			await _context.SaveChangesAsync();
			return CreatedAtAction("AddMember", new { id = spaceId }, perm);
		}

		/// <summary>
		/// Find messages in last 24h
		/// </summary>
		/// <param name="spaceId"></param>
		/// <returns></returns>
		[HttpGet("{spaceId}/Messages")]
		[BasicAuth]
		[Authorize(Policy = "RequireReadPermission")]
		public async Task<ActionResult<IEnumerable<Message>>> FindMessages([FromRoute] int spaceId)
		{
			var messages = _context.Messages.Where(m => m.SpaceId == spaceId && m.Time >= DateTime.Now.AddDays(-1));
			return await messages.ToListAsync();
		}

		// Added just for testing purposes. Not found in the book.
		/// <summary>
		/// Return a list of all possible spaces
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Space>>> GetSpaces()
		{
			return await _context.Spaces.ToListAsync();
		}
	}
}
