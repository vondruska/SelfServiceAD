using System.Web;

namespace SelfServiceAD.Helpers
{
    public static class WebsiteUser
    {
        public static string Username
        {
            get
            {
                if (HttpContext.Current == null) return null;

                if (HttpContext.Current.Session == null) return null;

                return HttpContext.Current.Session["Username"] as string;
            }

            set { HttpContext.Current.Session["Username"] = value; }
        }

        public static void Logout()
        {
            Username = null;
            HttpContext.Current.Session.Clear();
        }

        public static void Login(string username)
        {
            Username = username;
        }
    }
}