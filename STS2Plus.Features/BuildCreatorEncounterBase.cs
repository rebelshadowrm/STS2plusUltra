using System.Collections.Generic;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Monsters;

namespace STS2Plus.Features;

internal abstract class BuildCreatorEncounterBase : EncounterModel
{
	public override bool IsDebugEncounter => true;

	public override IEnumerable<MonsterModel> AllPossibleMonsters => (IEnumerable<MonsterModel>)(object)new BigDummy[1] { ModelDb.Monster<BigDummy>() };

	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		return new(MonsterModel, string)[1] { (((MonsterModel)ModelDb.Monster<BigDummy>()).ToMutable(), null) };
	}
}
