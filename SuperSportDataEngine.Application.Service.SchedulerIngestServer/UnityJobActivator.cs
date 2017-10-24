using Microsoft.Practices.Unity;
using SuperSportDataEngine.Application.Container;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hangfire
{
    /// <summary>
    /// A job activator which use unity to create job instance
    /// </summary>
    public class UnityJobActivator : JobActivator
    {
        private readonly IUnityContainer container;

        /// <summary>
        /// Initialize a new instance of the <see cref="T:UnityJobActivator"/> class
        /// </summary>
        /// <param name="container">The unity container to be used</param>
        public UnityJobActivator(IUnityContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            this.container = container;
        }

        /// <inheritdoc />
        public override object ActivateJob(Type jobType)
        {
            return this.container.Resolve(jobType);
        }

        public override JobActivatorScope BeginScope()
        {
            // The types in the container should be registered 
            // in the child container and not globally.
            var childContainer = container.CreateChildContainer();
            UnityConfigurationManager.RegisterTypes(childContainer, SuperSportDataEngine.Application.Container.Enums.ApplicationScope.ServiceInboundIngestServer);

            return new UnityScope(childContainer);
        }

        class UnityScope : JobActivatorScope
        {
            private readonly IUnityContainer container;

            public UnityScope(IUnityContainer container)
            {
                this.container = container;
            }

            public override object Resolve(Type type)
            {
                return container.Resolve(type);
            }

            public override void DisposeScope()
            {
                container.Dispose();
            }
        }
    }
}
