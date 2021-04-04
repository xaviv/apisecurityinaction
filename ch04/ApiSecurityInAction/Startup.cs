using ApiSecurityInAction.Audit;
using ApiSecurityInAction.Auth;
using ApiSecurityInAction.ORM;
using ApiSecurityInAction.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiSecurityInAction
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "ApiSecurityInAction", Version = "v1" });
				// Set the comments path for the Swagger JSON and UI.
				var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});
			services.AddDbContext<NatterContext>(options => options.UseInMemoryDatabase("NatterDB"));
			services.AddControllers();

			// The book uses Scrypt to manage passwords. Asp.net core provides Identity services instead
			services.Configure<IdentityOptions>(options =>
			{
				options.Password.RequiredLength = 8; // To match book settings
			});

			// As we are not using EF framework, custom user and role stores are needed
			services.AddIdentity<IdentityUser, IdentityRole>()
				.AddUserStore<UserStoreService>()
				.AddUserManager<UserManager<IdentityUser>>()
				.AddUserValidator<UserValidatorService>()
				.AddRoleStore<UserStoreService>()
				.AddRoleManager<RoleManager<IdentityRole>>()
				.AddDefaultTokenProviders();

			services.AddTransient<UserValidatorService>();

			services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", options => { });

			services.AddAuthorization(options =>
			{
				options.AddPolicy("BasicAuthentication", new AuthorizationPolicyBuilder("BasicAuthentication").RequireAuthenticatedUser().Build());
				options.AddPolicy("RequireWritePermission", policy => policy.Requirements.Add(new RequirePermission(Permissions.Write)));
				options.AddPolicy("RequireReadPermission", policy => policy.Requirements.Add(new RequirePermission(Permissions.Read)));
				options.AddPolicy("RequireDeletePermission", policy => policy.Requirements.Add(new RequirePermission(Permissions.Delete)));
			});

			services.AddScoped<IAuthorizationHandler, RequirePermissionHandler>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiSecurityInAction v1"));
			}

			app.UseHttpsRedirection();

			app.UseStaticFiles(); // Must be placed *before* the security headers

			// Add explicit security headers
			app.Use(async (context, next) =>
			{
				context.Response.Headers.Add("X-Xss-Protection", "0"); // 1; mode = block
				context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
				context.Response.Headers.Add("X-Frame-Options", "DENY");
				context.Response.Headers.Add("Cache-Control", "no-store");
				context.Response.Headers.Add("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none'; sandbox");
				await next();
			});

			app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			// Custom middleware to provide audit trail
			app.UseMiddleware<AuditMiddleware>();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

		}
	}
}
