using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using DataReader.Repository;
using DataReader.Utils;

namespace DataReader
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Initer.Init(ConfigurationManager.ConnectionStrings["EventStore"].ConnectionString);
        }
    }
}
