using System.Reflection;
using API.Modules;
using Microsoft.Extensions.Configuration;
using Autofac;

namespace API;

public class Program
{
    private static IConfiguration _configuration;
    private static IContainer _container;

    public static void Main()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var builder = new ContainerBuilder();

        builder.RegisterModule(new DatabaseModule(_configuration));
        
        _container = builder.Build();
    }
}

