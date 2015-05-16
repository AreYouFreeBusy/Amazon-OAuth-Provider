//  Copyright 2014 Stefan Negritoiu. See LICENSE file for more information.

using System;
using System.Globalization;
using System.Security.Claims;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Provider;
using Newtonsoft.Json.Linq;

namespace Owin.Security.Providers.Amazon
{
    /// <summary>
    /// Contains information about the login session as well as the user <see cref="System.Security.Claims.ClaimsIdentity"/>.
    /// </summary>
    public class AmazonAuthenticatedContext : BaseContext
    {
        /// <summary>
        /// Initializes a <see cref="AmazonAuthenticatedContext"/>
        /// </summary>
        /// <param name="context">The OWIN environment</param>
        /// <param name="user">The JSON-serialized user</param>
        /// <param name="accessToken">Azure AD Access token</param>
        public AmazonAuthenticatedContext(IOwinContext context, 
            JObject user, string accessToken, string expires, string refreshToken) 
            : base(context)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;

            int expiresValue;
            if (Int32.TryParse(expires, NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresValue)) 
            {
                ExpiresIn = TimeSpan.FromSeconds(expiresValue);
            }

            Id = TryGetValue(user, "user_id");
            Email = TryGetValue(user, "email");
            Name = TryGetValue(user, "name");
            PostalCode = TryGetValue(user, "postal_code");
        }

        /// <summary>
        /// Gets the Amazon OAuth access token
        /// </summary>
        public string AccessToken { get; private set; }

        /// <summary>
        /// Gets the Amazon access token expiration time
        /// </summary>
        public TimeSpan? ExpiresIn { get; private set; }

        /// <summary>
        /// Gets the Amazon OAuth refresh token
        /// </summary>
        public string RefreshToken { get; private set; }

        /// <summary>
        /// Gets the user's ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the user's email (user supplied, not verified by Amazon)
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Gets the user's full name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the user's postal code as provided to Amazon
        /// </summary>
        public string PostalCode { get; private set; }

        /// <summary>
        /// Gets the <see cref="ClaimsIdentity"/> representing the user
        /// </summary>
        public ClaimsIdentity Identity { get; set; }

        /// <summary>
        /// Gets or sets a property bag for common authentication properties
        /// </summary>
        public AuthenticationProperties Properties { get; set; }

        private static string TryGetValue(JObject user, string propertyName)
        {
            JToken value;
            return user.TryGetValue(propertyName, out value) ? value.ToString() : null;
        }
    }
}
