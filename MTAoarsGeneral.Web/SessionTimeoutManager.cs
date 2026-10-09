using System;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Practices.Unity;
using MTAoarsGeneral.Services.Interfaces;
using MTAoarsGeneral.Utilities.Constants;
using MTAoarsGeneral.Utilities.IoC;
using MTAoarsGeneral.Utilities.Mvc;

namespace MTAoarsGeneral.Web
{
    /// <summary>
    /// PCI DSS session timeouts.
    /// Idle timeout: a session with no request for SessionIdleTimeoutMinutes (default 15) is destroyed.
    /// Absolute timeout: a session is destroyed SessionAbsoluteTimeoutMinutes (default 60) after login,
    /// regardless of activity, so a stolen session id cannot be used indefinitely.
    /// On expiry every session layer (ASP.NET session, Forms and OWIN cookies) is signed out and the
    /// user is sent to the SSOUrl app setting to re-authenticate.
    /// </summary>
    public static class SessionTimeoutManager
    {
        public const string ExpiredHeader = "X-Session-Expired";

        private const string LoginUtcKey = "__SessionLoginUtc";
        private const string LastActivityUtcKey = "__SessionLastActivityUtc";
        private const string LoginOwnerKey = "__SessionLoginOwner";
        private const string SessionCookieName = "ASP.NET_SessionId";

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static TimeSpan IdleTimeout
        {
            get { return TimeSpan.FromMinutes(GetMinutes("SessionIdleTimeoutMinutes", 15)); }
        }

        public static TimeSpan AbsoluteTimeout
        {
            get { return TimeSpan.FromMinutes(GetMinutes("SessionAbsoluteTimeoutMinutes", 60)); }
        }

        public static string SSOUrl
        {
            get { return ConfigurationManager.AppSettings["SSOUrl"]; }
        }

        /// <summary>
        /// Called on every request once session state is available (Global.asax PostAcquireRequestState).
        /// </summary>
        public static void Enforce(HttpContext context)
        {
            if (context == null || context.Session == null) return;

            var session = context.Session;
            var identity = session[GlobalConstants.CurrentIdentity] as Identity;
            if (identity == null) {
                // Not logged in: nothing to time out.
                session.Remove(LoginUtcKey);
                session.Remove(LastActivityUtcKey);
                session.Remove(LoginOwnerKey);
                return;
            }

            var now = DateTime.UtcNow;
            var owner = GetOwner(identity);

            // A new login in this session (different user or login record) starts a new absolute window.
            if (!(session[LoginUtcKey] is DateTime) || !owner.Equals(session[LoginOwnerKey] as string)) {
                session[LoginUtcKey] = now;
                session[LastActivityUtcKey] = now;
                session[LoginOwnerKey] = owner;
                return;
            }

            // SSO hands the user back here to log in; let it replace the identity instead of bouncing it.
            if (IsLoginEntry(context.Request)) {
                session[LastActivityUtcKey] = now;
                return;
            }

            var loginUtc = (DateTime)session[LoginUtcKey];
            var lastActivityUtc = session[LastActivityUtcKey] is DateTime ? (DateTime)session[LastActivityUtcKey] : loginUtc;

            var idleExpired = now - lastActivityUtc > IdleTimeout;
            var absoluteExpired = now - loginUtc > AbsoluteTimeout;
            if (!idleExpired && !absoluteExpired) {
                session[LastActivityUtcKey] = now;
                return;
            }

            logger.Info(string.Format("Session expired ({0}) for user id {1}.",
                absoluteExpired ? "absolute timeout" : "idle timeout", identity.UserID));

            SignOut(context);
            WriteExpiredResponse(context);
        }

        /// <summary>
        /// Seconds left before the absolute timeout for the current session (used by the browser timer).
        /// </summary>
        public static int GetRemainingAbsoluteSeconds(HttpContextBase context)
        {
            var absolute = (int)AbsoluteTimeout.TotalSeconds;
            if (context == null || context.Session == null || !(context.Session[LoginUtcKey] is DateTime))
                return absolute;

            var remaining = (int)((DateTime)context.Session[LoginUtcKey] + AbsoluteTimeout - DateTime.UtcNow).TotalSeconds;
            return remaining < 0 ? 0 : remaining;
        }

        /// <summary>
        /// Invalidates every authentication/session layer for the current request.
        /// </summary>
        public static void SignOut(HttpContext context)
        {
            if (context == null) return;

            var session = context.Session;
            var identity = session == null ? null : session[GlobalConstants.CurrentIdentity] as Identity;

            if (identity != null && identity.UserLoginID > 0) {
                try {
                    ObjectContainer.Container.Resolve<IUserService>().SaveLogoutDetail(identity.UserLoginID);
                }
                catch (Exception ex) {
                    logger.Error(ex);
                }
            }

            try {
                context.GetOwinContext().Authentication.SignOut("Cookies");
            }
            catch (Exception ex) {
                // The OWIN pipeline is not running (owin:AutomaticAppStartup=false); nothing to sign out.
                logger.Debug("OWIN sign out skipped: " + ex.Message);
            }

            FormsAuthentication.SignOut();

            if (session != null) {
                session.Clear();
                session.Abandon();
            }

            context.Response.Cookies.Add(new HttpCookie(SessionCookieName, string.Empty) {
                Expires = DateTime.UtcNow.AddDays(-1),
                HttpOnly = true,
                Secure = context.Request.IsSecureConnection
            });
        }

        static void WriteExpiredResponse(HttpContext context)
        {
            var response = context.Response;
            response.Clear();

            if (new HttpRequestWrapper(context.Request).IsAjaxRequest()) {
                // Site.js checkError() and SessionTimeout.js both send the browser to the SSO page.
                response.StatusCode = 401;
                response.SuppressFormsAuthenticationRedirect = true;
                response.AddHeader(ExpiredHeader, "1");
                response.ContentType = "text/plain";
                response.Write("Unauthorized to view the page. Your session has expired, please login again.");
            }
            else {
                response.Redirect(SSOUrl, false);
            }

            context.ApplicationInstance.CompleteRequest();
        }

        static bool IsLoginEntry(HttpRequest request)
        {
            var path = (request.AppRelativeCurrentExecutionFilePath ?? string.Empty).TrimEnd('/');
            return path.Equals("~/Home/Authorize", StringComparison.OrdinalIgnoreCase);
        }

        static string GetOwner(Identity identity)
        {
            return identity.UserID.ToString(CultureInfo.InvariantCulture) + ":" +
                identity.UserLoginID.ToString(CultureInfo.InvariantCulture);
        }

        static int GetMinutes(string key, int defaultValue)
        {
            int minutes;
            if (int.TryParse(ConfigurationManager.AppSettings[key], NumberStyles.Integer, CultureInfo.InvariantCulture, out minutes) && minutes > 0)
                return minutes;
            return defaultValue;
        }

    }// class
}// namespace
