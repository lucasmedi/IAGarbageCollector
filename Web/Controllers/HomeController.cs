using System.Web.Mvc;
using Web.Helpers;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(WorldFactory.GetInstance());
        }

        [HttpPost]
        public ActionResult Index(ModelViewModel model)
        {
            return View(WorldFactory.NewInstance(model));
        }

        [HttpPost]
        public ActionResult NextStep()
        {
            var world = WorldFactory.GetInstance();
            world.Creator.NextAge();

            return PartialView("_World", world);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
    }
}