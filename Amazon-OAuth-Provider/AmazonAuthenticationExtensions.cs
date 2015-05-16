//  Copyright 2014 Stefan Negritoiu. See LICENSE file for more information.

using System;

namespace Owin.Security.Providers.Amazon
{
    public static class AmazonAuthenticationExtensions
    {
        public static IAppBuilder UseAmazonAuthentication(this IAppBuilder app, AmazonAuthenticationOptions options)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (options == null)
                throw new ArgumentNullException("options");

            app.Use(typeof(AmazonAuthenticationMiddleware), app, options);

            return app;
        }

        public static IAppBuilder UseAmazonAuthentication(this IAppBuilder app, string clientId, string clientSecret)
        {
            return app.UseAmazonAuthentication(new AmazonAuthenticationOptions
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            });
        }
    }
}