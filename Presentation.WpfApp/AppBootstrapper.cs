using System.Windows;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Caliburn.Micro;
using Core.Application.Common;
using Infrastructure.Common;
using Infrastructure.Persistance.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.WpfApp.Config;
using Presentation.WpfApp.ViewModels;

namespace Presentation.WpfApp;

public class AppBootstrapper : BootstrapperBase
{
    private readonly IHost _host;

    public AppBootstrapper()
    {
        _host = Host.CreateDefaultBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureServices((context, serviceCollection) =>
            {
                serviceCollection.AddApplicationServices();
                serviceCollection.AddInfrastructureServices();
                serviceCollection.AddPersistenceServices(context.Configuration);
            })
            .ConfigureContainer<ContainerBuilder>(containerBuilder => { containerBuilder.AddWpfAppServices(); })
            .Build();
        Initialize();
    }

    protected override object GetInstance(Type service, string key)
    {
        return _host.Services.GetService(service);
    }

    protected override IEnumerable<object> GetAllInstances(Type service)
    {
        return _host.Services.GetServices(service);
    }

    // ReSharper disable once AsyncVoidMethod
    protected override async void OnStartup(object sender, StartupEventArgs e)
    {
        await _host.StartAsync();
        await DisplayRootViewForAsync<ShellViewModel>();
    }

    // ReSharper disable once AsyncVoidMethod
    protected override async void OnExit(object sender, EventArgs e)
    {
        await _host.StopAsync();
        base.OnExit(sender, e);
    }
}
