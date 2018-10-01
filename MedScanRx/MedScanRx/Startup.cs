using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using MedScanRx.BLL;
using MedScanRx.BLL.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MedScanRx.ScheduledServices;

namespace MedScanRx
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

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(options =>
					{
						options.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuer = true,
							ValidateAudience = true,
							ValidateLifetime = true,
							ValidateIssuerSigningKey = true,
							ValidIssuer = "medscanrx.com",
							ValidAudience = "medscanrx.com",
							IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecurityKey"]))
						};
					});

			services.AddAuthorization(options =>
			{
				options.AddPolicy("Admin", policy => policy.RequireClaim("AdminClaimTest"));
			});

			services.AddCors();

			services.AddSingleton(Configuration);
			services.AddSingleton<DeactivatePastAlerts>();
			services.AddSingleton<CloudMessaging>();
			services.AddSingleton<IHostedService, ScheduledService>();

			services.AddMvc();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			app.UseCors(builder => builder.WithOrigins(new string[] { "http://localhost:3000",
								"http://medscanrx.s3-website-us-west-1.amazonaws.com" }
			).AllowAnyMethod().AllowAnyHeader());

			app.UseAuthentication();

			app.UseMvc();
		}
	}
}
