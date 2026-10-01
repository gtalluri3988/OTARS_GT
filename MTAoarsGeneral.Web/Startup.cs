using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using MTAoarsGeneral.Services;
using MTAoarsGeneral.Services.Accounts;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.Interfaces;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Mvc;
using Owin;
using System;
using System.IdentityModel.Tokens;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using MTAoarsGeneral.Utilities.Managers;

namespace MTAoarsGeneral.Web
{
    public class Startup
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void Configuration(IAppBuilder app)
        {
            logger.Info("Startup run.");

            //AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            //var authorityCertRawData = GetCertificatePublicKey(AppSettings.Authority); /* use when not in localhost */
            var authorityCertRawData = Convert.FromBase64String(AppSettings.IssuerSigningCert);
            var certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(authorityCertRawData);

            //var discoveryEndpoint = AppSettings.SSOAuthBaseUri + "/core/.well-known/openid-configuration";
            //var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(discoveryEndpoint);
            //var config = configurationManager.GetConfigurationAsync().Result;

            string json = "{\"issuer\":\"{0}\",\"jwks_uri\":\"{0}/core/.well-known/jwks\",\"authorization_endpoint\":\"{0}/core/connect/authorize\",\"token_endpoint\":\"{0}/core/connect/token\",\"userinfo_endpoint\":\"{0}/core/connect/userinfo\",\"end_session_endpoint\":\"{0}/core/connect/endsession\",\"check_session_iframe\":\"{0}/core/connect/checksession\",\"frontchannel_logout_supported\":true,\"frontchannel_logout_session_supported\":true,\"scopes_supported\":[\"roles\",\"openid\",\"profile\",\"email\",\"phone\",\"address\",\"fisapi\",\"ssoclient\",\"vehcheckapi\"],\"claims_supported\":[\"role\",\"sub\",\"name\",\"family_name\",\"given_name\",\"middle_name\",\"nickname\",\"preferred_username\",\"profile\",\"picture\",\"website\",\"gender\",\"birthdate\",\"zoneinfo\",\"locale\",\"updated_at\",\"email\",\"email_verified\",\"phone_number\",\"phone_number_verified\",\"address\"],\"response_types_supported\":[\"code\",\"token\",\"id_token\",\"id_tokentoken\",\"codeid_token\",\"codetoken\",\"codeid_tokentoken\"],\"response_modes_supported\":[\"form_post\",\"query\",\"fragment\"],\"grant_types_supported\":[\"authorization_code\",\"client_credentials\",\"password\",\"refresh_token\",\"implicit\"],\"subject_types_supported\":[\"public\"],\"id_token_signing_alg_values_supported\":[\"RS256\"],\"code_challenge_methods_supported\":[\"plain\",\"S256\"],\"token_endpoint_auth_methods_supported\":[\"client_secret_post\",\"client_secret_basic\"]}";
            var config = new OpenIdConnectConfiguration(json.Replace("{0}", AppSettings.SSOAuthBaseUri));

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = AppSettings.Authority,
                ClientId = "8A4E2741-A9BA-4F19-BED7-21B6CF602A6C",
                Scope = "openid profile otars",
                RedirectUri = AppSettings.RedirectUri,

                Configuration = config,
                //Configuration = new OpenIdConnectConfiguration(json.Replace("{0}", AppSettings.SSOAuthBaseUri))
                //{
                //    //JwksUri = "http://localhost/core/.well-known/jwks"
                //},
                TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningTokens = new[] { new X509SecurityToken(certificate) }
                },
                ResponseType = "id_token token",
                SignInAsAuthenticationType = "Cookies",                

                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    
                    SecurityTokenValidated = n =>
                    {
                        logger.Info("Raise event : SecurityTokenValidated");

                        var id = n.AuthenticationTicket.Identity;
                        
                        // we want to keep sub
                        var sub = id.FindFirst(Constants.ClaimTypes.Subject);
                        if (sub != null)
                        {
                            logger.Debug("Sub: " + sub.Value);

                            var dataProvider = ObjectContainer.Container.Resolve<IScopeDataProvider>();
                            var identity = ObjectContainer.Container
                                .Resolve<IUserService>()
                                .Get(sub.Value.AESDecrypt());

                            if (identity != null)
                            {
                                // SessionScopeDataProvider uses Add(), so remove an old
                                // value before registering the identity for this session.
                                dataProvider.Remove(GlobalConstants.CurrentIdentity);
                                dataProvider.Register(GlobalConstants.CurrentIdentity, identity);
                            }
                        }

                        // create new identity and set name and role claim type
                        var nid = new ClaimsIdentity(
                            id.AuthenticationType,
                            Constants.ClaimTypes.GivenName,
                            Constants.ClaimTypes.Role);

                        //nid.AddClaim(sub);
                        if (sub == null)
                            return Task.FromResult(0);

                        nid.AddClaim(new Claim(Constants.ClaimTypes.Subject, sub.Value.AESDecrypt()));

                        n.AuthenticationTicket = new AuthenticationTicket(
                            nid,
                            n.AuthenticationTicket.Properties);

                        return Task.FromResult(0);
                    },

                }
            });
        }

        public byte[] GetCertificatePublicKey(string url)
        {
            if (string.IsNullOrEmpty(url))
                return new byte[0];

            var uri = new Uri(url);
            url = uri.GetLeftPart(UriPartial.Authority);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            request.Method = "GET";
            using (WebResponse response = request.GetResponse())
            {
            }
            
            //retrieve the ssl cert and assign it to an X509Certificate object
            X509Certificate cert = request.ServicePoint.Certificate;

            //convert the X509Certificate to an X509Certificate2 object by passing it into the constructor
            var cert2 = new X509Certificate2(cert);

            return cert2.RawData;
        }

    }


}
