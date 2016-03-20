using Hamburglar.Core;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamburglar.Providers
{
    public class RealTimeMessagingProvider<THub> : IRealTimeMessaging where THub : IHub
    {
        public void NeedUpdate(Core.Game game)
        {
            var clients = GetClientsContext();
            clients.Group(game.Id).NeedUpdate();
        }
        public void OnGameCreated(Core.Game game)
        {
            var clients = GetClientsContext();
            clients.Users(game.PlayerIds).OnGameCreated();
        }
        public void GameFinished(Core.Game game)
        {
            var clients = GetClientsContext();
            clients.Group(game.Id).GameFinished(game.Winner);
        }
        public void OnPlayerTrapped(Core.Game game, string playerId, string trapOpponentId)
        {
            var clients = GetClientsContext();
            clients.Group(game.Id).OnPlayerTrapped(playerId, trapOpponentId);
        }
        public void OpponentCaught(Core.Game game, string playerId, string opponentId)
        {
            var clients = GetClientsContext();
            clients.Group(game.Id).OpponentCaught(playerId, opponentId);
        }
        public void RoomEntered(Core.Game game, string playerId)
        {
            var clients = GetClientsContext();
            var player = game.GetLocalPlayer(playerId);
            clients.Group(game.Id).RoomEntered(player.Floor, player.Room, playerId);
        }
        public void RoomExited(Core.Game game, string playerId)
        {
            var clients = GetClientsContext();
            var player = game.GetLocalPlayer(playerId);
            clients.Group(game.Id).RoomExited(player.Floor, player.Room, playerId);
        }
        public void OnGameReady(Core.Game game, DateTime utcNow)
        {
            var clients = GetClientsContext();
            clients.Group(game.Id).GameReady(utcNow);
        }
        private IHubConnectionContext<dynamic> GetClientsContext()
        {
            return GlobalHost.ConnectionManager.GetHubContext<THub>().Clients;
        }

    }
}
