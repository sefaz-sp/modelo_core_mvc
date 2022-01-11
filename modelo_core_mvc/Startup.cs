using Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using modelo_core_mvc.HttpClients;

namespace modelo_core_mvc
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
            services.AddControllersWithViews();

            IdentityConfig identityConfig = new IdentityConfig(Configuration);

            if (Configuration["identity:type"] == "openid")
            {
                services.AddAuthentication(identityConfig.AuthenticationOptions)
                        .AddOpenIdConnect(identityConfig.OpenIdConnectOptions)
                        .AddCookie();
            }
            else
            if (Configuration["identity:type"] == "wsfed")
            {
                services.AddAuthentication(identityConfig.AuthenticationOptions)
                        .AddWsFederation(identityConfig.WSFederationOptions)
                        .AddCookie();
            }
            else
            {
                services.AddAuthentication(identityConfig.AuthenticationOptions)
                        .AddWsFederation(identityConfig.WSFederationOptions)
                        .AddCookie("Cookies", identityConfig.CookieAuthenticationOptions);
            }

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<Usuario>();

            services.AddHttpClient<ProjetosApiClient>();
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_CONNECTIONSTRING"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (!string.IsNullOrEmpty(Configuration["dadosdeploy:nomeappk8s"]))
            {
                //No servidor kubernetes com aplicações compartilhadas, a pasta base da rota deve ser informada (nomeappk8s)
                app.Use((context, next) =>
                {
                    context.Request.PathBase = "/" + Configuration["dadosdeploy:nomeappk8s"];
                    return next();
                });
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
