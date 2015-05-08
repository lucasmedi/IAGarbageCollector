using System.Web;
using Web.Models;

namespace Web.Helpers
{
    public class SessionHelper
    {
        public static CreatorViewModel CreatorViewModel
        {
            get
            {
                return (CreatorViewModel)HttpContext.Current.Session["APP_CREATOR"];
            }
            set
            {
                HttpContext.Current.Session["APP_CREATOR"] = value;
            }
        }
    }
}