using System;
using System.Web;
using System.Web.Security;

namespace Lanban.Controller
{
    public class LanbanAuthentication
    {
        public void Authenticate(HttpResponse response, Model.UserModel user)
        {
            var ticket = GetAuthenTicket(user.Username, user.User_ID.ToString(), 30);
            SetAuthenCookie(response, ticket);
        }
        
        public void SetAuthenCookie(HttpResponse response, FormsAuthenticationTicket ticket)
        {
            string encTicket = FormsAuthentication.Encrypt(ticket);
            response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }

        public FormsAuthenticationTicket GetAuthenTicket(string username, string userdata, int timeout)
        {
            var now = DateTime.Now;
            return new FormsAuthenticationTicket(1, username, now, now.AddMinutes(timeout), false, userdata, FormsAuthentication.FormsCookiePath);
        }
    }
}