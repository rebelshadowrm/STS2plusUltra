using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Godot;
using STS2Plus;
using STS2Plus.Ui;

[assembly: AssemblyHasScripts(new Type[]
{
	typeof(ModEntry),
	typeof(BuildCreatorOverlay),
	typeof(CompactRelicDrawer),
	typeof(IncomingDamageOverlay),
	typeof(PlayerDamageShieldBadge)
})]
[assembly: AssemblyVersion("0.0.0.0")]
