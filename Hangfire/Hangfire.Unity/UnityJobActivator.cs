using SuperSportDataEngine.Application.Container;
using System;
using Unity;

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
            // This is so that Services that are registered with Hierachical lifetime,
            // Are registered with the correct container.
            var childContainer = container.CreateChildContainer();
            UnityConfigurationManager.RegisterTypes(childContainer, SuperSportDataEngine.Application.Container.Enums.ApplicationScope.ServiceSchedulerIngestServer);

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
