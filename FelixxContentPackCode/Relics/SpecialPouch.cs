using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using FelixxContentPack.FelixxContentPackCode.Extensions;
using Godot;

using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace FelixxContentPack.FelixxContentPackCode.Relics;

[Pool(typeof(SharedRelicPool))]
public class SpecialPouch : CustomRelicModel
{
    public override string PackedIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "specialpouch.png".RelicImagePath();
        }
    }

    protected override string PackedIconOutlinePath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
            return ResourceLoader.Exists(path) ? path : "specialpouch_outline.png".RelicImagePath();
        }
    }

    protected override string BigIconPath
    {
        get
        {
            var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
            return ResourceLoader.Exists(path) ? path : "specialpouch.png".BigRelicImagePath();
        }
    }

    public override RelicRarity Rarity => RelicRarity.Shop;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext context, Player player)
    {
        if (player != Owner)
            return;

        // Only first turn (same as Gambling Chip)
        if (Owner.Creature.CombatState.RoundNumber > 1)
            return;

        var hand = Owner.PlayerCombatState.Hand.Cards;

        // Count innate cards currently in hand
        int innateCount = hand.Count(card => card.Keywords.Contains(CardKeyword.Innate));

        if (innateCount <= 0)
            return;

        // Prevent overdraw (Scrawl logic)
        int space = 10 - hand.Count;
        if (space <= 0)
            return;

        int drawAmount = Math.Min(innateCount, space);

        if (drawAmount <= 0)
            return;

        Flash();

        await CardPileCmd.Draw(context, (decimal)drawAmount, Owner);
    }
}