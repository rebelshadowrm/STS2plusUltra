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
- `MarkGiantCreature(obj)` → returns true first time, false if already marked
- `MarkHardElite(obj)` → same, separate set
- `IsGiantCreature(obj)` → check without marking
- `Reset()` called on combat cleanup

HP patches fire on `Creature.AfterAddedToRoom` and `Creature.BeforeCombatStart`. The mark prevents double-application across both calls. Two patches firing sequentially on the same call compose multiplicatively (second reads HP already modified by first).

## HP Scaling — Confirmed Behavior
- Giant Creatures only: all enemies ×2.0
- Hard Elites only: elites ×1.5, others ×1.0
- Both active: elites ×3.0 (2.0 × 1.5), regular enemies ×2.0
- This is already correct in code — patches compose multiplicatively via sequential Postfix execution.

## Endless Mode Architecture
**Flow**: Beat act 3 boss → `TheArchitect.WinRun` → `RunManager.WinRun` → `TriggerEndlessLoop` → `EnterAct(0, true)`.

Key patches:
- `EndlessModeArchitectPatch` — blocks `TheArchitect.TriggerVictory` in endless mode
- `EndlessModeArchitectWinRunPatch` — in multiplayer, defers to RunManager; in singleplayer, calls TriggerEndlessLoop directly
- `EndlessModeWinRunPatch` — intercepts `RunManager.WinRun`; host calls TriggerEndlessLoop, clients return Task.CompletedTask
- `EndlessModeKillPlayersPatch` — blocks `GuaranteeKillAllPlayers` during loop transition
- `EndlessModeEnterActPatch` — Postfix on `EnterAct`, refreshes overlay

**`GameReflection.TriggerEndlessLoop(runManager)`** (in GameReflection.cs):
1. Updates RNG seed to `{baseSeed}_L{loopCount+1}` (this is how loop count is tracked — parsed from seed string)
2. Resets `CurrentActIndex = 0`, `ActFloor = 0`
3. Calls `GenerateRooms()` (new map for new loop)
4. Returns `EnterAct(0, true)` as Task (host awaits this; DO NOT add screen-clearing calls here — they break client sync)

**Multiplayer behavior**: `RunManager.WinRun` is only called on the HOST. Host's `TriggerEndlessLoop` → `EnterAct(0, true)` uses `NetLoadingHandle` to broadcast "load act 0" to clients. Clients must be in a receptive state when this broadcast arrives.

**Known fix (deployed, untested)**: Removed 3 `CloseSingleton`/`ClearSingleton` calls from `TriggerEndlessLoop` that were tearing down network event handlers before the `EnterAct` broadcast reached clients.

## Multiplayer Role Detection
`MultiplayerReflection.cs` / `MultiplayerSafety.cs`:
- `IsMultiplayerRun()` — checks `RunManager.NetService.Type`
- `IsInteractionLocked()` — returns true if local role is Client
- `ShouldApplyAuthoritativeGameplayPatches()` — true for host or singleplayer

## Loop Count
Stored in the RNG seed string: `baseSeed_L1`, `baseSeed_L2`, etc.
`GameReflection.GetLoopCount()` parses `_L{n}` suffix. `GetTotalActNumber()` = `loopCount * 3 + currentActIndex + 1`.

## Stun Threshold Scaling (GiantCreatures)
`GiantCreaturesStunThresholdPatch.cs` — patches `ConditionalBranchState.GetNextState`.
For giant creatures, temporarily halves `_currentHp` and `_maxHp` before predicate evaluation so hardcoded HP thresholds (e.g. "stun at 231 HP") see vanilla-equivalent values. Restores original values in Postfix.
**Warning**: static field initializers in this class must null-check before calling `AccessTools.Field` — passing null type throws and crashes the entire MoreRules patch category. Use pattern: `CreatureType != null ? AccessTools.Field(CreatureType, "field") : null`.

## Known Issues / Backlog
- **Endless mode multiplayer** — fix deployed (removed screen-clear calls from TriggerEndlessLoop), not yet tested
- **Damage cap scaling** — "clamshell boss" has 49-damage/turn cap that doesn't scale with modifiers. Needs investigation.
- **Desync on room transition** — existed before mod, decided not to investigate blind

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
