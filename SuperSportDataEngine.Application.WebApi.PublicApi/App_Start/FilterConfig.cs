using System.Web;
using System.Web.Mvc;

namespace SuperSportDataEngine.Application.WebApi.PublicApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
