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
            GlobalHost.DependencyResolver.UseRedis(
                                        new RedisScaleoutConfiguration(
                                            redisConnection.ConnectionString,
                                            redisConnection.ProviderName));

            GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new WebUserIdProvider());
            app.MapSignalR();
        }
    }
}