using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UrlsAndRoutes.Startup))]
namespace UrlsAndRoutes
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
