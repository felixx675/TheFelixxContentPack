using BaseLib.Abstracts;
using BaseLib.Extensions;
using FelixxContentPack.FelixxContentPackCode.Extensions;
using Godot;

namespace FelixxContentPack.FelixxContentPackCode.Powers;

public abstract class FelixxContentPackPower : CustomPowerModel
{
    //Loads from FelixxContentPack/images/powers/your_power.png
    public override string CustomPackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".PowerImagePath();
        }
    }

    public override string CustomBigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
            return ResourceLoader.Exists(path) ? path : "power.png".BigPowerImagePath();
        }
    }
}