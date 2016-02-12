using Hamburglar.Providers;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

namespace Hamburglar.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new WebUserIdProvider());
            app.MapSignalR();
        }
    }
}