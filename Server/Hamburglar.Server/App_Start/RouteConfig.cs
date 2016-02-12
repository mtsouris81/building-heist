using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Hamburglar.Server
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapApi(
                "Loot",
                "api/loot/{gameId}/{playerId}/{floor}/{room}/{itemIndex}/{trapId}");

            routes.MapApi(
                "EnterRoom",
                "api/enterroom/{gameId}/{playerId}/{floor}/{room}");

            routes.MapApi(
                "ExitRoom",
                "api/exitroom/{gameId}/{playerId}/{floor}/{room}");

            routes.MapApi(
                "Create",
                "api/creategame/{playerid}/{floors}/{rooms}/{title}");

            routes.MapApi(
                "GameUpdate",
                "api/update/{gameId}/{playerId}/{floor}");

            routes.MapApi(
                "CheckForMessages",
                "api/messages/{gameId}/{playerId}/{highestMessage}");

            routes.MapApi(
                "Game",
                "api/game/{gameId}/{playerId}");

            routes.MapApi(
                "Games",
                "api/games/{playerId}");

            routes.MapApi(
                "Login",
                "api/Login/{username}");

            routes.MapApi(
                "SignUp",
                "api/signup/{username}");
        }


    }
    public static class _routeExtensions
    {
        private static int mapNumber = 0;
        public static void MapApi(this RouteCollection routes, string actionName, string url)
        {
            mapNumber++;
            routes.MapRoute(
                name: "Api" + mapNumber.ToString(),
                url: url,
                defaults: new { controller = "Home", action = actionName }
            );
        }
    }
}
