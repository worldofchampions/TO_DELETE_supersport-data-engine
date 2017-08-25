using Hangfire;
using Microsoft.Practices.Unity;
using System;

public class ContainerJobActivator : JobActivator
{
    private IUnityContainer _container;

    public ContainerJobActivator(IUnityContainer container)
    {
        _container = container;
    }

    public override object ActivateJob(Type type)
    {
        return _container.Resolve(type);
    }
}