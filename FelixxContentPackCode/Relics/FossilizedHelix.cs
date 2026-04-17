using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using FelixxContentPack.FelixxContentPackCode.Extensions;
using Godot;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace FelixxContentPack.FelixxContentPackCode.Relics;

[Pool(typeof(SharedRelicPool))]

public class FossilizedHelix() : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "fossilizedhelix.png".RelicImagePath();
        }
    }
    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "fossilizedhelix_outline.png".RelicImagePath();
        }
    }
    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "fossilizedhelix.png".BigRelicImagePath();
        }
    }
    public override RelicRarity Rarity =>
        RelicRarity.Rare;
    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new[]
        {
            new PowerVar<BufferPower>(1M)
        };
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom)
            return;
        this.Flash();
        await PowerCmd.Apply<BufferPower>(
            this.Owner.Creature,
            this.DynamicVars["BufferPower"].BaseValue,
            this.Owner.Creature,
            null
        );
    }
}