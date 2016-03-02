using Hamburglar.Providers;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;
using System.Configuration;

namespace Hamburglar.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var redisConnection = ConfigurationManager.ConnectionStrings["Redis"];

            if (IsBackpaneEnabled())
            {
                int port = 6379;
                int.TryParse(ConfigurationManager.AppSettings["heist:BackpanePort"], out port);
                GlobalHost.DependencyResolver.UseRedis(
                    ConfigurationManager.AppSettings["heist:BackpaneHost"],
                    port,
                    ConfigurationManager.AppSettings["heist:BackpanePassword"],
                    "heist");
            }

            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new WebUserIdProvider());
            app.MapSignalR();
        }

        public bool IsBackpaneEnabled()
        {
            bool result = false;
            bool.TryParse(ConfigurationManager.AppSettings["heist:BackpaneEnabled"], out result);
            return result;
        }
    }
}