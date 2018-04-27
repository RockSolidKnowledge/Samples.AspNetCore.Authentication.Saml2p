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
                    options.ServiceProviderOptions = new SpOptions
                    {
                        // Your entity ID (simplest to be your root URI)
                        EntityId = "http://localhost:5001/saml",

                        // Your private key for signing SAML Requests
                        SignAuthenticationRequests = true,
                        SigningCertificate = new X509Certificate2("Resources/testclient.pfx", "test"),

                        // Your private key for decrypting SAML Assertions (OPTIONAL)
                        // EncryptionCertificate = new X509Certificate2("Resources/idsrv3test.pfx", "idsrv3test"),

                        // Endpoint to host your SP metadata
                        MetadataPath = "/saml"
                    };

                    options.IdentityProviderOptions = new IdpOptions
                    {
                        // The URI for the Identity Provider you want to integrate with
                        EntityId = "http://localhost:5000",

                        // The SSO endpoint in the Identity Provider to use
                        SsoEndpoint = "http://localhost:5000/saml/sso",

                        // The public key of the Identity Provider to verify signature
                        SigningCertificate = new X509Certificate2("Resources/idsrv3test.cer"),

                        // The binding type to use when sending SAML Requests
                        BindingType = SamlBindingTypes.HttpRedirect
                    };
                    
                    // Endpoint to receive SAML Responses
                    options.CallbackPath = "/saml/acs";

                    // Require user authentication on every SAML Request
                    options.ForceAuthentication = false;

                    // local auth type to sign in using
                    options.SignInScheme = "cookie";

                    // license from sales@rocksolidknowledge.com
                    options.Licensee = "";
                    options.LicenseKey = "";
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
