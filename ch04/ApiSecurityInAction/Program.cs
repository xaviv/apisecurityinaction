using ApiSecurityInAction.ORM;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ApiSecurityInAction
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().PrepareDatabase().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}
