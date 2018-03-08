using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Container;
using System.Web.Http.Dependencies;

namespace SuperSportDataEngine.Application.WebApi.SystemApi.App_Start
{
    public class UnityDependencyResolver : UnityDependencyScope, IDependencyResolver
    {
        public UnityDependencyResolver(IUnityContainer container)
           : base(container)
        {
        }

        public IDependencyScope BeginScope()
        {
            var childContainer = Container.CreateChildContainer();

            UnityConfigurationManager.RegisterTypes(childContainer, Application.Container.Enums.ApplicationScope.WebApiSystemApi);

            return new UnityDependencyScope(childContainer);
        }
    }
}