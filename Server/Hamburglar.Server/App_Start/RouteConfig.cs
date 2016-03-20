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
                "loot/{gameId}/{playerId}/{floor}/{room}/{itemIndex}/{trapId}");

            routes.MapApi(
                "EnterRoom",
                "enterroom/{gameId}/{playerId}/{floor}/{room}");

            routes.MapApi(
                "ExitRoom",
                "exitroom/{gameId}/{playerId}/{floor}/{room}");

            routes.MapApi(
                "Create",
                "creategame/{playerid}/{floors}/{rooms}/{title}");

            routes.MapApi(
                "GameUpdate",
                "update/{gameId}/{playerId}/{floor}");

            routes.MapApi(
                "CheckForMessages",
                "messages/{gameId}/{playerId}/{highestMessage}");

            routes.MapApi(
                "Game",
                "game/{gameId}/{playerId}");

            routes.MapApi(
                "Games",
                "games/{playerId}");

            routes.MapApi(
                "Login",
                "Login/{username}");

            routes.MapApi(
                "SignUp",
                "signup/{username}");

            routes.MapApi(
                "SearchPlayers",
                "players/{search}");


            routes.MapApi(
                "RequestFriend",
                "friends/{playerId}/request/{friendId}");

            routes.MapApi(
                "RejectFriendRequest",
                "friends/{playerId}/reject/{friendId}");

            routes.MapApi(
                "DeleteFriend",
                "friends/{playerId}/delete/{friendId}");

            routes.MapApi(
                "AcceptFriend",
                "friends/{playerId}/accept/{friendId}");

            routes.MapApi(
                "GetGamesForPlayers",
                "friends/{playerId}/games/{otherPlayerId}");

            routes.MapApi(
                "PendingFriends",
                "friends/{playerId}/pending");

            routes.MapApi(
                "Friends",
                "friends/{playerId}");
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
                url: $"api/{url}",
                defaults: new { controller = "Home", action = actionName }
            );
        }
    }
}
