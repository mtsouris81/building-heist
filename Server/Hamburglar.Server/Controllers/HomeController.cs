using Hamburglar.Core;
using Hamburglar.Providers;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace Hamburglar.Server.Controllers
{
    public class HomeController : Controller
    {
        ISharedCache SharedCache;
        IRealTimeMessaging RealTimeMessaging;
        IRelationalPersistence RelationalPersistence;
        IObjectPersistence ObjectPersistence;
        SynchController Synchronizer;

        public HomeController()
        {
            RelationalPersistence = new DefaultRelationalStoreProvider();
            ObjectPersistence = new DefaultObjectStoreProvider();
            SharedCache = new DefaultSharedCacheProvider();
            RealTimeMessaging = new RealTimeMessagingProvider<GameHub>();
            Synchronizer = new SynchController(SharedCache, ObjectPersistence, RelationalPersistence);
        }

        public ActionResult Create(string playerId, int floors, int rooms, string title)
        {
            string player_names = Request.QueryString["p"];
            if (player_names == null)
            {
                player_names = string.Empty;
            }
            var game = RelationalPersistence.CreateGame(
                                                    playerId,
                                                    title,
                                                    floors,
                                                    rooms,
                                                    player_names
                                                        .Split(
                                                            new char[] { ' ', ',' },
                                                            StringSplitOptions.RemoveEmptyEntries));
            ObjectPersistence.SetGame(game);
            SharedCache.SetGame(game);
            RealTimeMessaging.OnGameCreated(game);

            var model = WebGameTransport.CreateSuccess();
            model.id = game.Id;
            model = WebGameTransport.CheckGameVersion(game, model, null, playerId, 0, 0, 0);
            model.isFullGame = true;
            return JsonData(model);
        }
        public ActionResult Game(string gameId, string playerId)
        {
            var game = GetGame(gameId);
            var model = WebGameTransport.CreateSuccess();
            model.id = gameId;
            model = WebGameTransport.CheckGameVersion(game, model, null, playerId, 0, 0, 0);
            model.isFullGame = true;
            return JsonData(model);
        }
        public ActionResult Games(string playerId)
        {
            var games = RelationalPersistence.GetGames(playerId);
            var model = new GameListResult(games);
            return JsonData(model);
        }
        public ActionResult Login(string username)
        {
            Synchronizer.PerformSynch();
            string pw = Request.QueryString["password"];
            var player = RelationalPersistence.GetPlayerByUsername(username);
            if (player != null && player.AuthToken != null && player.AuthToken.Equals(pw, StringComparison.OrdinalIgnoreCase))
            {
                return JsonData(player.Id);
            }
            return Http401();
        }
        public ActionResult SignUp(string username)
        {
            string pw = Request.QueryString["password"];
            string image = Request.QueryString["imageurl"];
            var player = RelationalPersistence.CreatePlayer(username, Guid.NewGuid().ToString("N"), image, pw);
            if (player != null)
            {
                ObjectPersistence.SetPlayer(player);
                SharedCache.SetPlayer(player);
                return JsonData(player);
            }
            return HttpError("User could not be created. Try using a different username");
        }

        private Core.Game GetGame(string gameId)
        {
            var game = SharedCache.GetGame(gameId);
            if (game == null)
            {
                game = ObjectPersistence.GetGame(gameId);
                if (game != null)
                {
                    SharedCache.SetGame(game);
                }
            }
            return game;
        }
        private ActionResult Http401()
        {
            return new HttpUnauthorizedResult();
        }
        private ActionResult HttpError(string message)
        {
            return new HttpStatusCodeResult(500, message);
        }
        private ActionResult JsonData<T>(T model)
        {
            string json = JsonConvert.SerializeObject(model);
            return Content(json, "text/json");
        }


        // FRIEND STUFF
        public ActionResult Friends(string playerId)
        {
            var friends = RelationalPersistence.GetFriendsForPlayer(playerId);
            var friendRequests = RelationalPersistence.GetFriendRequests(playerId);
            var pendingFriends = RelationalPersistence.GetPendingFriends(playerId);
            var model = new FriendListResult(friends, friendRequests, pendingFriends);
            return JsonData(model);
        }
        public ActionResult SearchPlayers(string search)
        {
            var players = RelationalPersistence.SearchPlayers(search);
            var model = new PlayerListResult(players);
            return JsonData(model);
        }
        public ActionResult PendingFriends(string playerId)
        {
            var players = RelationalPersistence.GetPendingFriends(playerId);
            var model = new PlayerListResult(players);
            return JsonData(model);
        }
        public ActionResult GetGamesForPlayers(string playerId, string otherPlayerId)
        {
            var games = RelationalPersistence.GetGamesForPairOfPlayers(playerId, otherPlayerId);
            var model = new GameListResult(games);
            return JsonData(model);
        }
        public ActionResult RequestFriend(string playerId, string friendId)
        {
            if (RelationalPersistence.IsPendingFriend(playerId, friendId))
            {
                // already a requested friend, create confirmed friendship
                RelationalPersistence.AddFriend(playerId, friendId);
                RelationalPersistence.DeletePendingFriend(friendId, playerId);
                var player = RelationalPersistence.GetPlayerById(friendId);
                return JsonData(new BaseTransport()
                {
                    s = true,
                    m = $"You are now friends with {player.Username}!"
                });
            }
            else
            {
                RelationalPersistence.AddPendingFriend(playerId, friendId);
                return JsonData(new BaseTransport()
                {
                    s = true,
                    m = "Friend request sent!"
                });
            }
        }
        public ActionResult RejectFriendRequest(string playerId, string friendId)
        {
            RelationalPersistence.DeletePendingFriend(friendId, playerId);
            return JsonData(new BaseTransport() { s = true });
        }
        public ActionResult DeleteFriend(string playerId, string friendId)
        {
            RelationalPersistence.RemoveFriend(friendId, playerId);
            return JsonData(new BaseTransport() { s = true });
        }
        public ActionResult AcceptFriend(string playerId, string friendId)
        {
            RelationalPersistence.AddFriend(playerId, friendId);
            RelationalPersistence.DeletePendingFriend(friendId, playerId);
            var player = RelationalPersistence.GetPlayerById(friendId);
            return JsonData(new BaseTransport()
            {
                s = true,
                m = $"You are now friends with {player.Username}!"
            });
        }
        
        
        //string[] usernames = 
        //{
        //    "Henry",
        //    "Bob",
        //    "Brad",
        //    "Norm",
        //    "Joe",
        //    "Bill",
        //    "Brian",
        //    "Eugene",
        //    "Harold",
        //    "Reggie",
        //    "Bert",
        //    "Bernard"
        //};
        //public void CreateUsers(string[] usernames)
        //{
        //    foreach(var u in usernames)
        //    {
        //        RelationalPersistence.CreatePlayer(u, Guid.NewGuid().ToString("N"), "", "1");
        //    }
        //}
        
    }
}
