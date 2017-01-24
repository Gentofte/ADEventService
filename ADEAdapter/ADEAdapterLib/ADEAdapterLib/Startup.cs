using Owin;
using System.Web.Http;

using Microsoft.Practices.Unity;

using GK.AppCore.Unity;

namespace ADEAdapterLib
{
    // ================================================================================
    public class Startup
    {
        // This method is required by Katana:
        // -----------------------------------------------------------------------------
        public void Configuration(IAppBuilder app)
        {
            var webApiConfiguration = ConfigureWebApi();

            // Use the extension method provided by the WebApi.Owin library:
            app.UseWebApi(webApiConfiguration);
        }

        // -----------------------------------------------------------------------------
        private HttpConfiguration ConfigureWebApi()
        {
            var config = new HttpConfiguration();

            config.DependencyResolver = new UnityDependencyResolver(UnityConfig.GetConfiguredContainer());

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            return config;
        }
    }
}
