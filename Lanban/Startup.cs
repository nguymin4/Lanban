using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Lanban.Startup))]

namespace Lanban
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application
            app.MapSignalR();
        }
    }
}
