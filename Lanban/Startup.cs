using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using Lanban.Hubs;

[assembly: OwinStartup(typeof(Lanban.Startup))]

namespace Lanban
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new Hubs.UserHubIdProvider());
            app.MapSignalR();
        }
    }
}
