
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public interface IRealTimeMessaging
    {
        void OnGameCreated(Game game);
        void RoomEntered(Game game, string playerId);
        void RoomExited(Game game, string playerId);
        void OnPlayerTrapped(Game game, string playerId, string trapOpponentId);
        void OpponentCaught(Game game, string playerId, string opponentId);
        void NeedUpdate(Game game);
        void GameFinished(Game game);
        void OnGameReady(Game game, DateTime now);
    }

}
