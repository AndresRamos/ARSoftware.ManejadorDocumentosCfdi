﻿using System;
using System.Collections.Generic;
using System.Windows;
using Autofac;
using Caliburn.Micro;
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
            _container = IocContainerConfig.Configure();
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

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", key ?? service.Name));
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.Resolve(typeof(IEnumerable<>).MakeGenericType(service)) as IEnumerable<object>;
        }

        protected override void BuildUp(object instance)
        {
            _container.InjectProperties(instance);
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            await DisplayRootViewForAsync<ShellViewModel>();
        }
    }
}
