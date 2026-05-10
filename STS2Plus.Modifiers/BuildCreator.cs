using System;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Models;
using STS2Plus.Ui;

namespace STS2Plus.Modifiers;

internal sealed class BuildCreator : SyncedModifierModel
{
	public override Func<Task>? GenerateNeowOption(EventModel eventModel)
	{
		EventModel eventModel2 = eventModel;
		return () => BuildCreatorOverlay.OpenAsync(eventModel2);
	}
}
