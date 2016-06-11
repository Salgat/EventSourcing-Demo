using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using DataReader.BusinessLogic;
using DataReader.Repository;

namespace DataReader.Utils
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
            NameRepository.Init(EventStoreFactory.GetConnection(), new NameProjector()).Wait();
        }

        public static void InitializeDependencyInjection()
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<NameProjector>().As<INameProjector>();
            builder.RegisterType<NameBusinessLogic>().As<INameBusinessLogic>();
            Container = builder.Build();

            var config = GlobalConfiguration.Configuration;
            config.DependencyResolver = new AutofacWebApiDependencyResolver(Container);
        }
    }
}