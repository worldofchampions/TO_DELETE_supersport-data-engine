using System.Web;
using System.Web.Mvc;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
