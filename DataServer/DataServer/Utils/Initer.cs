using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using DataServer.BusinessLogic;
using DataServer.Repository;
using DataServer.Utils;

namespace DataServer
{
    /// <summary>
    /// Initializes service-wide references.
    /// </summary>
    public static class Initer
    {
        private static IContainer Container { get; set; }

        public static void Init(string connectionString)
        {
            EventStoreFactory.Init(connectionString);
            InitializeDependencyInjection();
        }

        public static void InitializeDependencyInjection()
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<FullNameProjector>().As<IFullNameProjector>();
            builder.RegisterType<FullNameRepository>().As<IFullNameRepository>();
            builder.RegisterType<CommandFullName>().As<ICommandFullName>();
            Container = builder.Build();

            var config = GlobalConfiguration.Configuration;
            config.DependencyResolver = new AutofacWebApiDependencyResolver(Container);
        }
    }
}