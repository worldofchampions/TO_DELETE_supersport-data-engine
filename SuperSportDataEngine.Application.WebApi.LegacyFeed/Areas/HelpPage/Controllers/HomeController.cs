using System.Threading.Tasks;
using System.Web.Mvc;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed.Areas.HelpPage.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Content("");
        }
    }
}