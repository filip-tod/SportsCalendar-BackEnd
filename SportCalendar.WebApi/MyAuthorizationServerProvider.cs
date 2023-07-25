using System;
using Microsoft.Owin.Security.OAuth;
using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.Repository;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security;

namespace SportCalendar.WebApi
{

    public class MyAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            bool isValidUsername = CredentialsValidation.ValidateUsername(context.UserName);
            bool isValidPassword = CredentialsValidation.ValidatePassword(context.Password);

            if (isValidUsername && isValidPassword)
            {
                AuthUser user = await AuthRepository.ValidateUserAsync(context.UserName, context.Password);

                if (user == null)
                {
                    context.SetError("invalid_grant", "Provided username and password is incorrect");
                    return;
                }

                var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                identity.AddClaim(new Claim("Id", user.Id.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Name, user.Username));
                identity.AddClaim(new Claim(ClaimTypes.Role, user.Access));
                identity.AddClaim(new Claim("Email", user.Email));

                AuthenticationProperties properties = new AuthenticationProperties(new Dictionary<string, string>
                {
                    { "Id", user.Id.ToString() },
                    { "Username", user.Username },
                    { "Role", user.Access }
                });

                context.Validated(new AuthenticationTicket(identity, properties));
                return;
            }
            context.SetError("Invalid Username or Password");
            return;


        }
    }
}