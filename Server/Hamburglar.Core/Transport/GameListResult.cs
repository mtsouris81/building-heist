
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public class GameListResult : ModelListResult<Game>
    {
        public GameListResult() : this(null) { }
        public GameListResult(IEnumerable<Game> models) : base(x => x.Id, x => x.Title, models) { }
    }
    public class PlayerListResult : ModelListResult<Player>
    {
        public PlayerListResult() : this(null) { }
        public PlayerListResult(IEnumerable<Player> models) : base(x => x.Id, x => x.Username, models) { }
    }
    public class FriendListResult : PlayerListResult
    {
        public List<GameListItem> fr { get; private set; }
        public List<GameListItem> pf { get; private set; }
        public FriendListResult(IEnumerable<Player> friends, IEnumerable<Player> friendRequests, IEnumerable<Player> pendingFriends) : base(friends)
        {
            fr = new List<GameListItem>();
            pf = new List<GameListItem>();
            Populate(fr, friendRequests);
            Populate(pf, pendingFriends);
        }
    }
    public class ModelListResult<T>
    {
        public bool s { get; set; }
        public List<GameListItem> i { get; private set; }
        Func<T, string> GetId;
        Func<T, string> GetName;

        public ModelListResult(Func<T, string> getId, Func<T, string> getName, IEnumerable<T> models)
        {
            GetId = getId;
            GetName = getName;
            i = new List<GameListItem>();
            s = false;
            Populate(i, models);
        }
        public void Populate(List<GameListItem> list, IEnumerable<T> models)
        {
            if (models == null)
                return;

            s = true;
            list.Clear();
            list.AddRange(models.Select(x => new GameListItem()
            {
                i = GetId(x),
                t = GetName(x)
            }));
        }
    }
}