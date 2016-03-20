
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


        IEnumerable<Player> GetFriendsForPlayer(string playerId);
        IEnumerable<Player> SearchPlayers(string playerNamePattern);
        void AddPendingFriend(string requestingPlayerId, string playerId);
        void AddFriend(string playerId_1, string playerId_2);
        void RemoveFriend(string playerId_1, string playerId_2);
        void DeletePendingFriend(string requestingPlayerId, string playerId);
        bool IsPendingFriend(string playerId, string friendId);
        IEnumerable<Player> GetPendingFriends(string playerId);
        IEnumerable<Player> GetFriendRequests(string playerId);
        IEnumerable<Game> GetGamesForPairOfPlayers(string requestingPlayerId, string playerId);

    }

}
