//  Copyright 2014 Stefan Negritoiu. See LICENSE file for more information.

using System;
using System.Globalization;
using System.Net.Http;
using Microsoft.Owin;
using Microsoft.Owin.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataHandler;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Infrastructure;

namespace Owin.Security.Providers.Amazon
{
    public class AmazonAuthenticationMiddleware : AuthenticationMiddleware<AmazonAuthenticationOptions>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;

        public AmazonAuthenticationMiddleware(OwinMiddleware next, IAppBuilder app, AmazonAuthenticationOptions options)
            : base(next, options)
        {
            if (String.IsNullOrWhiteSpace(Options.ClientId)) 
            {
                throw new ArgumentException("ClientId option must be provided.");
            }
            if (String.IsNullOrWhiteSpace(Options.ClientSecret)) 
            {
                throw new ArgumentException("ClientSecret option must be provided.");
            }
            _logger = app.CreateLogger<AmazonAuthenticationMiddleware>();

            if (Options.Provider == null) 
            {
                Options.Provider = new AmazonAuthenticationProvider();
            }
            
            if (Options.StateDataFormat == null)
            {
                IDataProtector dataProtector = app.CreateDataProtector(
                    typeof(AmazonAuthenticationMiddleware).FullName,
                    Options.AuthenticationType, "v1");
                Options.StateDataFormat = new PropertiesDataFormat(dataProtector);
            }

            if (String.IsNullOrEmpty(Options.SignInAsAuthenticationType)) 
            {
                Options.SignInAsAuthenticationType = app.GetDefaultSignInAsAuthenticationType();
            }

            _httpClient = new HttpClient(ResolveHttpMessageHandler(Options))
            {
                Timeout = Options.BackchannelTimeout,
                MaxResponseContentBufferSize = 1024 * 1024 * 10 // 10 MB
            };
        }

        /// <summary>
        ///     Provides the <see cref="T:Microsoft.Owin.Security.Infrastructure.AuthenticationHandler" /> object for processing
        ///     authentication-related requests.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:Microsoft.Owin.Security.Infrastructure.AuthenticationHandler" /> configured with the
        ///     <see cref="T:Owin.Security.Providers.Amazon.AmazonAuthenticationOptions" /> supplied to the constructor.
        /// </returns>
        protected override AuthenticationHandler<AmazonAuthenticationOptions> CreateHandler()
        {
            return new AmazonAuthenticationHandler(_httpClient, _logger);
        }

        private HttpMessageHandler ResolveHttpMessageHandler(AmazonAuthenticationOptions options)
        {
            HttpMessageHandler handler = options.BackchannelHttpHandler ?? new WebRequestHandler();

            // If they provided a validator, apply it or fail.
            if (options.BackchannelCertificateValidator != null)
            {
                // Set the cert validate callback
                var webRequestHandler = handler as WebRequestHandler;
                if (webRequestHandler == null)
                {
                    throw new InvalidOperationException("Vaidator Handler Mismatch");
                }
                webRequestHandler.ServerCertificateValidationCallback = options.BackchannelCertificateValidator.Validate;
            }

            return handler;
        }
    }
}