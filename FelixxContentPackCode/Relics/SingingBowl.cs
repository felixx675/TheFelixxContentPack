using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using FelixxContentPack.FelixxContentPackCode.Extensions;
using Godot;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace FelixxContentPack.FelixxContentPackCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class SingingBowl : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "singingbowl.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "singingbowl_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "singingbowl.png".BigRelicImagePath();
        }
    }

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[]
        {
            new MaxHpVar(2M)
        };

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (this.Owner.Creature.IsDead)
            return;

        var currentMapPoint = this.Owner.RunState.CurrentMapPoint;

        if (currentMapPoint == null || currentMapPoint.PointType != MapPointType.Unknown)
            return;

        this.Flash();

        await CreatureCmd.GainMaxHp(
            this.Owner.Creature,
            this.DynamicVars.MaxHp.BaseValue
        );
    }
}