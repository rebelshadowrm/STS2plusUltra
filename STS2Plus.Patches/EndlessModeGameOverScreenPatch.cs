using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;

namespace STS2Plus.Patches;

[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch(typeof(NGameOverScreen), "_Ready")]
internal static class EndlessModeGameOverScreenPatch
{
    private static void Postfix(NGameOverScreen __instance)
    {
        EndlessLoopCoordinator.RegisterGameOverScreen(__instance);

        if (!PlusState.IsEndlessModeActive() || !EndlessModeWinRunPatch.InWinTransition)
            return;
        if (!MultiplayerSafety.ShouldApplyAuthoritativeGameplayPatches())
            return;

        var button = new Button();
        button.Text = "Continue Loop";

        var normal = new StyleBoxFlat();
        normal.BgColor = new Color(0.35f, 0.35f, 0.35f);
        normal.SetCornerRadiusAll(4);
        button.AddThemeStyleboxOverride("normal", normal);
        button.AddThemeStyleboxOverride("hover", normal);
        button.AddThemeColorOverride("font_color", Colors.White);
        button.CustomMinimumSize = new Vector2(220, 50);

        // Anchor to bottom-left of the screen
        button.AnchorLeft = 0f;
        button.AnchorTop = 1f;
        button.AnchorRight = 0f;
        button.AnchorBottom = 1f;
        button.OffsetLeft = 30;
        button.OffsetTop = -80;
        button.OffsetRight = 250;
        button.OffsetBottom = -30;

        __instance.AddChild(button);
        button.Pressed += async () =>
        {
            EndlessModeWinRunPatch.InWinTransition = false;
            ModEntry.Logger.Info("STS2Plus endless loop: Continue Loop button pressed.", 1);
            await EndlessLoopCoordinator.BeginFromGameOverScreenAsync(__instance);
        };

        ModEntry.Logger.Info("STS2Plus endless loop: injected Continue Loop button into game over screen.");
    }
}
