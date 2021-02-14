using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiSecurityInAction.Auth
{
	public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly SignInManager<IdentityUser> _signInManager;
		public BasicAuthenticationHandler(
			IOptionsMonitor<AuthenticationSchemeOptions> options,
			ILoggerFactory logger,
			UrlEncoder encoder,
			ISystemClock clock,
			UserManager<IdentityUser> userManager,
			SignInManager<IdentityUser> signInManager
		) : base(options, logger, encoder, clock)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			Response.Headers.Add("WWW-Authenticate", "Basic");

			if (!Request.Headers.ContainsKey("Authorization"))
			{
				return AuthenticateResult.Fail("Authorization header missing.");
			}

			// Get authorization key
			var authorizationHeader = Request.Headers["Authorization"].ToString();
			var authHeaderRegex = new Regex(@"Basic (.*)");

			if (!authHeaderRegex.IsMatch(authorizationHeader))
			{
				return AuthenticateResult.Fail("Authorization code not formatted properly.");
			}

			var authBase64 = Encoding.UTF8.GetString(Convert.FromBase64String(authHeaderRegex.Replace(authorizationHeader, "$1")));
			var authSplit = authBase64.Split(Convert.ToChar(":"), 2);
			var authUsername = authSplit[0];
			var authPassword = authSplit.Length > 1 ? authSplit[1] : throw new Exception("Unable to get password");

			var user = await _userManager.FindByNameAsync(authUsername);
			if (user == null) return AuthenticateResult.Fail("The username or password is not correct.");
			if (! await _userManager.CheckPasswordAsync(user, authPassword))
			{
				return AuthenticateResult.Fail("The username or password is not correct.");
			}

			var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);

			return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
		}
	}
}
