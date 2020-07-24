using Autofac;
using ContactlessEntry.UwpFront.Services.Connectivity;
using ContactlessEntry.UwpFront.ViewModels;
using System;

namespace ContactlessEntry.UwpFront.Services
{
    public sealed class Locator
    {
        private IContainer _container;

        public static Locator Instance { get; } = new Locator();

        public Locator()
        {
            Builder = new ContainerBuilder();

            Register<IConnectivityService, ConnectivityService>();

            Register<MainWindowViewModel>();
        }

        public ContainerBuilder Builder { get; private set; }

        public void Build()
        {
            _container = Builder.Build();
        }

        public void Register<T>() where T : class
        {
            Builder.RegisterType<T>();
        }

        public void Register<T>(T instance) where T : class
        {
            Builder.RegisterInstance(instance);
        }

        public void Register<TInterface, TImplementation>() where TImplementation : TInterface
        {
            Builder.RegisterType<TImplementation>().As<TInterface>();
        }

        public T Resolve<T>()
        {
            if (null == _container)
            {
                throw new InvalidOperationException("Locator has not been built.");
            }

            return _container.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            if (null == _container)
            {
                throw new InvalidOperationException("Locator has not been built.");
            }

            return _container.Resolve(type);
        }
    }
}
