using Hamburglar.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Hamburglar.Server.Controllers
{
    public class SynchController
    {
        public static DateTime LastSynch { get; private set; }
        public static double SynchIntervalSeconds = 30;

        public ISharedCache Cache;
        public IObjectPersistence ObjectStore;
        IRelationalPersistence RelationalStore;
        public SynchController(ISharedCache cache, IObjectPersistence objectStore, IRelationalPersistence relationalStore)
        {
            RelationalStore = relationalStore;
            Cache = cache;
            ObjectStore = objectStore;
        }
        public void PerformSynch()
        {
            PerformSynch(false);
        }
        public void PerformSynch(bool forceSynch)
        {
            if ((DateTime.Now - LastSynch).TotalSeconds > SynchIntervalSeconds || forceSynch)
            {
                LastSynch = DateTime.Now;
                SynchronizeFromCache();
            }
        }

        private void SynchronizeFromCache()
        {
            Thread t = new Thread(() =>
            {
                List<Core.Game> games = Cache.GetGames();
                foreach (var g in games)
                {
                    ObjectStore.SetGame(g);
                    if (g.Winner != null)
                    {
                        RelationalStore.SetGameFinished(g.Id, g.Winner.Id);
                    }
                }
            });
            t.Start();
        }
    }
}