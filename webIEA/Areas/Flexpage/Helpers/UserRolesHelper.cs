using System;
using System.Configuration;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;

namespace Flexpage.Helpers
{
    public delegate string GetUserRoles();

    public class UserRolesHelper
    {
        private HttpContext _context;

        public UserRolesHelper(HttpContext context)
        {
            _context = context ?? throw new ArgumentNullException("Context cannot be null");
        }

        private FormsAuthenticationConfiguration __formsAuthentication = null;
        private FormsAuthenticationConfiguration _formsAuthentication
        {
            get
            {
                if(__formsAuthentication == null)
                {
                    // Get the external Authentication section.
                    AuthenticationSection authenticationSection =
                        (AuthenticationSection)ConfigurationManager.GetSection(
                        "system.web/authentication");

                    // Get the external Forms section.
                    __formsAuthentication = authenticationSection.Forms;
                }
                return __formsAuthentication;
            }
        }

        public void ConfigureUserRoles(string cookieLoggedUser, string cookieUserRoles,
            GetUserRoles getUserRoles)
        {
            if(_context.Request.IsAuthenticated)
            {
                String[] roles;
                string rolesStr;
                
                if(_context.Request.Cookies[cookieLoggedUser] == null
                    || _context.Request.Cookies[cookieLoggedUser].Value != _context.User.Identity.Name)
                {
                    _context.Response.Cookies[cookieLoggedUser].Value = _context.User.Identity.Name;
                    _context.Response.Cookies[cookieLoggedUser].Path = "/";
                    _context.Response.Cookies[cookieLoggedUser].Expires = 
                        DateTime.Now.Add(_formsAuthentication.Timeout);
                    _context.Request.Cookies.Remove(cookieUserRoles);
                }
                if((_context.Request.Cookies[cookieUserRoles] == null)
                    || String.IsNullOrEmpty(_context.Request.Cookies[cookieUserRoles].Value))
                {
                    rolesStr = getUserRoles();

                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                        1,
                        _context.User.Identity.Name,
                        DateTime.Now,
                        DateTime.Now.Add(_formsAuthentication.Timeout),
                        false,
                        rolesStr
                        );

                    roles = new Regex(@"[;,] ?").Split(rolesStr);

                    String cookieStr = FormsAuthentication.Encrypt(ticket);
                    _context.Response.Cookies.Add(new HttpCookie(cookieUserRoles, cookieStr));
                    _context.Response.Cookies[cookieUserRoles].Path = "/";
                    _context.Response.Cookies[cookieUserRoles].Expires = ticket.Expiration;
                }
                else
                {
                    FormsAuthenticationTicket ticket =
                        FormsAuthentication.Decrypt(_context.Request.Cookies[cookieUserRoles].Value);
                    roles = new Regex(@"[;,] ?").Split(ticket.UserData);
                }
                _context.User = new GenericPrincipal(_context.User.Identity, roles);
            }
            else
            {
                _context.Response.Cookies[cookieLoggedUser].Expires = DateTime.Now.AddDays(-1);
                _context.Response.Cookies[cookieUserRoles].Expires = DateTime.Now.AddDays(-1);
            }
        }            
    }
}
