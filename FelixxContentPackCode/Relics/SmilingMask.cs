using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;
using BaseLib.Abstracts;
using BaseLib.Utils;
using BaseLib.Extensions;
using FelixxContentPack.FelixxContentPackCode.Extensions;
using Godot;

namespace FelixxContentPack.FelixxContentPackCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class SmilingMask() : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "smilingmask.png".RelicImagePath();
        }
    }
    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "smilingmask_outline.png".RelicImagePath();
        }
    }
    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "smilingmask.png".BigRelicImagePath();
        }
    }
    
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    public override decimal ModifyMerchantPrice(
        Player player,
        MerchantEntry entry,
        decimal originalPrice)
    {
        if (player != this.Owner)
            return originalPrice;
        
        if (entry is MerchantCardRemovalEntry)
        {
            return 75m;
        }

        return originalPrice;
    }
}