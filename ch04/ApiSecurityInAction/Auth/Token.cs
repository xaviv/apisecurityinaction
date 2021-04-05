using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction.Auth
{
	public sealed class Token
	{
		public readonly DateTimeOffset Expiry;
		public readonly String UserName;
		public readonly Dictionary<String, String> Attributes = new Dictionary<string, string>();

		public Token(DateTimeOffset expiry, String username)
		{
			this.Expiry = expiry;
			this.UserName = username;
		}
	}
}
