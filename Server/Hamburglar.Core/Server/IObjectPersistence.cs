
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public interface IObjectPersistence
    {
        Game GetGame(string gameId);
        Player GetPlayerById(string id);
        void SetPlayer(Player player);
        void SetGame(Game game);
    }

}
