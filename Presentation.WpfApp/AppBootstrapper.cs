using System;
using System.Collections.Generic;
using System.Windows;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Caliburn.Micro;
using Core.Application.Common;
using Infrastructure.Common;
using Infrastructure.Persistance.Common;
using Microsoft.Extensions.DependencyInjection;
using Presentation.WpfApp.Config;
using Presentation.WpfApp.ViewModels;

namespace Presentation.WpfApp
{
    public class AppBootstrapper : BootstrapperBase
    {
        private IContainer _container;

        public AppBootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddApplicationServices();
            serviceCollection.AddInfrastructureServices();
            serviceCollection.AddPersistenceServices();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(serviceCollection);
            containerBuilder.AddWpfAppServices();

            _container = containerBuilder.Build();
        }

        protected override object GetInstance(Type service, string key)
        {
            object instance;

            if (string.IsNullOrWhiteSpace(key))
            {
                if (_container.TryResolve(service, out instance))
                {
                    return instance;
                }
            }
            else
            {
                if (_container.TryResolveNamed(key, service, out instance))
                {
                    return instance;
                }
            }

            throw new Exception($"Could not locate any instances of contract {key ?? service.Name}.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            _container.InjectProperties(instance);
        }

        // ReSharper disable once AsyncVoidMethod
        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<ShellViewModel>();
        }
    }
}
