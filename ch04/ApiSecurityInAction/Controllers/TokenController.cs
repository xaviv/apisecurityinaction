using ApiSecurityInAction.Auth;
using ApiSecurityInAction.ORM;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiSecurityInAction.Controllers
{
	[Route("api/sessions")]
	[ApiController]
	public class TokenController : ControllerBase
	{
		private readonly ITokenStore _store;

		public TokenController(ITokenStore store)
		{
			_store = store;
		}

		/// <summary>
		/// Entry point to create a token
		/// </summary>
		/// <returns></returns>
		[BasicAuth]
		[HttpPost()]
		public async Task<ActionResult<TokenDto>> Login()
		{
			var expiry = DateTimeOffset.Now.AddMinutes(10);

			var token = new Token(expiry, Request.HttpContext.User.Identity.Name);
			var tokenResult = _store.Create(Request, token);

			return CreatedAtAction("Login", tokenResult);
		}
	}
}
