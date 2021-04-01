using ApiSecurityInAction.ORM;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ApiSecurityInAction.Services
{

	internal class UserStoreService : IUserStore<IdentityUser>, IUserPasswordStore<IdentityUser>, IRoleStore<IdentityRole>
	{
		private readonly NatterContext _context;

		public UserStoreService(NatterContext context)
		{
			_context = context;
		}

		#region IUserStore
		public async Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			var u = await ((IUserStore<IdentityUser>)this).FindByNameAsync(user.UserName, cancellationToken);
			return u != null ? IdentityResult.Success : IdentityResult.Failed();
		}

		public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			_context.Dispose();
		}

		Task<IdentityUser> IUserStore<IdentityUser>.FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			return _context.Users.FindAsync(userId).AsTask();
		}

		Task<IdentityUser> IUserStore<IdentityUser>.FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			return _context.Users.Where(u => u.UserName.Equals(normalizedUserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync(cancellationToken);
		}

		public Task<string> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public async Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			var u = await ((IUserStore<IdentityUser>)this).FindByNameAsync(user.UserName, cancellationToken);
			return u?.Id ?? string.Empty;
		}

		public async Task<string> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			var u = await ((IUserStore<IdentityUser>)this).FindByNameAsync(user.UserName, cancellationToken);
			return u?.UserName ?? string.Empty;
		}

		public async Task SetNormalizedUserNameAsync(IdentityUser user, string normalizedName, CancellationToken cancellationToken)
		{
			var u = await ((IUserStore<IdentityUser>)this).FindByNameAsync(user.UserName, cancellationToken);
			if (u == null)
			{
				user.NormalizedUserName = normalizedName;
				_context.Users.Add(user);
			}
			else
			{
				if (u.NormalizedUserName == normalizedName) return;
				u.NormalizedUserName = normalizedName;
				_context.Users.Update(u);
			}
			await _context.SaveChangesAsync();
		}

		public async Task SetUserNameAsync(IdentityUser user, string userName, CancellationToken cancellationToken)
		{
			var u = await ((IUserStore<IdentityUser>)this).FindByNameAsync(user.UserName, cancellationToken);
			if (u == null)
			{
				user.UserName = userName;
				_context.Users.Add(user);
			}
			else
			{
				if (u.UserName == userName) return;
				u.UserName = userName;
				_context.Users.Update(u);
			}
			await _context.SaveChangesAsync();
		}

		public async Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			var r = await _context.SaveChangesAsync(cancellationToken) > 0;
			if (r) return IdentityResult.Success;
			return IdentityResult.Failed();
		}
		#endregion

		#region IPasswordStore
		public async Task<string> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			var u = await ((IUserStore<IdentityUser>)this).FindByNameAsync(user.UserName, cancellationToken);
			return u?.PasswordHash ?? string.Empty;
		}

		public async Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
		{
			var u = await ((IUserStore<IdentityUser>)this).FindByNameAsync(user.UserName, cancellationToken);
			return !string.IsNullOrEmpty(u.PasswordHash);
		}

		public async Task SetPasswordHashAsync(IdentityUser user, string passwordHash, CancellationToken cancellationToken)
		{
			var u = await ((IUserStore<IdentityUser>)this).FindByNameAsync(user.UserName, cancellationToken);
			if (u == null)
			{
				user.PasswordHash = passwordHash;
				_context.Users.Add(user);
			}
			else
			{
				u.PasswordHash = passwordHash;
				_context.Users.Update(u);
			}
			await _context.SaveChangesAsync();
		}
		#endregion

		#region IRoleStore
		public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<IdentityRole> IRoleStore<IdentityRole>.FindByIdAsync(string roleId, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		Task<IdentityRole> IRoleStore<IdentityRole>.FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
		#endregion

	}
}