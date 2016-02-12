using Hamburglar.Core;
using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hamburglar.Providers
{
    public class DefaultSharedCacheProvider : ISharedCache
    {
        public RedisClient GetCache()
        {
            return new RedisClient();
        }
        public Core.Game GetGame(string gameId)
        {
            return Get<Core.Game>("Game", gameId);
        }
        public Core.Player GetPlayerById(string id)
        {
            return Get<Core.Player>("Player", id);
        }
        public void SetGame(Core.Game game)
        {
            Set("Game", game.Id, game);
        }
        public void SetPlayer(Core.Player player)
        {
            Set("Player", player.Id, player);
        }

        private void Set<T>(string set, string id, T item)
        {
            using (var cache = GetCache())
            {
                cache.SetValue(GetPrefix(set, id), JsonConvert.SerializeObject(item));
            }
        }
        private T Get<T>(string set, string id)
        {
            using (var cache = GetCache())
            {
                string json = cache.GetValue(GetPrefix(set, id));
                if (json != null)
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }
            }
            return default(T);
        }

        private List<T> GetAll<T>(string set)
        {
            List<T> result = new List<T>();
            string prefix = string.Format("{0}-", set);
            using (var cache = GetCache())
            {
                var keys = cache.GetAllKeys();
                foreach(var k in keys)
                {
                    if (k.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    {
                        string json = cache.GetValue(k);
                        if (json != null)
                        {
                            result.Add(JsonConvert.DeserializeObject<T>(json));
                        }
                    }
                }
            }
            return result;
        }
        public string GetPrefix(string prefix, string id)
        {
            return string.Format("{0}-{1}", prefix, id);
        }

        public List<Core.Game> GetGames()
        {
            return GetAll<Core.Game>("Game");
        }
    }
}
