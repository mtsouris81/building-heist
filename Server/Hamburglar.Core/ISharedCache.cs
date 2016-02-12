using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public interface ISharedCache
    {
        Game GetGame(string gameId);
        Player GetPlayerById(string id);
        void SetPlayer(Player player);
        void SetGame(Game game);
        List<Game> GetGames();
    }
    public interface IRealTimeMessaging
    {
        void OnGameCreated(Game game);
        void RoomEntered(Game game, string playerId);
        void RoomExited(Game game, string playerId);
        void OnPlayerTrapped(Game game, string playerId, string trapOpponentId);
        void OpponentCaught(Game game, string playerId, string opponentId);
        void NeedUpdate(Game game);
        void GameFinished(Game game);
    }
    public interface IRelationalPersistence
    {
        void SetGameFinished(string gameId, string winnerId);
        Game CreateGame(string playerid, string title, int floors, int roomsPerFloor, string[] playerNames);
        Player GetPlayerById(string id);
        IEnumerable<Game> GetGames(string playerId);
        Player GetPlayerByUsername(string username);
        Player CreatePlayer(string username, string id, string image, string pw);
    }
    public interface IObjectPersistence
    {
        Game GetGame(string gameId);
        Player GetPlayerById(string id);
        void SetPlayer(Player player);
        void SetGame(Game game);
    }
}
