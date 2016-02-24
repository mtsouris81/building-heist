
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public class GameListResult
    {
        public bool s { get; set; }
        public List<GameListItem> i { get; private set; }
        public GameListResult()
        {
            i = new List<GameListItem>();
            s = false;
        }
        public GameListResult(IEnumerable<Game> games)
        {
            s = true;
            i = games.Select(x => new GameListItem()
            {
                i = x.Id,
                t = x.Title,
               // o = x.Creator.Username
            }).ToList();
        }
    }

}
