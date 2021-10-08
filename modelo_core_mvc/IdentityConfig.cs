using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Identity
{
    public class IdentityConfig
    {
        private static IConfiguration configuration;

        public static Action<WsFederationOptions> WSFederationOptions { get; private set; }
        public static Action<CookieAuthenticationOptions> CookieAuthenticationOptions { get; private set; }
        public static Action<Microsoft.AspNetCore.Authentication.AuthenticationOptions> AuthenticationOptions { get; private set; }
        public static Action<OpenIdConnectOptions> OpenIdConnectOptions { get; private set; }

        public IdentityConfig()
        {
        }

        public static void RegistrarOpcoes(IConfiguration Configuration)
        {
            configuration = Configuration;
            AuthenticationOptions = options =>
            {
                if (Configuration["identity:type"] == "openid")
                {
                    //options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    //options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }
                else
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                };
            };

            WSFederationOptions = options =>
            {
                options.Wtrealm = configuration["identity:realm"];
                options.Wreply = configuration["identity:reply"];
                options.MetadataAddress = configuration["identity:metadataaddress"];

                //if (configuration["identity:tokenws"] != "")
                //{
                //    options.Events.OnRedirectToIdentityProvider = OnRedirectToIdentityProvider;
                //    options.Events.OnSecurityTokenReceived = OnSecurityTokenReceived;
                //}

                options.TokenValidationParameters = new TokenValidationParameters { SaveSigninToken = true };
                options.CorrelationCookie = new CookieBuilder
                {
                    Name = ".Correlation.",
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                    SecurePolicy = CookieSecurePolicy.Always,
                    Expiration = new TimeSpan(0, 0, 15, 0),
                    MaxAge = new TimeSpan(0, 0, 15, 0),
                };
            };

            OpenIdConnectOptions = options =>
            {
                options.ClientId = "identity:clientid";
                options.Authority = "identity:authority";
                options.SignedOutRedirectUri = "identity:realm";
                options.RequireHttpsMetadata = false;

                options.Events = new OpenIdConnectEvents
                {
                    OnRemoteFailure = OnAuthenticationFailed,
                };

            };

            CookieAuthenticationOptions = options =>
            {
                options.Cookie = new CookieBuilder
                {
                    Name = "FedAuth",
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.None,
                    SecurePolicy = CookieSecurePolicy.Always,
                };
                options.ExpireTimeSpan = new TimeSpan(0, 0, int.Parse(configuration["identity:timeout"]), 0);
                options.SlidingExpiration = false;
            };
        }

        private static async Task<Task<int>> OnSecurityTokenReceived(SecurityTokenReceivedContext arg)
        {
            TokenWSClient tokenWS = new TokenWSClient(TokenWSClient.EndpointConfiguration.TokenWS, configuration["identity:tokenws"]);
            try
            {
                if (await tokenWS.IsTokenValidAsync(arg.ProtocolMessage.GetToken(), configuration["identity:realm"], "00031C33"))
                {
                    return Task.FromResult(0);
                }
            }
            finally
            {
                #region Close_or_Abort
                if (tokenWS != null)
                {
                    try
                    {
                        await tokenWS.CloseAsync();
                    }
                    catch (Exception)
                    {
                        tokenWS.Abort();
                    }
                }
                #endregion
            }
            throw new Exception($"Token recebido é inválido ou não foi emitido para '{configuration["identity:realm"]}'.");
        }

        //public static Task OnRedirectToIdentityProvider(RedirectContext arg)
        //{
        //    arg.ProtocolMessage.Wauth = configuration["identity:Wauth"];
        //    arg.ProtocolMessage.Wfresh = configuration["identity:timeout"];
        //    arg.ProtocolMessage.Parameters.Add("ClaimSets", "80000000");
        //    arg.ProtocolMessage.Parameters.Add("TipoLogin", "00031C33");
        //    arg.ProtocolMessage.Parameters.Add("AutoLogin", "0");
        //    arg.ProtocolMessage.Parameters.Add("Layout", "2");
        //    return Task.FromResult(0);
        //}

        public static Task OnAuthenticationFailed(RemoteFailureContext context)
        {
            context.HandleResponse();
            context.Response.Redirect("/?errormessage=" + context.Failure.Message);
            return Task.FromResult(0);
        }
    }
}
