using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using BaseLib.Extensions;
using FelixxContentPack.FelixxContentPackCode.Extensions;
using Godot;

namespace FelixxContentPack.FelixxContentPackCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class TinyHouse() : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "tinyhouse.png".RelicImagePath();
        }
    }
    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "tinyhouse_outline.png".RelicImagePath();
        }
    }
    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "tinyhouse.png".BigRelicImagePath();
        }
    }
    
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new GoldVar(50),
            new MaxHpVar(5)
        };

    public override async Task AfterObtained()
    {
        this.Flash();
        
        await PlayerCmd.GainGold(this.DynamicVars.Gold.BaseValue, this.Owner);
        
        await CreatureCmd.GainMaxHp(this.Owner.Creature, this.DynamicVars.MaxHp.BaseValue);
        
        var upgradableCards = PileType.Deck
            .GetPile(this.Owner)
            .Cards
            .Where(c => c != null && c.IsUpgradable)
            .ToList();

        if (upgradableCards.Count > 0)
        {
            var card = upgradableCards
                .StableShuffle(this.Owner.RunState.Rng.Niche)
                .First();

            NRun.Instance?.GlobalUi.GridCardPreviewContainer.ForceMaxColumnsUntilEmpty(3);
            CardCmd.Upgrade(card, CardPreviewStyle.GridLayout);
        }
        
        List<Reward> rewards = new();

        var options = new CardCreationOptions(
            new[] { this.Owner.Character.CardPool },
            CardCreationSource.Other,
            CardRarityOddsType.RegularEncounter
        );

        rewards.Add(new CardReward(options, 3, this.Owner));
        rewards.Add(new PotionReward(this.Owner));

        await RewardsCmd.OfferCustom(this.Owner, rewards);
    }
}