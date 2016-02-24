
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public interface IRelationalPersistence
    {
        void SetGameFinished(string gameId, string winnerId);
        Game CreateGame(string playerid, string title, int floors, int roomsPerFloor, string[] playerNames);
        Player GetPlayerById(string id);
        IEnumerable<Game> GetGames(string playerId);
        Player GetPlayerByUsername(string username);
        Player CreatePlayer(string username, string id, string image, string pw);
    }

}
