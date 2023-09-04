using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YazawaCommand
{
    public enum NearestAssetFlagType
    {
        Invalid = 0,
        Type = 1,
        Subtype = 2,
        AssetID = 3,
        CanBreak = 4,
        EntityUID = 5,
        Distance = 6,
    }
}
