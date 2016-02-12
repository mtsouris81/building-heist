using Hamburglar.Core;
using Hamburglar.Providers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
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
        public ActionResult GameUpdate(string gameId, string playerId, int floor)
        {
            var versions = GetVersions(Request);
            var game = GetGame(gameId);
            var model = WebGameTransport.CreateSuccess();
            model.id = gameId;
            model = WebGameTransport.CheckGameVersion(game, model, floor, playerId, versions.Game, versions.Floor, versions.Player);
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


        public ActionResult JsonData<T>(T model)
        {
            string json = JsonConvert.SerializeObject(model);
            return Content(json, "text/json");
        }

        public ClientVersions GetVersions(HttpRequestBase request)
        {
            ClientVersions result = new ClientVersions();

            int.TryParse(request.QueryString["g"], out result.Game);
            int.TryParse(request.QueryString["f"], out result.Floor);
            int.TryParse(request.QueryString["p"], out result.Player);

            return result;
        }

        public class ClientVersions
        {
            public int Game;
            public int Floor;
            public int Player;
        }

    }

}
