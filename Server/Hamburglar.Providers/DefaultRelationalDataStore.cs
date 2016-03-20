using Hamburglar.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamburglar.Providers
{
    public class DefaultRelationalStoreProvider : IRelationalPersistence
    {
        BuildingHeistEntities GetDb()
        {
            return new BuildingHeistEntities();
        }

        public IEnumerable<Core.Game> GetGames(string playerId)
        {
            using (var db = GetDb())
            {
                var player = db.Players.Where(p => p.Id.Equals(playerId, StringComparison.OrdinalIgnoreCase)).First();
                return player.Games.Where(g => g.Winner == null).Select(g => new Core.Game()
                        {
                            Id = g.Id,
                            Title = g.Title
                        })
                        .ToList();
            }
        }
        public Core.Game CreateGame(string playerid, string title, int floors, int roomsPerFloor, string[] playerNames)
        {
            // validate all players
            var owner = GetPlayerById(playerid);
            List<Core.Player> otherPlayers = new List<Core.Player>();
            for (int i = 0; i < playerNames.Length; i++)
            {
                var p = GetPlayerByUsername(playerNames[i]);
                if (p != null)
                {
                    otherPlayers.Add(p);
                }
            }
            // create actual game and generate floors
            Core.Game game = Core.Game.Create(owner, title, floors, roomsPerFloor, otherPlayers);
            // save to database
            using (var db = GetDb())
            {
                Game g = new Game()
                {
                    Id = game.Id,
                    Title = game.Title
                };
                db.Games.Add(g);
                db.SaveChanges();
                foreach(var p in game.PlayerIds)
                {
                    g.Players.Add(db.Players.Where(x => x.Id.Equals(p, StringComparison.OrdinalIgnoreCase)).First());
                }
                db.SaveChanges();
            }
            return game;
        }
        public Core.Player CreatePlayer(string username, string id, string image, string pw)
        {
            var existing = GetPlayerById(id);

            if (existing != null)
                throw new Exception("Player ID already exists");

            existing = GetPlayerByUsername(username);

            if (existing != null)
                throw new Exception("Player username already exists");

            Core.Player p = new Core.Player()
            {
                Id = id,
                ImageUrl = image,
                Username = username,
                AuthToken = pw
            };
            
            using (var db = GetDb())
            {
                db.Players.Add(new Player()
                {
                    Id = p.Id,
                    Username = p.Username,
                    Password = p.AuthToken,
                    Salt = Guid.NewGuid().ToString("N"),
                });
                db.SaveChanges();
            }
            return p;
        }
        public Core.Player GetPlayerById(string id)
        {
            using (var db = GetDb())
            {
                var p = GetDbPlayer(db, id);

                if (p == null)
                    return null;

                return Map(p);
            }
        }
        private Player GetDbPlayer(BuildingHeistEntities db, string id)
        {
            return db.Players.Where(x => x.Id.Equals(id, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        private Player[] GetDbPlayers(BuildingHeistEntities db, params string[] ids)
        {
            var dbResults = db.Players.Where(x => ids.Contains(x.Id)).ToArray();
            Player[] result = new Player[ids.Length];
            for (int i = 0; i < ids.Length; i++) // return in order they are asked for
            {
                result[i] = dbResults.Where(x => x.Id.Equals(ids[i], StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            }
            return result;
        }
        public Core.Player GetPlayerByUsername(string username)
        {
            using (var db = GetDb())
            {
                var p = db.Players.Where(x => x.Username.Equals(username, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                if (p == null)
                    return null;

                return Map(p);
            }
        }
        private Core.Player Map(Player p)
        {
            return new Core.Player()
            {
                AuthToken = p.Password,
                Id = p.Id,
                Username = p.Username,
                Name = p.Username
            };
        }
        private IEnumerable<Core.Player> Map(IEnumerable<Player> p)
        {
            List<Core.Player> result = new List<Core.Player>();
            if (p == null)
            {
                return result;
            }
            foreach(var record in p)
            {
                result.Add(Map(record));
            }
            return result;
        }
        public void SetGameFinished(string gameId, string winnerId)
        {
            using (var db = GetDb())
            {
                var game = db.Games.Where(x => x.Id == gameId).FirstOrDefault();
                if (game != null)
                {
                    game.Winner = winnerId;
                    db.SaveChanges();
                }
            }
        }



        
        public IEnumerable<Core.Player> GetFriendsForPlayer(string playerId)
        {
            using (var db = GetDb())
            {
                var player = GetDbPlayer(db, playerId);
                var dbFriendRecords = db.PlayerFriends
                    .Where(f => (f.PlayerId_1 == player.dbId || f.PlayerId_2 == player.dbId))
                    .ToList();

                List<int> friends = new List<int>();
                foreach(var f in dbFriendRecords)
                {
                    if (f.PlayerId_1 != player.dbId && !friends.Contains(f.PlayerId_1))
                        friends.Add(f.PlayerId_1);

                    if (f.PlayerId_2 != player.dbId && !friends.Contains(f.PlayerId_2))
                        friends.Add(f.PlayerId_2);
                }

                return Map(db.Players.Where(p => friends.Contains(p.dbId)));
            }
        }
        public IEnumerable<Core.Player> SearchPlayers(string playerNamePattern)
        {
            using (var db = GetDb())
            {
                Func<Player, bool> startsWith = p => p.Username.StartsWith(playerNamePattern, StringComparison.InvariantCultureIgnoreCase);
                Func<Player, bool> contains = p => p.Username.ToLower().Contains(playerNamePattern);
                Func<Player, bool> actualCheck = (playerNamePattern != null && playerNamePattern.Length >= 3)
                                                        ? contains
                                                        : startsWith;
                return Map(
                        db.Players
                            .Where(actualCheck)
                            .OrderBy(x => x.Username)
                            .ToList());
            }
        }
        public void AddPendingFriend(string requestingPlayerId, string playerId)
        {
            using (var db = GetDb())
            {
                var players = GetDbPlayers(db, requestingPlayerId, playerId);
                var requester = players[0];
                var player = players[1];
                var requests = db.FriendRequests.Where(f => f.RequestingPlayerId == requester.dbId && f.PlayerId == player.dbId).ToList();
                if (requests.Count < 1)
                {
                    db.FriendRequests.Add(new FriendRequest()
                    {
                        PlayerId = player.dbId,
                        RequestingPlayerId = requester.dbId
                    });
                    db.SaveChanges();
                }
            }
        }
        public void AddFriend(string playerId_1, string playerId_2)
        {
            using (var db = GetDb())
            {
                var players = GetDbPlayers(db, playerId_1, playerId_2);
                var pid1 = players[0].dbId;
                var pid2 = players[1].dbId;

                var existing = db.PlayerFriends
                                    .Where(f =>
                                        (f.PlayerId_1 == pid1 || f.PlayerId_2 == pid1) &&
                                        (f.PlayerId_1 == pid2 || f.PlayerId_2 == pid2))
                                    .Count();
                if (existing < 1)
                {
                    db.PlayerFriends.Add(new PlayerFriend()
                    {
                        PlayerId_1 = players[0].dbId,
                        PlayerId_2 = players[1].dbId
                    });
                    db.SaveChanges();
                }
            }
        }
        public void RemoveFriend(string playerId_1, string playerId_2)
        {
            using (var db = GetDb())
            {
                var players = GetDbPlayers(db, playerId_1, playerId_2);
                var pid1 = players[0].dbId;
                var pid2 = players[1].dbId;
                var existing = db.PlayerFriends
                                    .Where(f =>
                                        (f.PlayerId_1 == pid1 || f.PlayerId_2 == pid1) &&
                                        (f.PlayerId_1 == pid2 || f.PlayerId_2 == pid2))
                                    .ToList();

                if (existing.Count > 0)
                {
                    db.PlayerFriends.RemoveRange(existing);
                    db.SaveChanges();
                }
            }
        }
        public void DeletePendingFriend(string requestingPlayerId, string playerId)
        {
            using (var db = GetDb())
            {
                var players = GetDbPlayers(db, requestingPlayerId, playerId);
                var pid1 = players[0].dbId;
                var pid2 = players[1].dbId;
                var requests = db.FriendRequests.Where(f => f.RequestingPlayerId == pid1 && f.PlayerId == pid2).ToList();
                db.FriendRequests.RemoveRange(requests);
                db.SaveChanges();
            }
        }
        public IEnumerable<Core.Player> GetPendingFriends(string playerId)
        {
            if (playerId == null)
                throw new ArgumentException("PlayerId cannot be null");

            using (var db = GetDb())
            {
                var player = GetDbPlayer(db, playerId);

                if (player == null)
                    throw new Exception("Player does not exist with id : " + playerId);

                var dbPlayers = db
                                .FriendRequests
                                    .Where(f => f.RequestingPlayerId == player.dbId)
                                .Join(db.Players, f => f.PlayerId, p => p.dbId, (p, t) => t)
                                .ToList();

                foreach (var p in dbPlayers)
                {
                    yield return Map(p);
                }
            }
        }
        public IEnumerable<Core.Player> GetFriendRequests(string playerId)
        {
            if (playerId == null)
                throw new ArgumentException("PlayerId cannot be null");

            using (var db = GetDb())
            {
                var player = GetDbPlayer(db, playerId);

                if (player == null)
                    throw new Exception("Player does not exist with id : " + playerId);

                var dbPlayers = db
                                .FriendRequests
                                    .Where(f => f.PlayerId == player.dbId)
                                .Join(db.Players, f => f.RequestingPlayerId, p => p.dbId, (p, t) => t)
                                .ToList();

                foreach (var p in dbPlayers)
                {
                    yield return Map(p);
                }
            }
        }
        public IEnumerable<Core.Game> GetGamesForPairOfPlayers(string player1, string player2)
        {
            using (var db = GetDb())
            {
                List<Game> result = new List<Game>();
                GameEqualityComparer gameEquality = new GameEqualityComparer();
                var p = GetDbPlayers(db, player1, player2);
                result.AddRange(p[0].Games.Where(g => p[1].Games.Any(x => x.dbId == g.dbId)));
                result.AddRange(p[1].Games.Where(g => p[0].Games.Any(x => x.dbId == g.dbId)));
                return result.Distinct(gameEquality).Select(g => new Core.Game()
                {
                    Id = g.Id,
                    Title = g.Title
                });
            }
        }
        public bool IsPendingFriend(string playerId, string friendId)
        {
            using (var db = GetDb())
            {
                var p = GetDbPlayers(db, playerId, friendId);
                var pid1 = p[0].dbId;
                var pid2 = p[1].dbId;
                return db.FriendRequests.Any(x => x.RequestingPlayerId == pid2 && x.PlayerId == pid1);
            }
        }
        class GameEqualityComparer : EqualityComparer<Game>
        {
            public override bool Equals(Game b1, Game b2)
            {
                if (b2 == null && b1 == null)
                {
                    return true;
                }
                else if (b1 == null || b2 == null)
                {
                    return false;
                }
                else
                {
                    return b1.Id.Equals(b2.Id, StringComparison.OrdinalIgnoreCase);
                }
            }
            public override int GetHashCode(Game obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
