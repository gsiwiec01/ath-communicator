using API.Helpers;
using ATH.DAL.DataAccess;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace API.Modules;

public class DatabaseModule : Module
{
    private readonly IConfiguration _configuration;

    public DatabaseModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(x =>
        {
            var options = new DbContextOptionsBuilder();

            var connectionString = _configuration.GetConnectionString(AppSettingsKeys.GameConnectionString);
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

            return new DatabaseContext(options.Options);
        }).InstancePerLifetimeScope().AsSelf();
    }
}