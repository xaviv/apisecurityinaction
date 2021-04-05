using ApiSecurityInAction.ORM;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction.Auth
{
	public interface ITokenStore
	{
		TokenDto Create(HttpRequest request, Token token);
		Token Read(HttpRequest request, String tokenId);
	}

	public class CookieTokenStore : ITokenStore
	{
		public TokenDto Create(HttpRequest request, Token token)
		{
			request.HttpContext.Response.Cookies.Append("JSESSIONID", request.HttpContext.Session.Id);
			return new TokenDto() { Token = request.HttpContext.Session.Id };
		}

		public Token Read(HttpRequest request, string tokenId)
		{
			var session = request.HttpContext.Session;
			if (!session.IsAvailable) return null;
			var token = new Token(DateTimeOffset.FromUnixTimeSeconds((long)session.GetInt32("expiry")), session.GetString("username"));
			foreach (var k in session.Keys) token.Attributes.Add(k, session.GetString(k));
			return token;
		}
	}
}
