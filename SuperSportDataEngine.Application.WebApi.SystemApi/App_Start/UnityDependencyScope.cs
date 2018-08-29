using System;
using System.Collections.Generic;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using Unity;

namespace SuperSportDataEngine.Application.WebApi.SystemApi
{
    /// <summary>
    /// 
    /// </summary>
    public class UnityDependencyScope : IDependencyScope
    {
        /// <summary>
        /// 
        /// </summary>
        protected IUnityContainer Container { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public UnityDependencyScope(IUnityContainer container)
        {
            Container = container;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType)
        {
            if (typeof(IHttpController).IsAssignableFrom(serviceType))
            {
                return Container.Resolve(serviceType);
            }

            try
            {
                return Container.Resolve(serviceType);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Container.ResolveAll(serviceType);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Container.Dispose();
        }
    }
}