using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Rsk.AspNetCore.Authentication.Saml2p;

namespace Rsk.Samples.AspNetCore.Authentication.Saml2p
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "cookie";
                    options.DefaultChallengeScheme = "saml2p";
                })
                .AddCookie("cookie")
                .AddSaml2p("saml2p", options =>
                {
                    // Your entity ID (simplest to be your root URI)
                    options.EntityId = "http://localhost:5001/saml";

                    // Your private key for signing SAML Requests
                    options.ServiceProviderSigningCertificate = new X509Certificate2("Resources/testclient.pfx", "test");

                    // Your private key for decrypting SAML Assertions (OPTIONAL)
                    // options.ServiceProviderEncryptionCertificate = new X509Certificate2("Resources/idsrv3test.pfx", "idsrv3test");

                    // The URI for the Identity Provider you want to integrate with
                    options.IdentityProviderEntityId = "http://localhost:5000";

                    // The SSO endpoint in the Identity Provider to use
                    options.IdentityProviderSsoEndpoint = "http://localhost:5000/saml/sso";

                    // The public key of the Identity Provider to verify signature
                    options.IdentityProviderSigningCertificate = new X509Certificate2("Resources/idsrv3test.cer");

                    // The binding type to use when sending SAML Requests
                    options.BindingType = SamlBindingTypes.HttpRedirect;

                    // Endpoint to host your Service Provider metadata
                    options.MetadataPath = "/saml";

                    // Endpoint to receive SAML Responses
                    options.CallbackPath = "/saml/acs";

                    // Require user authentication on every SAML Request
                    options.ForceAuthentication = false;

                    // Cookie used to track SAML request ID & relay state
                    options.CorrelationCookieOptions = new CorrelationCookieOptions
                    {
                        Secure = false,
                        LifetimeInMinutes = 30
                    };

                    options.SignInScheme = "cookie";
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseAuthentication();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
