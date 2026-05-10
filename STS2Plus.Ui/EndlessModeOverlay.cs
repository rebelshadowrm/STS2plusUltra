using Godot;

namespace STS2Plus.Ui;

internal static class EndlessModeOverlay
{
	private const string LayerName = "STS2PlusEndlessModeLayer";

	private const string LabelName = "STS2PlusEndlessModeLabel";

	public static void Refresh()
	{
		MainLoop mainLoop = Engine.GetMainLoop();
		SceneTree val = (SceneTree)(object)((mainLoop is SceneTree) ? mainLoop : null);
		Window val2 = ((val != null) ? val.Root : null);
		if (val2 != null)
		{
			CanvasLayer nodeOrNull = ((Node)val2).GetNodeOrNull<CanvasLayer>((NodePath)"STS2PlusEndlessModeLayer");
			if (nodeOrNull != null)
			{
				((Node)nodeOrNull).QueueFree();
			}
		}
	}
}
