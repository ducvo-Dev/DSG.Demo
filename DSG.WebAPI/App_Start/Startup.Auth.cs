using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DSG.Common;
using DSG.Data;
using DSG.Model.Models;
using DSG.Service;
using DSG.WebAPI.Infrastructure;

namespace DSG.WebAPI
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit https://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(DsgDbContext.Create);

            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            app.CreatePerOwinContext<UserManager<AppUser>>(CreateManager);

            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/oauth/token"),
                Provider = new AuthorizationServerProvider(),
                AccessTokenExpireTimeSpan = TimeSpan.FromMinutes(30),
                AllowInsecureHttp = true,

            });
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());

            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                LogoutPath = new PathString("/Account/Logout"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, AppUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager, DefaultAuthenticationTypes.ApplicationCookie))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});

        }

        public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
        {
            public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
            {
                 context.Validated();
            }
    
                public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
                {
                    var allowedOrigin = context.OwinContext.Get<string>("as:clientAllowedOrigin");

                    if (allowedOrigin == null) allowedOrigin = "*";

                    context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });

                    UserManager<AppUser> userManager = context.OwinContext.GetUserManager<UserManager<AppUser>>();
                    AppUser user;
                    try
                    {
                        user = await userManager.FindAsync(context.UserName, context.Password);
                    }
                    catch
                    {
                        // Could not retrieve the user due to error.
                        context.SetError("server_error");
                        context.Rejected();
                        return;
                    }
                    if (user != null)
                    {
                        var applicationGroupService = ServiceFactory.Get<IApplicationGroupService>();
                        var listGroup = applicationGroupService.GetListGroupByUserId(user.Id);
                        if (listGroup.Any(x => x.Name == CommonConstants.Administrator))
                        {
                            ClaimsIdentity identity = await userManager.CreateIdentityAsync(
                                           user,
                                           DefaultAuthenticationTypes.ExternalBearer);
                            context.Validated(identity);
                        }
                        else
                        {
                            context.Rejected();
                            context.SetError("invalid_group", "Bạn không phải là admin");
                        }

                    }
                    else
                    {
                        context.SetError("invalid_grant", "Tài khoản hoặc mật khẩu không đúng.'");
                        context.Rejected();
                    }
                }
            }
            private static UserManager<AppUser> CreateManager(IdentityFactoryOptions<UserManager<AppUser>> options, IOwinContext context)
        {
            var userStore = new UserStore<AppUser>(context.Get<DsgDbContext>());
            var owinManager = new UserManager<AppUser>(userStore);
            return owinManager;
        }
    }
}