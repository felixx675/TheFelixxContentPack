using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using BaseLib.Utils;
using FelixxContentPack.FelixxContentPackCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace FelixxContentPack.FelixxContentPackCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class DeadBranch : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "deadbranch.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "deadbranch_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "deadbranch.png".BigRelicImagePath();
        }
    }

    private const string _exhaustAmountKey = "ExhaustAmount";

    private bool _isActivating;
    private int _cardsExhausted;
    private int _etherealCount;

    public override RelicRarity Rarity => RelicRarity.Rare;

    public override bool ShowCounter => true;

    public override int DisplayAmount =>
        !IsActivating
            ? CardsExhausted
            : DynamicVars[_exhaustAmountKey].IntValue;

    private bool IsActivating
    {
        get => _isActivating;
        set
        {
            this.AssertMutable();
            _isActivating = value;
            this.InvokeDisplayAmountChanged();
        }
    }

    [SavedProperty(SerializationCondition.SaveIfNotTypeDefault)]
    public int CardsExhausted
    {
        get => _cardsExhausted;
        set
        {
            this.AssertMutable();
            _cardsExhausted = value;

            this.Status =
                (decimal)_cardsExhausted == DynamicVars[_exhaustAmountKey].BaseValue - 1M
                    ? RelicStatus.Active
                    : RelicStatus.Normal;

            this.InvokeDisplayAmountChanged();
        }
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        new DynamicVar[]
        {
            new DynamicVar(_exhaustAmountKey, 2M),
            new CardsVar(1)
        };

    public override async Task AfterCardExhausted(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal)
    {
        if (card.Owner != Owner)
            return;

        if (causedByEthereal)
        {
            _etherealCount++;
        }
        else
        {
            CardsExhausted++;
            await TryTrigger(choiceContext);
        }
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != CombatSide.Player)
            return;

        CardsExhausted += _etherealCount;
        _etherealCount = 0;

        await TryTrigger(choiceContext);
    }

    private async Task TryTrigger(PlayerChoiceContext context)
    {
        while (CardsExhausted >= 2)
        {
            CardsExhausted -= 2;
            await Trigger(context);
        }
    }

    private async Task Trigger(PlayerChoiceContext context)
    {
        _ = DoActivateVisuals(); // run visuals in parallel (like Joss Paper)

        var unlocked = Owner.Character.CardPool.GetUnlockedCards(
            Owner.UnlockState,
            Owner.RunState.CardMultiplayerConstraint
        ).ToList();

        var generated = CardFactory.GetForCombat(
            Owner,
            unlocked,
            1,
            Owner.RunState.Rng.CombatCardGeneration
        ).ToList();

        await CardPileCmd.AddGeneratedCardsToCombat(
            generated,
            PileType.Hand,
            true
        );
    }

    private async Task DoActivateVisuals()
    {
        IsActivating = true;
        Flash();
        await Cmd.Wait(1f);
        IsActivating = false;
    }
}