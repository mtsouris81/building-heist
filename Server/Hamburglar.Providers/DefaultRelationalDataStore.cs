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
                var p = db.Players.Where(x => x.Id == id).FirstOrDefault();

                if (p == null)
                    return null;

                return Map(p); 
            }
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
    }
}
