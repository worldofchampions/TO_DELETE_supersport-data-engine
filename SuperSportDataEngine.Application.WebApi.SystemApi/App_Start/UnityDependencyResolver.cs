using System.Web.Http.Dependencies;
using SuperSportDataEngine.Application.Container;
using Unity;

namespace SuperSportDataEngine.Application.WebApi.SystemApi
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