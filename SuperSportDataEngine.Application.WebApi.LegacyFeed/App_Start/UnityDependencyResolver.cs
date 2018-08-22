using System.Web.Http.Dependencies;
using SuperSportDataEngine.Application.Container;
using Unity;

namespace SuperSportDataEngine.Application.WebApi.LegacyFeed
{
    public class UnityDependencyResolver: UnityDependencyScope, IDependencyResolver
    {
        public UnityDependencyResolver(IUnityContainer container)
           : base(container)
        {
        }

        public IDependencyScope BeginScope()
        {
            var childContainer = Container.CreateChildContainer();

            UnityConfigurationManager.RegisterTypes(childContainer, Application.Container.Enums.ApplicationScope.WebApiLegacyFeed);

            return new UnityDependencyScope(childContainer);
        }
    }
}