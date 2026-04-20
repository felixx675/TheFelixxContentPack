using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using FelixxContentPack.FelixxContentPackCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Models;

namespace FelixxContentPack.FelixxContentPackCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class BirdUrn : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "birdurn.png".RelicImagePath();
        }
    }
    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "birdurn_outline.png".RelicImagePath();
        }
    }
    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "birdurn.png".BigRelicImagePath();
        }
    }
    
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override bool ShowCounter => false;

    private bool _usedThisTurn;
    private CardModel? _trackedPower;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        new[]
        {
            HoverTipFactory.FromKeyword(CardKeyword.Ethereal)
        };

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return Task.CompletedTask;

        if (_usedThisTurn)
            return Task.CompletedTask;

        if (cardPlay.Card.Type != CardType.Power)
            return Task.CompletedTask;

        if (_trackedPower != null)
            return Task.CompletedTask;

        _trackedPower = cardPlay.Card;

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card != _trackedPower)
            return;

        _trackedPower = null;
        _usedThisTurn = true;

        Flash();

        // 1. Create a copy of the Power card
        CardModel clone = cardPlay.Card.CreateClone();

        // 2. Make it Ethereal (Music Box pattern)
        CardCmd.ApplyKeyword(clone, CardKeyword.Ethereal);

        // 3. Increase cost permanently for combat (Kingly Kick pattern)
        clone.EnergyCost.AddThisCombat(1);

        // 4. Add to hand
        await CardPileCmd.AddGeneratedCardToCombat(
            clone,
            PileType.Hand,
            true
        );
    }

    public override Task BeforeSideTurnStart(
        PlayerChoiceContext choiceContext,
        CombatSide side,
        CombatState combatState)
    {
        if (side != Owner.Creature.Side)
            return Task.CompletedTask;

        _usedThisTurn = false;
        _trackedPower = null;

        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        _usedThisTurn = false;
        _trackedPower = null;

        return Task.CompletedTask;
    }
}