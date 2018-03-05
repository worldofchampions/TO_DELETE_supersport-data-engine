using SuperSportDataEngine.Application.WebApi.SystemApi.Authentication;
using System.Web;
using System.Web.Mvc;

namespace SuperSportDataEngine.Application.WebApi.SystemApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
