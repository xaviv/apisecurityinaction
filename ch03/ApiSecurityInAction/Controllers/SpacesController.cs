﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ApiSecurityInAction.ORM;
using System.Text.RegularExpressions;
using ApiSecurityInAction.Services;
using ApiSecurityInAction.Auth;

namespace ApiSecurityInAction.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SpacesController : ControllerBase
	{
		private readonly NatterContext _context;
		private readonly UserValidatorService _userValidatorService;

		public SpacesController(NatterContext context, UserValidatorService userValidatorService )
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
		[BasicAuth]
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
		public async Task<ActionResult<Message>> ReadMessage([FromRoute] int spaceId, [FromRoute] int messageId)
		{
			var message = await _context.Messages.FindAsync(spaceId, messageId);
			if (message == null) return NotFound("Message not found");
			return message;
		}

		// TODO: Missing post create Message & check author with authenticated user name

		/// <summary>
		/// Find messages in last 24h
		/// </summary>
		/// <param name="spaceId"></param>
		/// <returns></returns>
		[HttpGet("{spaceId}/Messages")]
		public async Task<ActionResult<IEnumerable<Message>>> FindMessages([FromRoute]int spaceId)
		{
			var messages =  _context.Messages.Where(m => m.SpaceId == spaceId && m.Time >= DateTime.Now.AddDays(-1));
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