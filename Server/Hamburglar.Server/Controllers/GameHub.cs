using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using Hamburglar.Core;
using Hamburglar.Providers;
using System.Threading.Tasks;

namespace Hamburglar.Server.Controllers
{
    public class GameHub : Hub
    {
        ISharedCache SharedCache;
        IRealTimeMessaging RealTimeMessaging;
        IObjectPersistence ObjectPersistence;
        SynchController Synchronizer;

        public GameHub()
        {
            ObjectPersistence = new DefaultObjectStoreProvider();
            SharedCache = new DefaultSharedCacheProvider();
            RealTimeMessaging = new RealTimeMessagingProvider<GameHub>();
            Synchronizer = new SynchController(SharedCache, ObjectPersistence, new DefaultRelationalStoreProvider());
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }
        public void JoinGame(string gameId)
        {
            Groups.Add(Context.ConnectionId, gameId);
        }
        public WebGameTransport EnterRoom(string gameId, string playerId, int floor, int room)
        {
            return SetRoomOccupied(gameId, playerId, floor, room, true);
        }
        public WebGameTransport ExitRoom(string gameId, string playerId, int floor, int room)
        {
            return SetRoomOccupied(gameId, playerId, floor, room, false);
        }
        public void Loot(string gameId, string playerId, int floor, int room, int itemIndex, int trapId)
        {
            var game = GetGame(gameId);
            var versions = GetVersions();
            var lootResult = game.ClearRoomLoot(playerId, floor, room, itemIndex);
            if (!lootResult.IsNothing)
            {
                if (lootResult.WasTrapped)
                {
                    RealTimeMessaging.OnPlayerTrapped(game, playerId, lootResult.TrapOpponentId);
                }
            }
            if (trapId > 0)
            {
                game.SetRoomLootTrap(playerId, floor, room, itemIndex, trapId);
            }

            game.CheckForWinner();
            SharedCache.SetGame(game);
            RealTimeMessaging.NeedUpdate(game);

            Synchronizer.PerformSynch(game.Winner != null);
        }
        public WebGameTransport GameUpdate(int playerVersion, int floorVersion, int gameVersion, string gameId, string playerId, int floor)
        {
            Synchronizer.PerformSynch();
            var game = GetGame(gameId);
            var model = WebGameTransport.CreateSuccess();
            model.id = gameId;
            model = WebGameTransport.CheckGameVersion(game, model, floor, playerId, gameVersion, floorVersion, playerVersion);
            return model;
        }

        private WebGameTransport SetRoomOccupied(string gameId, string playerId, int floor, int room, bool isOccupying)
        {
            Synchronizer.PerformSynch();
            var model = WebGameTransport.CreateSuccess();
            var versions = GetVersions();
            var game = GetGame(gameId);
            if (isOccupying)
            {
                var occupant = game.GetRoomOccupant(floor, room);
                if (!string.IsNullOrEmpty(occupant) && !playerId.Equals(occupant, StringComparison.OrdinalIgnoreCase))
                {
                    var occ = game.GetLocalPlayer(occupant);
                    if (occ != null)
                    {
                        model.roomOccupant = PlayerMetaData.FromPlayer(occ);
                    }
                }
                var result = game.SetRoomOccupant(playerId, floor, room);
                SharedCache.SetGame(game);
                if (result.OpponentCaught)
                {
                    RealTimeMessaging.OpponentCaught(game, playerId, result.OpponentId);
                }
                RealTimeMessaging.RoomEntered(game, playerId);
            }
            else
            {
                game.ExitRoom(playerId, floor, room);
                SharedCache.SetGame(game);
                RealTimeMessaging.RoomExited(game, playerId);
            }
            RealTimeMessaging.NeedUpdate(game);
            return model;
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
        private ClientVersions GetVersions()
        {
            ClientVersions result = new ClientVersions();

            int.TryParse(Context.QueryString["g"], out result.Game);
            int.TryParse(Context.QueryString["f"], out result.Floor);
            int.TryParse(Context.QueryString["p"], out result.Player);

            return result;
        }

        private class ClientVersions
        {
            public int Game;
            public int Floor;
            public int Player;
        }
    }
}