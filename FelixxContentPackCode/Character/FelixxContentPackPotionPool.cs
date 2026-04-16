using BaseLib.Abstracts;
using FelixxContentPack.FelixxContentPackCode.Extensions;
using Godot;

namespace FelixxContentPack.FelixxContentPackCode.Character;

public class FelixxContentPackPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => FelixxContentPack.Color;


    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}