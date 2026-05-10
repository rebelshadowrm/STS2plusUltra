using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace STS2Plus.Modifiers;

internal abstract class SyncedModifierModel : ModifierModel
{
	public override LocString Title => new LocString("modifiers", ((AbstractModel)this).Id.Entry + ".title");

	public override LocString Description => new LocString("modifiers", ((AbstractModel)this).Id.Entry + ".description");

	protected override string IconPath => ImageHelper.GetImagePath(GetBuiltInIconPathForEntry(((AbstractModel)this).Id.Entry));

	internal static string GetBuiltInIconPathForEntry(string entry)
	{
		if (1 == 0)
		{
		}
		string result = entry switch
		{
			"ATTACK_DEFENSE" => "packed/modifiers/all_star.png", 
			"ATTACK_DEFENSE_PLUS" => "packed/modifiers/murderous.png", 
			"IRON_SKIN" => "packed/modifiers/terminal.png", 
			"GIANT_CREATURES" => "packed/modifiers/big_game_hunter.png", 
			"HARD_ELITES" => "packed/modifiers/vintage.png", 
			"ENDLESS_MODE" => "packed/modifiers/flight.png", 
			"GLASS_CANNON" => "packed/modifiers/insanity.png", 
			"UNLIMITED_GROWTH" => "packed/modifiers/hoarder.png", 
			"SANDBOX" => "packed/modifiers/sealed_deck.png", 
			"BUILD_CREATOR" => "packed/modifiers/sealed_deck.png", 
			_ => "packed/modifiers/midas.png", 
		};
		if (1 == 0)
		{
		}
		return result;
	}
}
