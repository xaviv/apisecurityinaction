using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiSecurityInAction.ORM;
using System.Text.RegularExpressions;

namespace ApiSecurityInAction.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SpacesController : ControllerBase
	{
		private readonly NatterContext _context;

		public SpacesController(NatterContext context)
		{
			_context = context;
		}

		// POST: api/Spaces
		// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
		[HttpPost]
		public async Task<ActionResult<Space>> CreateSpace(Space space)
		{
			if (space.Name.Length > 255) return BadRequest("Space name too long");
			var match = Regex.Match(space.Owner, @"[a-zA-Z][a-zA-Z0-9]{1,29}", RegexOptions.IgnoreCase);
			if (!match.Success) return BadRequest("Invalid username");

			//transactions are not supported at inmemory database
			//using var transaction = _context.Database.BeginTransaction();

			space.SpaceId = await _context.Spaces.MaxAsync(s => s.SpaceId) + 1; // InMemory database has no sequence support
			_context.Spaces.Add(space);
			await _context.SaveChangesAsync();

			//transaction.Commit();

			return CreatedAtAction("CreateSpace", new { id = space.SpaceId }, space);
		}

		/// <summary>
		/// Read an specific message
		/// </summary>
		/// <param name="spaceId"></param>
		/// <param name="messageId"></param>
		/// <returns></returns>
		[HttpGet("{spaceId}/Messages/{messageId}")]
		public async Task<ActionResult<Message>> ReadMessage(int spaceId, int messageId)
		{
			var message = await _context.Messages.FindAsync(spaceId, messageId);
			if (message == null) return NotFound("Message not found");
			return message;
		}

		/// <summary>
		/// Find messages in last 24h
		/// </summary>
		/// <param name="spaceId"></param>
		/// <returns></returns>
		[HttpGet("{spaceId}/Messages")]
		public async Task<ActionResult<IEnumerable<Message>>> FindMessages(int spaceId)
		{
			var messages =  _context.Messages.Where(m => m.SpaceId == spaceId && m.Time >= DateTime.Now.AddDays(-1));
			return await messages.ToListAsync();
		}

		// Adding this just for testing purposes. Not found in the book.
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Space>>> GetSpaces()
		{
			return await _context.Spaces.ToListAsync();
		}
	}
}
