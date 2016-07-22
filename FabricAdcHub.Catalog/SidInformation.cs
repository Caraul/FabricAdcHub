using System.Collections.Generic;

namespace FabricAdcHub.Catalog
{
    public class SidInformation
    {
        public string Nick { get; set; }

        public bool IsInNormalState { get; set; }

        public HashSet<string> Features { get; set; }
    }
}
