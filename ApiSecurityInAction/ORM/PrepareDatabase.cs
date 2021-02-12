using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction.ORM
{
	public static class PrepareDatabaseManager
	{
		public static IHost PrepareDatabase(this IHost host)
		{
			using (var scope = host.Services.CreateScope())
			{
				using (var context = scope.ServiceProvider.GetRequiredService<NatterContext>())
				{
					var space1 = new Space() { SpaceId = 1, Name = "Space1", Owner = "TestOwner" };
					context.Spaces.Add(space1);
					var message1 = new Message() { SpaceId = 1, MessageId = 1, Author = "Author1", Text = "Message text test", Time = DateTime.Now };
					context.Messages.Add(message1);
					context.SaveChanges();
				}
			}
			return host;
		}
	}
}
