using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace MTAoarsGeneral.Web
{
    public static class Constants
    {
        public const string IdentityServerName = "IdentityServer3";
        public const string ScopeDescriptionSuffix = "_Description";
        public const string ScopeDisplayNameSuffix = "_DisplayName";
        public const string DefaultHashAlgorithm = "SHA256";
        public const int SignInMessageThreshold = 5;
        public const string PartialLoginResumeClaimPrefix = "partial_login_resume_id:";
        public const string BuiltInIdentityProvider = "idsrv";
        public const string PartialSignInAuthenticationType = "idsrv.partial";
        public const string ExternalAuthenticationType = "idsrv.external";
        public const string PrimaryAuthenticationType = "idsrv";
        public const string AccessTokenAudience = "{0}resources";
        public static string[] SupportedSubjectTypes;
        public static List<string> SupportedTokenTypeHints;
        public static readonly string[] AuthenticateResultClaimTypes;
        public static readonly string[] OidcProtocolClaimTypes;
        public static readonly string[] ClaimsProviderFilerClaimTypes;
        public static readonly Dictionary<string, IEnumerable<string>> ScopeToClaimsMapping;
        public static Dictionary<string, HttpStatusCode> ProtectedResourceErrorStatusCodes;
        public static readonly List<string> SupportedCodeChallengeMethods;
        public static readonly List<string> SupportedPromptModes;
        public static readonly List<string> SupportedDisplayModes;
        public static readonly List<string> SupportedResponseModes;
        public static IEnumerable<string> AllowedProofKeyAlgorithms;
        public static readonly List<string> SupportedGrantTypes;
        //public static readonly Dictionary<Flows, IEnumerable<string>> AllowedResponseModesForFlow;
        public static readonly TimeSpan DefaultCookieTimeSpan;
        public static readonly TimeSpan DefaultRememberMeDuration;
        public static readonly TimeSpan DefaultCacheDuration;
        public static readonly List<string> SupportedResponseTypes;
        public static readonly TimeSpan ExternalCookieTimeSpan;
        //public static readonly List<Flows> AllowedFlowsForAuthorizeEndpoint;
        //public static readonly List<Flows> AllowedFlowsForAuthorizationCodeGrantType;
        //public static readonly Dictionary<Flows, IList<string>> ProofKeyFlowToResponseTypesMapping;
        //public static readonly Dictionary<string, ScopeRequirement> ResponseTypeToScopeRequirement;
        //public static readonly Dictionary<string, Flows> ResponseTypeToFlowMapping;

        public enum ScopeRequirement
        {
            None = 0,
            ResourceOnly = 1,
            IdentityOnly = 2,
            Identity = 3
        }

        public static class ParsedSecretTypes
        {
            public const string SharedSecret = "SharedSecret";
            public const string X509Certificate = "X509Certificate";
            public const string JwtBearer = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
        }
        public static class ProtectedResourceErrors
        {
            public const string InvalidToken = "invalid_token";
            public const string ExpiredToken = "expired_token";
            public const string InvalidRequest = "invalid_request";
            public const string InsufficientScope = "insufficient_scope";
        }
        public static class StandardScopes
        {
            //
            // Summary:
            //     REQUIRED. Informs the Authorization Server that the Client is making an OpenID
            //     Connect request. If the openid scope value is not present, the behavior is entirely
            //     unspecified.
            public const string OpenId = "openid";
            //
            // Summary:
            //     OPTIONAL. This scope value requests access to the End-User's default profile
            //     Claims, which are: name, family_name, given_name, middle_name, nickname, preferred_username,
            //     profile, picture, website, gender, birthdate, zoneinfo, locale, and updated_at.
            public const string Profile = "profile";
            //
            // Summary:
            //     OPTIONAL. This scope value requests access to the email and email_verified Claims.
            public const string Email = "email";
            //
            // Summary:
            //     OPTIONAL. This scope value requests access to the address Claim.
            public const string Address = "address";
            //
            // Summary:
            //     OPTIONAL. This scope value requests access to the phone_number and phone_number_verified
            //     Claims.
            public const string Phone = "phone";
            //
            // Summary:
            //     This scope value MUST NOT be used with the OpenID Connect Implicit Client Implementer's
            //     Guide 1.0. See the OpenID Connect Basic Client Implementer's Guide 1.0 (http://openid.net/specs/openid-connect-implicit-1_0.html#OpenID.Basic)
            //     for its usage in that subset of OpenID Connect.
            public const string OfflineAccess = "offline_access";
            public const string AllClaims = "all_claims";
            public const string Roles = "roles";
        }
        public static class ClaimTypes
        {
            //
            // Summary:
            //     Unique Identifier for the End-User at the Issuer.
            public const string Subject = "sub";
            //
            // Summary:
            //     Authentication Methods References. JSON array of strings that are identifiers
            //     for authentication methods used in the authentication.
            public const string AuthenticationMethod = "amr";
            //
            // Summary:
            //     Session identifier. This represents a Session of an OP at an RP to a User Agent
            //     or device for a logged-in End-User. Its contents are unique to the OP and opaque
            //     to the RP.
            public const string SessionId = "sid";
            //
            // Summary:
            //     Authentication Context Class Reference. String specifying an Authentication Context
            //     Class Reference value that identifies the Authentication Context Class that the
            //     authentication performed satisfied. The value "0" indicates the End-User authentication
            //     did not meet the requirements of ISO/IEC 29115 level 1. Authentication using
            //     a long-lived browser cookie, for instance, is one example where the use of "level
            //     0" is appropriate. Authentications with level 0 SHOULD NOT be used to authorize
            //     access to any resource of any monetary value. (This corresponds to the OpenID
            //     2.0 PAPE nist_auth_level 0.) An absolute URI or an RFC 6711 registered name SHOULD
            //     be used as the acr value; registered names MUST NOT be used with a different
            //     meaning than that which is registered. Parties using this claim will need to
            //     agree upon the meanings of the values used, which may be context-specific. The
            //     acr value is a case sensitive string.
            public const string AuthenticationContextClassReference = "acr";
            //
            // Summary:
            //     Time when the End-User authentication occurred. Its value is a JSON number representing
            //     the number of seconds from 1970-01-01T0:0:0Z as measured in UTC until the date/time.
            //     When a max_age request is made or when auth_time is requested as an Essential
            //     Claim, then this Claim is REQUIRED; otherwise, its inclusion is OPTIONAL.
            public const string AuthenticationTime = "auth_time";
            //
            // Summary:
            //     The party to which the ID Token was issued. If present, it MUST contain the OAuth
            //     2.0 Client ID of this party. This Claim is only needed when the ID Token has
            //     a single audience value and that audience is different than the authorized party.
            //     It MAY be included even when the authorized party is the same as the sole audience.
            //     The azp value is a case sensitive string containing a StringOrURI value.
            public const string AuthorizedParty = "azp";
            //
            // Summary:
            //     Access Token hash value. Its value is the base64url encoding of the left-most
            //     half of the hash of the octets of the ASCII representation of the access_token
            //     value, where the hash algorithm used is the hash algorithm used in the alg Header
            //     Parameter of the ID Token's JOSE Header. For instance, if the alg is RS256, hash
            //     the access_token value with SHA-256, then take the left-most 128 bits and base64url
            //     encode them. The at_hash value is a case sensitive string.
            public const string AccessTokenHash = "at_hash";
            //
            // Summary:
            //     Code hash value. Its value is the base64url encoding of the left-most half of
            //     the hash of the octets of the ASCII representation of the code value, where the
            //     hash algorithm used is the hash algorithm used in the alg Header Parameter of
            //     the ID Token's JOSE Header. For instance, if the alg is HS512, hash the code
            //     value with SHA-512, then take the left-most 256 bits and base64url encode them.
            //     The c_hash value is a case sensitive string.
            public const string AuthorizationCodeHash = "c_hash";
            //
            // Summary:
            //     String value used to associate a Client session with an ID Token, and to mitigate
            //     replay attacks. The value is passed through unmodified from the Authentication
            //     Request to the ID Token. If present in the ID Token, Clients MUST verify that
            //     the nonce Claim Value is equal to the value of the nonce parameter sent in the
            //     Authentication Request. If present in the Authentication Request, Authorization
            //     Servers MUST include a nonce Claim in the ID Token with the Claim Value being
            //     the nonce value sent in the Authentication Request. Authorization Servers SHOULD
            //     perform no other processing on nonce values used. The nonce value is a case sensitive
            //     string.
            public const string Nonce = "nonce";
            //
            // Summary:
            //     JWT ID. A unique identifier for the token, which can be used to prevent reuse
            //     of the token. These tokens MUST only be used once, unless conditions for reuse
            //     were negotiated between the parties; any such negotiation is beyond the scope
            //     of this specification.
            public const string JwtId = "jti";
            //
            // Summary:
            //     The iat (issued at) claim identifies the time at which the JWT was issued, ,
            //     specified as the number of seconds from 1970-01-01T0:0:0Z
            public const string IssuedAt = "iat";
            //
            // Summary:
            //     Proof key confirmation
            public const string Confirmation = "cnf";
            //
            // Summary:
            //     OpenID Connect requests MUST contain the "openid" scope value. If the openid
            //     scope value is not present, the behavior is entirely unspecified. Other scope
            //     values MAY be present. Scope values used that are not understood by an implementation
            //     SHOULD be ignored.
            public const string Scope = "scope";
            public const string Id = "id";
            public const string Secret = "secret";
            public const string IdentityProvider = "idp";
            public const string Role = "role";
            public const string ReferenceTokenId = "reference_token_id";
            public const string AuthorizationReturnUrl = "authorization_return_url";
            public const string PartialLoginRestartUrl = "partial_login_restart_url";
            public const string PartialLoginReturnUrl = "partial_login_return_url";
            //
            // Summary:
            //     OAuth 2.0 Client Identifier valid at the Authorization Server.
            public const string ClientId = "client_id";
            public const string ExternalProviderUserId = "external_provider_user_id";
            //
            // Summary:
            //     Time the End-User's information was last updated. Its value is a JSON number
            //     representing the number of seconds from 1970-01-01T0:0:0Z as measured in UTC
            //     until the date/time.
            public const string UpdatedAt = "updated_at";
            //
            // Summary:
            //     The time before which the JWT MUST NOT be accepted for processing, specified
            //     as the number of seconds from 1970-01-01T0:0:0Z
            public const string NotBefore = "nbf";
            //
            // Summary:
            //     End-User's full name in displayable form including all name parts, possibly including
            //     titles and suffixes, ordered according to the End-User's locale and preferences.
            public const string Name = "name";
            //
            // Summary:
            //     Given name(s) or first name(s) of the End-User. Note that in some cultures, people
            //     can have multiple given names; all can be present, with the names being separated
            //     by space characters.
            public const string GivenName = "given_name";
            //
            // Summary:
            //     Surname(s) or last name(s) of the End-User. Note that in some cultures, people
            //     can have multiple family names or no family name; all can be present, with the
            //     names being separated by space characters.
            public const string FamilyName = "family_name";
            //
            // Summary:
            //     Middle name(s) of the End-User. Note that in some cultures, people can have multiple
            //     middle names; all can be present, with the names being separated by space characters.
            //     Also note that in some cultures, middle names are not used.
            public const string MiddleName = "middle_name";
            //
            // Summary:
            //     Casual name of the End-User that may or may not be the same as the given_name.
            //     For instance, a nickname value of Mike might be returned alongside a given_name
            //     value of Michael.
            public const string NickName = "nickname";
            //
            // Summary:
            //     Shorthand name by which the End-User wishes to be referred to at the RP, such
            //     as janedoe or j.doe. This value MAY be any valid JSON string including special
            //     characters such as @, /, or whitespace. The relying party MUST NOT rely upon
            //     this value being unique
            //
            // Remarks:
            //     The RP MUST NOT rely upon this value being unique, as discussed in http://openid.net/specs/openid-connect-basic-1_0-32.html#ClaimStability
            public const string PreferredUserName = "preferred_username";
            //
            // Summary:
            //     URL of the End-User's profile page. The contents of this Web page SHOULD be about
            //     the End-User.
            public const string Profile = "profile";
            //
            // Summary:
            //     URL of the End-User's profile picture. This URL MUST refer to an image file (for
            //     example, a PNG, JPEG, or GIF image file), rather than to a Web page containing
            //     an image.
            //
            // Remarks:
            //     Note that this URL SHOULD specifically reference a profile photo of the End-User
            //     suitable for displaying when describing the End-User, rather than an arbitrary
            //     photo taken by the End-User.
            public const string Picture = "picture";
            //
            // Summary:
            //     URL of the End-User's Web page or blog. This Web page SHOULD contain information
            //     published by the End-User or an organization that the End-User is affiliated
            //     with.
            public const string WebSite = "website";
            //
            // Summary:
            //     The exp (expiration time) claim identifies the expiration time on or after which
            //     the token MUST NOT be accepted for processing, specified as the number of seconds
            //     from 1970-01-01T0:0:0Z
            public const string Expiration = "exp";
            //
            // Summary:
            //     End-User's preferred e-mail address. Its value MUST conform to the RFC 5322 [RFC5322]
            //     addr-spec syntax. The relying party MUST NOT rely upon this value being unique
            public const string Email = "email";
            //
            // Summary:
            //     End-User's gender. Values defined by this specification are "female" and "male".
            //     Other values MAY be used when neither of the defined values are applicable.
            public const string Gender = "gender";
            //
            // Summary:
            //     End-User's birthday, represented as an ISO 8601:2004 [ISO8601‑2004] YYYY-MM-DD
            //     format. The year MAY be 0000, indicating that it is omitted. To represent only
            //     the year, YYYY format is allowed. Note that depending on the underlying platform's
            //     date related function, providing just year can result in varying month and day,
            //     so the implementers need to take this factor into account to correctly process
            //     the dates.
            public const string BirthDate = "birthdate";
            //
            // Summary:
            //     String from the time zone database (http://www.twinsun.com/tz/tz-link.htm) representing
            //     the End-User's time zone. For example, Europe/Paris or America/Los_Angeles.
            public const string ZoneInfo = "zoneinfo";
            //
            // Summary:
            //     End-User's locale, represented as a BCP47 [RFC5646] language tag. This is typically
            //     an ISO 639-1 Alpha-2 [ISO639‑1] language code in lowercase and an ISO 3166-1
            //     Alpha-2 [ISO3166‑1] country code in uppercase, separated by a dash. For example,
            //     en-US or fr-CA. As a compatibility note, some implementations have used an underscore
            //     as the separator rather than a dash, for example, en_US; Relying Parties MAY
            //     choose to accept this locale syntax as well.
            public const string Locale = "locale";
            //
            // Summary:
            //     End-User's preferred telephone number. E.164 (https://www.itu.int/rec/T-REC-E.164/e)
            //     is RECOMMENDED as the format of this Claim, for example, +1 (425) 555-1212 or
            //     +56 (2) 687 2400. If the phone number contains an extension, it is RECOMMENDED
            //     that the extension be represented using the RFC 3966 [RFC3966] extension syntax,
            //     for example, +1 (604) 555-1234;ext=5678.
            public const string PhoneNumber = "phone_number";
            //
            // Summary:
            //     True if the End-User's phone number has been verified; otherwise false. When
            //     this Claim Value is true, this means that the OP took affirmative steps to ensure
            //     that this phone number was controlled by the End-User at the time the verification
            //     was performed.
            //
            // Remarks:
            //     The means by which a phone number is verified is context-specific, and dependent
            //     upon the trust framework or contractual agreements within which the parties are
            //     operating. When true, the phone_number Claim MUST be in E.164 format and any
            //     extensions MUST be represented in RFC 3966 format.
            public const string PhoneNumberVerified = "phone_number_verified";
            //
            // Summary:
            //     End-User's preferred postal address. The value of the address member is a JSON
            //     structure containing some or all of the members defined in http://openid.net/specs/openid-connect-basic-1_0-32.html#AddressClaim
            public const string Address = "address";
            //
            // Summary:
            //     Audience(s) that this ID Token is intended for. It MUST contain the OAuth 2.0
            //     client_id of the Relying Party as an audience value. It MAY also contain identifiers
            //     for other audiences. In the general case, the aud value is an array of case sensitive
            //     strings. In the common special case when there is one audience, the aud value
            //     MAY be a single case sensitive string.
            public const string Audience = "aud";
            //
            // Summary:
            //     Issuer Identifier for the Issuer of the response. The iss value is a case sensitive
            //     URL using the https scheme that contains scheme, host, and optionally, port number
            //     and path components and no query or fragment components.
            public const string Issuer = "iss";
            //
            // Summary:
            //     "true" if the End-User's e-mail address has been verified; otherwise "false".
            //
            // Remarks:
            //     When this Claim Value is "true", this means that the OP took affirmative steps
            //     to ensure that this e-mail address was controlled by the End-User at the time
            //     the verification was performed. The means by which an e-mail address is verified
            //     is context-specific, and dependent upon the trust framework or contractual agreements
            //     within which the parties are operating.
            public const string EmailVerified = "email_verified";
            public const string PartialLoginResumeId = "partial_login_resume_id:{0}";
        }
        public static class AuthenticationMethods
        {
            public const string Certificate = "certificate";
            public const string Password = "password";
            public const string TwoFactorAuthentication = "2fa";
            public const string External = "external";
        }
        public static class SecretTypes
        {
            public const string SharedSecret = "SharedSecret";
            public const string X509CertificateThumbprint = "X509Thumbprint";
            public const string X509CertificateName = "X509Name";
            public const string X509CertificateBase64 = "X509CertificateBase64";
        }
        public static class Authentication
        {
            public const string SigninQueryParamName = "signin";
            public const string SigninId = "signinid";
            public const string SignoutId = "id";
            public const string KatanaAuthenticationType = "katanaAuthenticationType";
            public const string PartialLoginRememberMe = "idsvr:rememberme";
        }
        public static class RouteNames
        {
            public const string Welcome = "idsrv.welcome";
            public const string Login = "idsrv.authentication.login";
            public const string LoginExternal = "idsrv.authentication.loginexternal";
            public const string LoginExternalCallback = "idsrv.authentication.loginexternalcallback";
            public const string LogoutPrompt = "idsrv.authentication.logoutprompt";
            public const string Logout = "idsrv.authentication.logout";
            public const string ResumeLoginFromRedirect = "idsrv.authentication.resume";
            public const string CspReport = "idsrv.csp.report";
            public const string ClientPermissions = "idsrv.permissions";

            public static class Oidc
            {
                public const string AccessTokenValidation = "idsrv.oidc.accesstokenvalidation";
                public const string Authorize = "idsrv.oidc.authorize";
                public const string Consent = "idsrv.oidc.consent";
                public const string SwitchUser = "idsrv.oidc.switch";
                public const string CheckSession = "idsrv.oidc.checksession";
                public const string DiscoveryConfiguration = "idsrv.oidc.discoveryconfiguration";
                public const string DiscoveryWebKeys = "idsrv.oidc.discoverywebkeys";
                public const string EndSession = "idsrv.oidc.endsession";
                public const string EndSessionCallback = "idsrv.oidc.endsessioncallback";
                public const string IdentityTokenValidation = "idsrv.oidc.identitytokenvalidation";
                public const string Token = "idsrv.oidc.token";
                public const string Revocation = "idsrv.oidc.revocation";
                public const string UserInfo = "idsrv.oidc.userinfo";
                public const string Introspection = "idsrv.oidc.introspection";
            }
        }
        public static class RoutePaths
        {
            public const string Welcome = "";
            public const string Login = "login";
            public const string LoginExternal = "external";
            public const string LoginExternalCallback = "callback";
            public const string Logout = "logout";
            public const string ResumeLoginFromRedirect = "return";
            public const string CspReport = "csp/report";
            public const string ClientPermissions = "permissions";
            public static readonly string[] CorsPaths;

            public static class Oidc
            {
                public const string Authorize = "connect/authorize";
                public const string Consent = "connect/consent";
                public const string SwitchUser = "connect/switch";
                public const string DiscoveryConfiguration = ".well-known/openid-configuration";
                public const string DiscoveryWebKeys = ".well-known/jwks";
                public const string Token = "connect/token";
                public const string Revocation = "connect/revocation";
                public const string UserInfo = "connect/userinfo";
                public const string AccessTokenValidation = "connect/accessTokenValidation";
                public const string Introspection = "connect/introspect";
                public const string IdentityTokenValidation = "connect/identityTokenValidation";
                public const string EndSession = "connect/endsession";
                public const string EndSessionCallback = "connect/endsessioncallback";
                public const string CheckSession = "connect/checksession";
            }
        }
        public static class OwinEnvironment
        {
            public const string IdentityServerBasePath = "idsrv:IdentityServerBasePath";
            public const string IdentityServerHost = "idsrv:IdentityServerHost";
            public const string IdentityServerOrigin = "idsrv:IdentityServerOrigin";
            public const string RequestId = "idsrv:RequestId";
        }
        public static class TokenErrors
        {
            public const string InvalidRequest = "invalid_request";
            public const string InvalidClient = "invalid_client";
            public const string InvalidGrant = "invalid_grant";
            public const string UnauthorizedClient = "unauthorized_client";
            public const string UnsupportedGrantType = "unsupported_grant_type";
            public const string UnsupportedResponseType = "unsupported_response_type";
            public const string InvalidScope = "invalid_scope";
        }
        public static class LocalizationCategories
        {
            public const string Messages = "Messages";
            public const string Events = "Events";
            public const string Scopes = "Scopes";
        }
        public static class TokenTypeHints
        {
            public const string RefreshToken = "refresh_token";
            public const string AccessToken = "access_token";
        }
        public static class RevocationErrors
        {
            public const string UnsupportedTokenType = "unsupported_token_type";
        }
        public static class TokenEndpointAuthenticationMethods
        {
            public const string PostBody = "client_secret_post";
            public const string BasicAuthentication = "client_secret_basic";
            public const string PrivateKeyJwt = "private_key_jwt";
        }
        public static class AuthorizeErrors
        {
            public const string InvalidRequest = "invalid_request";
            public const string UnauthorizedClient = "unauthorized_client";
            public const string AccessDenied = "access_denied";
            public const string UnsupportedResponseType = "unsupported_response_type";
            public const string InvalidScope = "invalid_scope";
            public const string ServerError = "server_error";
            public const string TemporarilyUnavailable = "temporarily_unavailable";
            public const string InteractionRequired = "interaction_required";
            public const string LoginRequired = "login_required";
            public const string AccountSelectionRequired = "account_selection_required";
            public const string ConsentRequired = "consent_required";
            public const string InvalidRequestUri = "invalid_request_uri";
            public const string InvalidRequestObject = "invalid_request_object";
            public const string RequestNotSupported = "request_not_supported";
            public const string RequestUriNotSupported = "request_uri_not_supported";
            public const string RegistrationNotSupported = "registration_not_supported";
        }
        public static class ResponseModes
        {
            public const string FormPost = "form_post";
            public const string Query = "query";
            public const string Fragment = "fragment";
        }
        public static class KnownAcrValues
        {
            public const string HomeRealm = "idp:";
            public const string Tenant = "tenant:";
        }
        public static class ProfileDataCallers
        {
            public const string UserInfoEndpoint = "UserInfoEndpoint";
            public const string ClaimsProviderIdentityToken = "ClaimsProviderIdentityToken";
            public const string ClaimsProviderAccessToken = "ClaimsProviderAccessToken";
        }
        public static class AuthorizeRequest
        {
            public const string Scope = "scope";
            public const string ResponseType = "response_type";
            public const string ClientId = "client_id";
            public const string RedirectUri = "redirect_uri";
            public const string State = "state";
            public const string ResponseMode = "response_mode";
            public const string Nonce = "nonce";
            public const string Display = "display";
            public const string Prompt = "prompt";
            public const string MaxAge = "max_age";
            public const string UiLocales = "ui_locales";
            public const string IdTokenHint = "id_token_hint";
            public const string LoginHint = "login_hint";
            public const string AcrValues = "acr_values";
            public const string CodeChallenge = "code_challenge";
            public const string CodeChallengeMethod = "code_challenge_method";
        }
        public static class TokenRequest
        {
            public const string GrantType = "grant_type";
            public const string RedirectUri = "redirect_uri";
            public const string ClientId = "client_id";
            public const string ClientSecret = "client_secret";
            public const string ClientAssertion = "client_assertion";
            public const string ClientAssertionType = "client_assertion_type";
            public const string Assertion = "assertion";
            public const string Code = "code";
            public const string RefreshToken = "refresh_token";
            public const string Scope = "scope";
            public const string UserName = "username";
            public const string Password = "password";
            public const string CodeVerifier = "code_verifier";
            public const string Algorithm = "alg";
            public const string Key = "key";
        }
        public static class EndSessionRequest
        {
            public const string IdTokenHint = "id_token_hint";
            public const string PostLogoutRedirectUri = "post_logout_redirect_uri";
            public const string State = "state";
        }
        public static class TokenResponse
        {
            public const string AccessToken = "access_token";
            public const string IdentityToken = "id_token";
            public const string ExpiresIn = "expires_in";
            public const string RefreshToken = "refresh_token";
            public const string TokenType = "token_type";
            public const string State = "state";
            public const string Scope = "scope";
            public const string Error = "error";
        }
        public static class TokenTypes
        {
            public const string AccessToken = "access_token";
            public const string IdentityToken = "id_token";
            public const string RefreshToken = "refresh_token";
        }
        public static class CodeChallengeMethods
        {
            public const string Plain = "plain";
            public const string SHA_256 = "S256";
        }
        public static class ResponseTokenTypes
        {
            public const string Bearer = "Bearer";
            public const string PoP = "pop";
        }
        public static class GrantTypes
        {
            public const string Password = "password";
            public const string AuthorizationCode = "authorization_code";
            public const string ClientCredentials = "client_credentials";
            public const string RefreshToken = "refresh_token";
            public const string Implicit = "implicit";
            public const string Saml2Bearer = "urn:ietf:params:oauth:grant-type:saml2-bearer";
            public const string JwtBearer = "urn:ietf:params:oauth:grant-type:jwt-bearer";
        }
        public static class ClientAssertionTypes
        {
            public const string JwtBearer = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer";
        }
        public static class ResponseTypes
        {
            public const string Code = "code";
            public const string Token = "token";
            public const string IdToken = "id_token";
            public const string IdTokenToken = "id_token token";
            public const string CodeIdToken = "code id_token";
            public const string CodeToken = "code token";
            public const string CodeIdTokenToken = "code id_token token";
        }
        public static class SigningAlgorithms
        {
            public const string RSA_SHA_256 = "RS256";
        }
        public static class DisplayModes
        {
            public const string Page = "page";
            public const string Popup = "popup";
            public const string Touch = "touch";
            public const string Wap = "wap";
        }
        public static class PromptModes
        {
            public const string None = "none";
            public const string Login = "login";
            public const string Consent = "consent";
            public const string SelectAccount = "select_account";
        }
        public static class AuthenticationSchemes
        {
            public const string BearerAuthorizationHeader = "Bearer";
            public const string BearerFormPost = "access_token";
            public const string BearerQueryString = "access_token";
        }
        public static class ClaimValueTypes
        {
            public const string Json = "json";
        }
    }
}