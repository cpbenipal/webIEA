using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace webIEA
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //DevExtremeBundleConfig.RegisterBundles(BundleTable.Bundles);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Flexpage.Area.FlexpageBundleConfig.RegisterBundles(BundleTable.Bundles);

            // Add dependencies resolver
            IKernel kernel = new StandardKernel(new NinjectSettings { AllowNullInjection = true });

            var resolver = new NinjectDependencyResolver(kernel);
            GlobalConfiguration.Configuration.DependencyResolver = resolver;
            DependencyResolver.SetResolver(resolver);

            ControllerBuilder.Current.SetControllerFactory(new Flexpage.Infrastructure.FlexpageControllerFactory());
            ModelBinders.Binders.DefaultBinder = new Flexpage.Infrastructure.FlexpageModelBuilder();

            // start DB migrations
            new Flexpage.Domain.Context.FlexpageRepository().MigrateDB();

        }
    }
}
