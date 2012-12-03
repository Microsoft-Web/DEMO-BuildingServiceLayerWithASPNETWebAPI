using System.Web.Mvc;

namespace MvcSPA.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}