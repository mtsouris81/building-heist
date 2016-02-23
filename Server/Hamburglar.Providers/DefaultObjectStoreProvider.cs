using Hamburglar.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamburglar.Providers
{
    public class DefaultObjectStoreProvider : IObjectPersistence
    {
        public string DatabaseName = "BuildingHeist";
        private MongoClient client;

        public DefaultObjectStoreProvider()
        {
            client = new MongoClient(ConfigurationManager.ConnectionStrings["MongoDB"].ConnectionString);
        }

        private IMongoCollection<Core.Game> GetGameCollection()
        {
            return client.GetDatabase(DatabaseName).GetCollection<Core.Game>("Games");
        }
        private IMongoCollection<Core.Player> GetPlayerCollection()
        {
            return client.GetDatabase(DatabaseName).GetCollection<Core.Player>("Players");
        }
        private T GetById<T>(string id, IMongoCollection<T> collection)
        {
            var task = collection.Find(Builders<T>.Filter.Eq("Id", id)).FirstAsync();
            task.Wait();
            return task.Result;
        }
        private void Set<T>(string id, T item, IMongoCollection<T> collection)
        {
            var task = collection.ReplaceOneAsync(Builders<T>.Filter.Eq("Id", id), item, new UpdateOptions()
            {
                IsUpsert = true
            });
            task.Wait();
        }

        public Core.Game GetGame(string id)
        {
            return GetById(id, GetGameCollection());
        }
        public Core.Player GetPlayerById(string id)
        {
            return GetById(id, GetPlayerCollection());
        }
        public void SetPlayer(Core.Player player)
        {
            Set(player.Id, player, GetPlayerCollection());
        }
        public void SetGame(Core.Game game)
        {
            Set(game.Id, game, GetGameCollection());
        }
        
    }
}
