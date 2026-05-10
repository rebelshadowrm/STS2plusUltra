# STS2Plus — Claude Reference

## Project
Harmony mod for Slay the Spire 2 (v0.105.1). Three players run it together in multiplayer.
Built with `dotnet build STS2Plus.csproj -c Debug`. Output: `bin\Debug\net9.0\STS2Plus.dll`.
Deploy: copy DLL to `B:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\STS2Plus.dll`. All 3 players must sync simultaneously before playing.

## Key Directories
- `STS2Plus/` — ModEntry, PlusState, MultiplayerSafety, PatchCategories
- `STS2Plus.Patches/` — all Harmony patches
- `STS2Plus.Reflection/` — GameReflection.cs, MultiplayerReflection.cs (reflection helpers)
- `STS2Plus.Features/` — AppliedTracker, RouteAdvisor, BuildCreator, etc.
- `STS2Plus.Modifiers/` — modifier model definitions
- `probe*/` — throwaway probe projects (excluded from main build)

## Active Modifiers (PlusState)
| Name | Key | Effect |
|------|-----|--------|
| Attack/Defense | `ATTACK_DEFENSE` | +2 dmg/block to attack/defense cards |
| Giant Creatures | `GIANT_CREATURES` | ×2.0 HP to all enemies |
| Hard Elites | `HARD_ELITES` | ×1.5 HP to elites only |
| Endless Mode | `ENDLESS_MODE` | Loop back to act 0 after beating act 3 |
| Glass Cannon | `GLASS_CANNON` | Players start at 1 HP max |

Check active state via `PlusState.IsGiantCreaturesActive()`, `PlusState.IsHardElitesActive()`, etc.

## Patch Pattern
All gameplay patches use `[HarmonyPatchCategory("MoreRules")]`. They target game methods via reflection using `RuntimeTypeResolver.FindType("FullTypeName")` + `AccessTools.Method(type, "MethodName")`.

```csharp
[HarmonyPatchCategory("MoreRules")]
[HarmonyPatch]
internal static class MyPatch
{
    [HarmonyTargetMethod]
    private static MethodBase? TargetMethod() =>
        AccessTools.Method(RuntimeTypeResolver.FindType("MegaCrit.Sts2.Core.Runs.RunManager"), "MethodName");

    private static bool Prefix(object __instance, ref Task? __result) { ... }
    private static void Postfix(object __instance) { ... }
}
```

## AppliedTracker
`STS2Plus.Features/AppliedTracker.cs` — prevents double-applying HP multipliers per creature.
- `MarkGiantCreature(obj)` / `IsGiantCreature(obj)` — giant creatures set
- `MarkHardElite(obj)` / `IsHardElite(obj)` — hard elites set
- `MarkEndlessScaled(obj)` / `IsEndlessScaled(obj)` — endless HP scaling set
- `Reset()` called on combat cleanup

HP patches fire on `Creature.AfterAddedToRoom` and `Creature.BeforeCombatStart`. The mark prevents double-application across both calls. Two patches firing sequentially on the same call compose multiplicatively (second reads HP already modified by first).

## HP Scaling — Confirmed Behavior ✅
- Giant Creatures only: all enemies ×2.0
- Hard Elites only: elites ×1.5, others ×1.0
- Both active: elites ×3.0 (2.0 × 1.5), regular enemies ×2.0
- Correct in code — patches compose multiplicatively via sequential Postfix execution (separate `AppliedTracker` marks allow both to apply to the same creature).

## Endless Mode Scaling (Act-Based) ✅
All scaling anchors to `GetTotalActNumber()` = `loopCount * 3 + currentActIndex + 1`. Acts 1–3 are always unscaled (×1.0).

**HP scaling** — `GameReflection.GetEndlessHpMultiplier()`:
```
1.33 ^ (actNumber - 3)
```
Act 4 = ×1.33, act 6 = ×2.35, act 9 = ×5.54, act 12 = ×13.0

**Damage scaling** — `GameReflection.GetEndlessDamageMultiplier()`:
```
1.0 + (actNumber - 3) × 0.2
```
Act 4 = ×1.2, act 8 = ×2.0, act 13 = ×3.0

**Damage patch** — `EndlessModeDamageScalingPatch.cs` patches `NCreature.PerformIntent`. Finds damage fields on the current intent via reflection (tries `_damage`, `_baseDamage`, `BaseDamage`, `Damage`, `_hitDamage`). Scales in Prefix, restores in Postfix. Verbose log shows whether fields were found — check if "no damage fields found" appears and report field names to fix.

**Vision**: infinite acts (act 4, 5, 6, 7... indefinitely), not looping back to act 1. Loop count/seed tracking is a current implementation detail, not the end goal.

## Endless Mode Architecture — NEEDS PORT ⚠️
The working reference implementation is in `C:\Games\Slay the spire 2 mods\EndlessModeRef\src\`. The current code in this repo uses an outdated approach that does NOT work in multiplayer.

**Working approach (from reference mod):**
- `RunManager.WinRun` is intercepted (returns false) → `EndlessLoopCoordinator.StartFromWinRunAsync`
- Host broadcasts a custom `EndlessLoopBeginMessage` network message to all clients
- Every player (host + clients) independently runs `EndlessLoopTransition.StartAsync` locally
- `EndlessLoopTransition.StartAsync` serializes run → sanitizes save → revives dead players → rebuilds RunState → resets ALL multiplayer synchronizers → clears screens → calls `NGame.LoadRun`
- `EndlessModeNeowOptionsPatch` + `EndlessModeNeowRoomPatch` fix Neow options on loop restart (sealed deck issue)
- `EndlessLoopCoordinator.AttachCurrentRun()` called from `RunManager.Launch` Postfix to register the network message handler

**Files to port from reference mod:**
- `STS2Plus.Multiplayer/EndlessLoopBeginMessage.cs` — custom net message struct
- `STS2Plus.Patches/EndlessLoopCoordinator.cs` — host/client coordination logic
- `STS2Plus.Patches/EndlessLoopTransition.cs` — full transition (~400 lines, resets all multiplayer state)
- `STS2Plus.Patches/EndlessMapSelectionParity.cs` — map selection sync helper
- `STS2Plus.Patches/EndlessModeNeowOptionsPatch.cs` — fixes Neow options on loop restart
- `STS2Plus.Patches/EndlessModeNeowRoomPatch.cs` — repairs Neow room if options are empty
- Rewrite `EndlessModeWinRunPatch` — intercept WinRun, call coordinator
- Rewrite `EndlessModeGameOverScreenPatch` — use `EndlessLoopCoordinator.BeginFromGameOverScreenAsync`
- Add `EndlessLoopCoordinator.AttachCurrentRun()` to the `RunManager.Launch` Postfix patch

## Multiplayer Role Detection
`MultiplayerReflection.cs` / `MultiplayerSafety.cs`:
- `IsMultiplayerRun()` — checks `RunManager.NetService.Type`
- `IsInteractionLocked()` — returns true if local role is Client
- `ShouldApplyAuthoritativeGameplayPatches()` — true for host or singleplayer

## Loop Count
Stored in the RNG seed string: `baseSeed_L1`, `baseSeed_L2`, etc.
`GameReflection.GetLoopCount()` parses `_L{n}` suffix. `GetTotalActNumber()` = `loopCount * 3 + currentActIndex + 1`.

## Stun Threshold Scaling ✅
`GiantCreaturesStunThresholdPatch.cs` — patches `ConditionalBranchState.GetNextState`.
Temporarily divides `_currentHp` and `_maxHp` by the **total HP multiplier** applied to that creature before predicate evaluation, so hardcoded thresholds (e.g. "stun at 231 HP") always fire at the correct percentage of HP. Restores original values in Postfix.

Total multiplier is the product of whichever are active and marked on the creature:
- Giant Creatures: ×2.0
- Hard Elites: ×1.5
- Endless mode: `GetEndlessHpMultiplier()` (act-based)

Examples at act 9 (endless ×5.54):
- Giant Creatures only: divides by ×2.0
- Giant Creatures + Hard Elites (elite): divides by ×3.0
- Giant Creatures + Endless: divides by ×11.1
- All three on an elite: divides by ×16.6

**Warning**: static field initializers in this class must null-check before calling `AccessTools.Field` — passing null type throws and crashes the entire MoreRules patch category. Use pattern: `CreatureType != null ? AccessTools.Field(CreatureType, "field") : null`.

## Compact Relic Drawer ✅
`STS2Plus.Ui/CompactRelicDrawer.cs` — a full Godot `Control` node that replaces the default relic display. Attached via `CompactRelicDrawerAttachPatch` on `NGlobalUi.Initialize` (Core category). Seven dedicated patches handle show/hide animations, immediate visibility toggling, potion navigation, and multiplayer layout. `CompactRelicDrawerMultiplayerPositionPatch` patches `NMultiplayerPlayerStateContainer.UpdatePosition` to fix the layout in 3-player runs (previously blocked top half of screen). Confirmed implemented and functional.

## Known Issues / Backlog
- **Endless mode multiplayer** ⚠️ — needs full port from reference mod (see Endless Mode Architecture section above). Current implementation broken in multiplayer.
- **Sealed deck on loop restart** — Neow shows sealed deck on loop; fixed in reference mod via `EndlessModeNeowOptionsPatch` + `EndlessModeNeowRoomPatch` (part of the port).
- **Damage cap scaling** — "clamshell boss" has 49-damage/turn cap that doesn't scale with modifiers. Needs investigation.
- **Desync on room transition** — existed before mod, decided not to investigate blind.

## Game API Quick Reference
Key types (all accessed via reflection):
- `MegaCrit.Sts2.Core.Runs.RunManager` — `WinRun()`, `EnterAct(int, bool)`, `SetActInternal(int)`, `GenerateRooms()`, `State` (RunState)
- `MegaCrit.Sts2.Core.Models.RunState` — `CurrentActIndex`, `ActFloor`, `Acts`, `Rng`
- `MegaCrit.Sts2.Core.Entities.Creatures.Creature` — `_currentHp`, `_maxHp` (fields), `AfterAddedToRoom()`, `BeforeCombatStart()`
- `MegaCrit.Sts2.Core.Multiplayer.Game.ActChangeSynchronizer` — `SetLocalPlayerReady()`, `MoveToNextAct()`, `IsWaitingForOtherPlayers()`
- `MegaCrit.Sts2.Core.Multiplayer.Game.MapSelectionSynchronizer` — `BeforeMapGenerated()`
- `MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine.ConditionalBranchState` — `GetNextState(Creature, Rng)`

## Probe Projects
Throwaway C# console projects in `probe*/` directories to inspect game DLLs via `MetadataLoadContext`. These are excluded from the main build. Create via bash (not PowerShell — PowerShell creates `:TEMP` NTFS streams). Save important results to a text file rather than re-running each session.

## Log Searching
When given a godot.log, search for `[STS2Plus]` to find mod-specific events. Key signals:
- `intercepting WinRun (host/singleplayer)` — endless loop triggered on host
- `deferring Architect.WinRun to RunManager` — multiplayer deferral working
- `CombatStateSynchronizer] Waiting to receive all sync messages` — clients didn't follow host into new act (multiplayer sync broke)
