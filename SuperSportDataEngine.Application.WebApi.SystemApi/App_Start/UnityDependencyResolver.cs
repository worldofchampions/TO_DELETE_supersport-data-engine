using System.Web.Http.Dependencies;
using SuperSportDataEngine.Application.Container;
using Unity;

namespace SuperSportDataEngine.Application.WebApi.SystemApi
{
    /// <summary>
    /// 
    /// </summary>
    public class UnityDependencyResolver : UnityDependencyScope, IDependencyResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public UnityDependencyResolver(IUnityContainer container)
           : base(container)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDependencyScope BeginScope()
        {
            var childContainer = Container.CreateChildContainer();

            UnityConfigurationManager.RegisterTypes(childContainer, Application.Container.Enums.ApplicationScope.WebApiSystemApi);

            return new UnityDependencyScope(childContainer);
        }
    }
}