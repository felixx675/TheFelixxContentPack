using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using FelixxContentPack.FelixxContentPackCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace FelixxContentPack.FelixxContentPackCode.Relics;


[Pool(typeof(SharedRelicPool))]
public class WarpedTongs : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "warpedtongs.png".RelicImagePath();
        }
    }
    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "warpedtongs_outline.png".RelicImagePath();
        }
    }
    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "warpedtongs.png".BigRelicImagePath();
        }
    }
    
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    public override async Task AfterRestSiteSmith(Player player)
    {
        if (player != Owner)
            return;
        
        var deck = Owner.Deck;

        var candidates = deck.Cards
            .Where(c => c != null && c.IsUpgradable)
            .ToList();

        if (candidates.Count == 0)
            return;

        Flash();

        CardModel target = candidates
            .StableShuffle(Owner.RunState.Rng.Niche)
            .First();

        CardCmd.Upgrade(target);

        await Task.CompletedTask;
    }
}