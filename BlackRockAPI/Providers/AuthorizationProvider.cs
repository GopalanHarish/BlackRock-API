using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Security.Claims;
using Microsoft.Owin.Security;
using BlackRockAPI.DataModel;
using BlackRockAPI.Helpers;

namespace BlackRockAPI.Providers
{
    public class AuthorizationProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            var Password = new TripleDES().BytesToStringPadded(new TripleDES().Encrypt(context.Password));

            using (BlackRockEntities entity = new BlackRockEntities())
            {
                User user = entity.Users
                                  .Where(x => (x.Email == context.UserName && x.Password == Password))
                                  .FirstOrDefault();

                if (user == null)
                {
                    context.SetError("invalid_grant", "The user name or password is incorrect.");
                    return;
                }

                ClaimsIdentity identity = new ClaimsIdentity(context.Options.AuthenticationType);                
                context.Validated(identity);
            }

        }

    }
}