using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Hamburglar
{
	public static class UrlResolver
	{
        const string protocol = "http://";

        public static string Host { get; set; }

        public static string BaseURL { get { return protocol + Host + "/api/"; } }
        public static string RealTimeURL { get { return protocol + Host; } }
        public static string RealTimeHub = "GameHub";

        public static string Login(string username,string password)
        {
            return string.Format("{0}login/{1}?password={2}", BaseURL, username, password);
        }
        public static string CreateUser(string username, string password, int imageUrl)
        {
            return string.Format("{0}signup/{1}?password={2}&imageurl={3}", BaseURL, username, password, imageUrl);
        }
        public static string GameList()
        {
            return string.Format("{0}games/{1}", BaseURL, HamburglarContext.Instance.PlayerId);
        }
        public static string Game(string gameId)
        {
            return string.Format("{0}game/{1}/{2}", BaseURL, gameId, HamburglarContext.Instance.PlayerId);
        }
        public static string CreateGame(string title, string playerNames, int floors, int rooms)
        {
            return string.Format("{0}creategame/{1}/{2}/{3}/{4}?p={5}", 
                BaseURL, 
                HamburglarContext.Instance.PlayerId,
                floors,
                rooms,
                UrlSanitize(title),
                UrlSanitize(playerNames),
                floors, 
                rooms);
        }

        public static string Loot(string gameId, int floor, int room, int index, int trapId)
        {
            return string.Format("{0}loot/{1}/{2}/{3}/{4}/{5}/{6}?{7}",
                BaseURL,
                HamburglarContext.Instance.BuildingData.Id,
                HamburglarContext.Instance.PlayerId,
                floor,
                room,
                index,
                trapId,
                GetVersionQueryString());
        }
        public static string EnterRoom(int floor, int room)
        {
            return string.Format("{0}enterroom/{1}/{2}/{3}/{4}?{5}",
                BaseURL,
                HamburglarContext.Instance.BuildingData.Id,
                HamburglarContext.Instance.PlayerId,
                floor,
                room,
                GetVersionQueryString());
        }
        public static string ExitRoom(int floor, int room)
        {
            return string.Format("{0}exitroom/{1}/{2}/{3}/{4}?{5}",
                BaseURL,
                HamburglarContext.Instance.BuildingData.Id,
                HamburglarContext.Instance.PlayerId,
                floor,
                room,
                GetVersionQueryString());
        }
        public static string CheckForMessages()
        {
            return string.Format("{0}messages/{1}/{2}/{3}",
                BaseURL,
                HamburglarContext.Instance.BuildingData.Id,
                HamburglarContext.Instance.PlayerId,
                HamburglarContext.Instance.HighestMessageReceived);
        }
        public static string GetGameUpdates()
        {
            return string.Format("{0}update/{1}/{2}/{3}?{4}",
                BaseURL,
                HamburglarContext.Instance.BuildingData.Id,
                HamburglarContext.Instance.PlayerId,
                HamburglarContext.Instance.Floor,
                GetVersionQueryString());
        }


        public static string SearchPlayers(string search)
        {
            return string.Format("{0}players/{1}",
                BaseURL,
                search);
        }
        public static string RequestFriend(string friendId)
        {
            return string.Format("{0}friends/{1}/request/{2}",
                BaseURL,
                HamburglarContext.Instance.PlayerId,
                friendId);
        }
        public static string RejectFriend(string friendId)
        {
            return string.Format("{0}friends/{1}/reject/{2}",
                BaseURL,
                HamburglarContext.Instance.PlayerId,
                friendId);
        }
        public static string RejectFriend(string requestingFriendId, string playerId)
        {
            return string.Format("{0}friends/{1}/reject/{2}",
                BaseURL,
                playerId,
                requestingFriendId);
        }
        public static string DeleteFriend(string friendId)
        {
            return string.Format("{0}friends/{1}/delete/{2}",
                BaseURL,
                HamburglarContext.Instance.PlayerId,
                friendId);
        }
        public static string AcceptFriend(string friendId)
        {
            return string.Format("{0}friends/{1}/accept/{2}",
                BaseURL,
                HamburglarContext.Instance.PlayerId,
                friendId);
        }
        public static string GetPendingFriends()
        {
            return string.Format("{0}friends/{1}/pending",
                BaseURL,
                HamburglarContext.Instance.PlayerId);
        }
        public static string GetFriends()
        {
            return string.Format("{0}friends/{1}",
                BaseURL,
                HamburglarContext.Instance.PlayerId);
        }
        public static string GetGamesInCommonWithFriend(string friendId)
        {
            return string.Format("{0}friends/{1}/games/{2}",
                BaseURL,
                HamburglarContext.Instance.PlayerId,
                friendId);
        }




        private static string GetVersionQueryString()
        {
            return string.Format("g={0}&p={1}&f={2}",
                                    HamburglarContext.Instance.BuildingData.GameMetaVersion,
                                    HamburglarContext.Instance.BuildingData.PlayerVersions[HamburglarContext.Instance.PlayerId],
                                    HamburglarContext.Instance.BuildingData.FloorVersions[HamburglarContext.Instance.Floor]);

        }
        public static ClientVersions GetVersions()
        {
            return new ClientVersions()
            {
                Game = HamburglarContext.Instance.BuildingData.GameMetaVersion - 1,
                Player = HamburglarContext.Instance.BuildingData.PlayerVersions[HamburglarContext.Instance.PlayerId] - 1,
                Floor = HamburglarContext.Instance.BuildingData.FloorVersions[HamburglarContext.Instance.Floor] - 1
            };
        }
        public class ClientVersions{
            public int Player { get; set; }
public int Game { get; set; }
public int Floor { get; set; }

        }
        
        private static string UrlSanitize(string source)
        {
            if (source == null)
                return string.Empty;

            foreach (var r in remove)
            {
                source = source.Replace(r, string.Empty);
            }
            return source.Replace(" ", "%20").Replace("&", "and");
        }
        
        static string[] remove = new string[] { "<", ">", "*", "%", ":", "\\", "?"  };

    }
}
