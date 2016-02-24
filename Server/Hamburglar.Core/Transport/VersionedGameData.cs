
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hamburglar.Core
{
    public class VersionedGameData<T>
    {
        public T d { get; set; }
        public int v { get; set; }
    }

}
