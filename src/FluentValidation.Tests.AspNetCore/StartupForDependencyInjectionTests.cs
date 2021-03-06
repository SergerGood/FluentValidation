namespace FluentValidation.Tests.AspNetCore {
	using System.Globalization;
	using Controllers;
	using FluentValidation.AspNetCore;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Localization;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Logging;

	public class StartupForDependencyInjectionTests {
		public StartupForDependencyInjectionTests(IHostingEnvironment env) {
			var builder = new ConfigurationBuilder();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc().AddFluentValidation(fv => {
				fv.ImplicitlyValidateChildProperties = false;
			});
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddTransient<IValidator<ParentModel>, InjectsExplicitChildValidator>();
			services.AddTransient<IValidator<ChildModel>, InjectedChildValidator>();
			services.AddTransient<IValidator<ParentModel6>, InjectsExplicitChildValidatorCollection>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
			CultureInfo cultureInfo = new CultureInfo("en-US");
			app.UseRequestLocalization(options => {
				options.DefaultRequestCulture = new RequestCulture(cultureInfo);
				options.SupportedCultures = new[] {cultureInfo};
				options.SupportedUICultures = new[] {cultureInfo};
			});

			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}