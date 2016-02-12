using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    [Serializable]
    public class LootTrap
    {
        public int TrapId { get; set; }
        public string PlayerId { get; set; }
    }
}
