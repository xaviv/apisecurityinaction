using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiSecurityInAction.Services
{
	public class UserValidatorService : IUserValidator<IdentityUser>
	{
		public UserValidatorService()
		{
		}

		public async Task<IdentityResult> ValidateAsync(UserManager<IdentityUser> manager, IdentityUser user)
		{
			return await Task.Run(() =>
			{
				if (Validate(user.UserName)) return IdentityResult.Success;
				return IdentityResult.Failed();
			});
		}

		public bool Validate(string userName)
		{
			var match = Regex.Match(userName, @"[a-zA-Z][a-zA-Z0-9]{1,29}", RegexOptions.IgnoreCase);
			return match.Success;

		}
	}
}
