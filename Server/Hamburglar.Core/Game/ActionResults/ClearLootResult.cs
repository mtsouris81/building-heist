
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public class ClearLootResult
    {
        public int? LootEarned { get; set; }
        public string TrapOpponentId { get; set; }
        public bool WasTrapped
        {
            get
            {
                return !string.IsNullOrEmpty(TrapOpponentId);
            }
        }
        public bool IsNothing
        {
            get
            {
                return (!LootEarned.HasValue || LootEarned.Value == 0) && !WasTrapped;
            }
        }
    }

}
