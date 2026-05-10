# STS2 Game API Reference

Auto-generated from `sts2.dll` v0.1.0.0. Full type listings below.
To regenerate: `dotnet run --project probe_api/probe_api.csproj`
To decompile any type: `ilspycmd "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\data_sts2_windows_x86_64\sts2.dll" -t "Full.Type.Name"`

---

## Quick Reference — Confirmed Patch Targets

### Intent Damage (EndlessModeDamageScalingPatch) ✅
```
AttackIntent.GetSingleDamage(IEnumerable<Creature> targets, Creature owner) → int
```
- Non-virtual. Called once per hit for ALL attack types (single and multi).
- Calls `DamageCalc()` internally, then runs through `Hook.ModifyDamage` for powers.
- **Patch here** (Postfix `ref int __result`) to scale enemy damage.
- `DamageCalc` (`Func<decimal>`) is **display-only** — patching it does NOT affect actual damage dealt.

**Class hierarchy:**
```
AbstractIntent
  └── AttackIntent (abstract)        GetSingleDamage() ← patch here
        ├── SingleAttackIntent        GetTotalDamage() = GetSingleDamage()
        ├── MultiAttackIntent         GetTotalDamage() = GetSingleDamage() * Repeats
        └── DeathBlowIntent : SingleAttackIntent
```

### Intent Display
```
NIntent.UpdateIntent(AbstractIntent intent, IEnumerable<Creature> targets, Creature owner)
```
Patched by `IncomingDamageIntentPatch` (Core category) to track incoming damage overlay.

### Stun Threshold (GiantCreaturesStunThresholdPatch) ✅
```
ConditionalBranchState.GetNextState(Creature creature, Rng rng)
```
Temporarily divides `Creature._currentHp` / `Creature._maxHp` by total HP multiplier during predicate evaluation, restores in Postfix.

### HP Scaling ✅
Fires on `Creature.AfterAddedToRoom` and `Creature.BeforeCombatStart`.
Tracked per-creature via `AppliedTracker` (identity hash sets).

---

## MegaCrit.Sts2.Core.Combat

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c__58`1`

### `sealed class <>c__DisplayClass115_0`

**Fields**

- `public TaskCompletionSource completionSource`

### `sealed class <>c__DisplayClass59_0`

**Fields**

- `public Nullable<UInt32> combatId`

### `sealed class <>c__DisplayClass60_0`

**Fields**

- `public Nullable<UInt32> combatId`
- `public TaskCompletionSource<Creature> completionSource`

### `sealed class <>c__DisplayClass68_0`

**Fields**

- `public UInt64 playerId`

### `sealed class <AfterAllPlayersReadyToBeginEnemyTurn>d__119 : ValueType, IAsyncStateMachine`

**Fields**

- `public Func<Task> actionDuringEnemyTurn`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <AfterAllPlayersReadyToEndTurn>d__114 : ValueType, IAsyncStateMachine`

**Fields**

- `public Func<Task> actionDuringEnemyTurn`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <AfterCreatureAdded>d__103 : ValueType, IAsyncStateMachine`

**Fields**

- `public Creature creature`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <CallCombatStateChangedDeferred>d__31 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <CheckForEmptyHand>d__104 : ValueType, IAsyncStateMachine`

**Fields**

- `public PlayerChoiceContext choiceContext`
- `public Player player`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <CheckWinCondition>d__111 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <DoTurnEnd>d__117 : ValueType, IAsyncStateMachine`

**Fields**

- `public PlayerChoiceContext choiceContext`
- `public Player player`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EndCombatInternal>d__109 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EndEnemyTurn>d__101 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EndEnemyTurnInternal>d__118 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EndPlayerTurnPhaseOneInternal>d__116 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EndPlayerTurnPhaseTwoInternal>d__120 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <ExecuteEnemyTurn>d__112 : ValueType, IAsyncStateMachine`

**Fields**

- `public Func<Task> actionDuringEnemyTurn`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <FlushPlayerHand>d__121 : ValueType, IAsyncStateMachine`

**Fields**

- `public Player player`
- `public HookPlayerChoiceContext playerChoiceContext`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <GetCreatureAsync>d__60 : ValueType, IAsyncStateMachine`

**Fields**

- `public Nullable<UInt32> combatId`
- `public double timeoutSec`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <GodotTimerTask>d__75 : ValueType, IAsyncStateMachine`

**Fields**

- `public double timeSec`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <HandlePlayerDeath>d__106 : ValueType, IAsyncStateMachine`

**Fields**

- `public Player player`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <IterateHookListeners>d__69 : IEnumerable<AbstractModel>, IEnumerable, IEnumerator<AbstractModel>, IEnumerator, IDisposable`

**Properties**

- `internal AbstractModel System.Collections.Generic.IEnumerator<MegaCrit.Sts2.Core.Models.AbstractModel>.Current` { get }
- `internal object System.Collections.IEnumerator.Current` { get }

**Methods**

- `internal bool MoveNext()`
- `internal IEnumerator<AbstractModel> System.Collections.Generic.IEnumerable<MegaCrit.Sts2.Core.Models.AbstractModel>.GetEnumerator()`
- `internal IEnumerator System.Collections.IEnumerable.GetEnumerator()`
- `internal void System.Collections.IEnumerator.Reset()`
- `internal void System.IDisposable.Dispose()`

### `sealed class <RunAutoPrePlayPhase>d__93 : ValueType, IAsyncStateMachine`

**Fields**

- `public Player player`
- `public HookPlayerChoiceContext playerChoiceContext`
- `public Task setupPlayerTurnTask`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SetupPlayerTurn>d__94 : ValueType, IAsyncStateMachine`

**Fields**

- `public Player player`
- `public HookPlayerChoiceContext playerChoiceContext`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <StartCombatInternal>d__91 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <StartTurn>d__92 : ValueType, IAsyncStateMachine`

**Fields**

- `public Func<Task> actionDuringEnemyTurn`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SwitchFromPlayerToEnemySide>d__122 : ValueType, IAsyncStateMachine`

**Fields**

- `public Func<Task> actionDuringEnemyTurn`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <WaitForActionThenEndTurn>d__113 : ValueType, IAsyncStateMachine`

**Fields**

- `public GameAction action`
- `public Func<Task> actionDuringEnemyTurn`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <WaitForUnpause>d__127 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <WaitUntilQueueIsEmptyOrWaitingOnNonPlayerDrivenAction>d__115 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `class CombatManager`

**Properties**

- `internal CancellationToken CombatCt` { get }
- `public CardModel DebugForcedTopCardOnNextShuffle` { get; set }
- `public bool EndingPlayerTurnPhaseOne` { get; set }
- `public bool EndingPlayerTurnPhaseTwo` { get; set }
- `public CombatHistory History` { get }
- `public bool IsAboutToLose` { get }
- `public bool IsEnding` { get }
- `public bool IsEnemyTurnStarted` { get; set }
- `public bool IsInProgress` { get; set }
- `public bool IsOverOrEnding` { get }
- `public bool IsPaused` { get; set }
- `public bool PlayerActionsDisabled` { get; set }
- `public IReadOnlyList<Player> PlayersTakingExtraTurn` { get }
- `public CombatStateTracker StateTracker` { get }

**Fields**

- `private CancellationTokenSource _combatCts`
- `private PendingLossState _pendingLoss`
- `private bool _playerActionsDisabled`
- `private Lock _playerReadyLock`
- `private HashSet<Player> _playersReadyToBeginEnemyTurn`
- `private HashSet<Player> _playersReadyToEndTurn`
- `private List<Player> _playersTakingExtraTurn`
- `private CombatState _state`
- `private Action<CombatState> AboutToSwitchToEnemyTurn`
- `private Action<CombatRoom> CombatEnded`
- `private Action<CombatState> CombatSetUp`
- `private Action<CombatRoom> CombatWon`
- `private Action<CombatState> CreaturesChanged`
- `private Action<CombatState> PlayerActionsDisabledChanged`
- `private Action<Player, bool> PlayerEndedTurn`
- `private Action<Player> PlayerUnendedTurn`
- `private Action<CombatState> TurnEnded`
- `private Action<CombatState> TurnStarted`

**Methods**

- `public void AddCreature(Creature creature)`
- `internal Task AfterAllPlayersReadyToBeginEnemyTurn(Func<Task> actionDuringEnemyTurn)`
- `internal Task AfterAllPlayersReadyToEndTurn(Func<Task> actionDuringEnemyTurn)`
- `public void AfterCombatRoomLoaded()`
- `public Task AfterCreatureAdded(Creature creature)`
- `public bool AllPlayersReadyToEndTurn()`
- `public Task CheckForEmptyHand(PlayerChoiceContext choiceContext, Player player)`
- `public Task<bool> CheckWinCondition()`
- `public void DebugClearForcedTopCardOnNextShuffle()`
- `public void DebugForceTopCardOnNextShuffle(CardModel card)`
- `public CombatState DebugOnlyGetState()`
- `internal Task DoTurnEnd(Player player, PlayerChoiceContext choiceContext)`
- `public Task EndCombatInternal()`
- `internal Task EndEnemyTurn()`
- `internal Task EndEnemyTurnInternal()`
- `public Task EndPlayerTurnPhaseOneInternal()`
- `public Task EndPlayerTurnPhaseTwoInternal()`
- `internal Task ExecuteEnemyTurn(Func<Task> actionDuringEnemyTurn)`
- `internal Task FlushPlayerHand(Player player, HookPlayerChoiceContext playerChoiceContext)`
- `public Task HandlePlayerDeath(Player player)`
- `public bool IsPartOfPlayerTurn(Player player)`
- `public bool IsPlayerReadyToEndTurn(Player player)`
- `public void LoseCombat()`
- `public void OnEndedTurnLocally()`
- `public void Pause()`
- `internal void ProcessPendingLoss()`
- `public void RemoveCreature(Creature creature)`
- `public void Reset(bool graceful)`
- `internal Task RunAutoPrePlayPhase(HookPlayerChoiceContext playerChoiceContext, Task setupPlayerTurnTask, Player player)`
- `internal void SetPhaseForAllPlayers(PlayerTurnPhase phase)`
- `public void SetReadyToBeginEnemyTurn(Player player, Func<Task> actionDuringEnemyTurn)`
- `public void SetReadyToEndTurn(Player player, bool canBackOut, Func<Task> actionDuringEnemyTurn)`
- `public void SetUpCombat(CombatState state)`
- `internal Task SetupPlayerTurn(Player player, HookPlayerChoiceContext playerChoiceContext)`
- `public Task StartCombatInternal()`
- `internal Task StartTurn(Func<Task> actionDuringEnemyTurn)`
- `public Task SwitchFromPlayerToEnemySide(Func<Task> actionDuringEnemyTurn)`
- `internal void SwitchSides()`
- `public void UndoReadyToEndTurn(Player player)`
- `public void Unpause()`
- `internal Task WaitForActionThenEndTurn(GameAction action, Func<Task> actionDuringEnemyTurn)`
- `public Task WaitForUnpause()`
- `internal Task WaitUntilQueueIsEmptyOrWaitingOnNonPlayerDrivenAction()`

### `enum CombatSide`

Values: `None`, `Player`, `Enemy`

### `sealed class CombatSideExtensions`

### `class CombatState : ICombatState, ICardScope`

**Properties**

- `public IReadOnlyList<Creature> Allies` { get }
- `public IReadOnlyList<BadgeModel> BadgeModels` { get }
- `public IReadOnlyList<Creature> Creatures` { get }
- `public IReadOnlyList<Creature> CreaturesOnCurrentSide` { get }
- `public CombatSide CurrentSide` { get; set }
- `public EncounterModel Encounter` { get; set }
- `public IReadOnlyList<Creature> Enemies` { get }
- `public IReadOnlyList<Creature> EscapedCreatures` { get }
- `public IReadOnlyList<Creature> HittableEnemies` { get }
- `public IReadOnlyList<ModifierModel> Modifiers` { get }
- `public MultiplayerScalingModel MultiplayerScalingModel` { get; set }
- `public IReadOnlyList<Creature> PlayerCreatures` { get }
- `public IReadOnlyList<Player> Players` { get }
- `public int RoundNumber` { get; set }
- `public IRunState RunState` { get }

**Fields**

- `private List<CardModel> _allCards`
- `private List<Creature> _allies`
- `private EncounterModel _encounter`
- `private List<Creature> _enemies`
- `private List<Creature> _escapedCreatures`
- `private UInt32 _nextCreatureId`
- `private Action<ICombatState> CreaturesChanged`

**Methods**

- `public void AddCard(CardModel card, Player owner)`
- `internal void AddCard(CardModel card)`
- `public void AddCreature(Creature creature)`
- `public void AddPlayer(Player player)`
- `internal void AttachCreature(Creature creature)`
- `public CardModel CloneCard(CardModel mutableCard)`
- `internal bool Contains(AbstractModel model)`
- `public bool ContainsCard(CardModel card)`
- `public bool ContainsCreature(Creature creature)`
- `public bool ContainsMonster()`
- `public T CreateCard(Player owner)`
- `public CardModel CreateCard(CardModel canonicalCard, Player owner)`
- `public Creature CreateCreature(MonsterModel monster, CombatSide side, string slot)`
- `public void CreatureEscaped(Creature creature)`
- `public Creature GetCreature(Nullable<UInt32> combatId)`
- `public Task<Creature> GetCreatureAsync(Nullable<UInt32> combatId, double timeoutSec)`
- `public IReadOnlyList<Creature> GetCreaturesOnSide(CombatSide side)`
- `public IReadOnlyList<Creature> GetOpponentsOf(Creature creature)`
- `public Player GetPlayer(UInt64 playerId)`
- `public IReadOnlyList<Creature> GetTeammatesOf(Creature creature)`
- `public bool IsLiveCombat()`
- `public IEnumerable<AbstractModel> IterateHookListeners()`
- `public void RemoveCard(CardModel card)`
- `public void RemoveCreature(Creature creature, bool unattach)`
- `public void SetEnemyIndex(Creature creature, int index)`
- `public void SortEnemiesBySlotName()`

### `class CombatStateTracker`

**Fields**

- `private CombatManager _combatManager`
- `private Task _combatStateChangedDeferredTask`
- `private CombatState _state`
- `private Action<CombatState> CombatStateChanged`

**Methods**

- `internal Task CallCombatStateChangedDeferred()`
- `internal void NotifyCombatStateChanged(string caller)`
- `internal void OnCardPileContentsChanged()`
- `internal void OnCardValueChanged()`
- `internal void OnCombatHistoryChanged()`
- `internal void OnCreatureChanged(Creature _)`
- `internal void OnCreaturesChanged(CombatState _)`
- `internal void OnCreatureValueChanged(int _, int __)`
- `internal void OnPlayerCombatStateValueChanged(int _, int __)`
- `internal void OnPlayerStateChanged()`
- `internal void OnPowerAppliedOrRemoved(PowerModel _)`
- `internal void OnPowerDecreased(PowerModel _, bool __)`
- `internal void OnPowerIncreased(PowerModel _, int __, bool ___)`
- `internal void OnTurnEnded(CombatState _)`
- `internal void OnTurnStarted(CombatState _)`
- `public void SetState(CombatState state)`
- `public void Subscribe(CardModel card)`
- `public void Subscribe(CardPile pile)`
- `public void Subscribe(Creature creature)`
- `public void Subscribe(PlayerCombatState combatState)`
- `public void Unsubscribe(CardModel card)`
- `public void Unsubscribe(CardPile pile)`
- `public void Unsubscribe(Creature creature)`
- `public void Unsubscribe(PlayerCombatState combatState)`

### `interface ICombatState`

**Properties**

- `public IReadOnlyList<Creature> Allies` { get }
- `public IReadOnlyList<Creature> Creatures` { get }
- `public IReadOnlyList<Creature> CreaturesOnCurrentSide` { get }
- `public CombatSide CurrentSide` { get; set }
- `public EncounterModel Encounter` { get }
- `public IReadOnlyList<Creature> Enemies` { get }
- `public IReadOnlyList<Creature> EscapedCreatures` { get }
- `public IReadOnlyList<Creature> HittableEnemies` { get }
- `public IReadOnlyList<ModifierModel> Modifiers` { get }
- `public MultiplayerScalingModel MultiplayerScalingModel` { get }
- `public IReadOnlyList<Creature> PlayerCreatures` { get }
- `public IReadOnlyList<Player> Players` { get }
- `public int RoundNumber` { get; set }
- `public IRunState RunState` { get }

**Methods**

- `public virtual void AddCard(CardModel card, Player owner)`
- `public virtual void AddCreature(Creature creature)`
- `public virtual void AddPlayer(Player player)`
- `public virtual CardModel CloneCard(CardModel mutableCard)`
- `public virtual bool ContainsCard(CardModel card)`
- `public virtual bool ContainsCreature(Creature creature)`
- `public virtual bool ContainsMonster()`
- `public virtual T CreateCard(Player owner)`
- `public virtual CardModel CreateCard(CardModel canonicalCard, Player owner)`
- `public virtual Creature CreateCreature(MonsterModel monster, CombatSide side, string slot)`
- `public virtual void CreatureEscaped(Creature creature)`
- `public virtual Creature GetCreature(Nullable<UInt32> combatId)`
- `public virtual Task<Creature> GetCreatureAsync(Nullable<UInt32> combatId, double timeoutSec)`
- `public virtual IReadOnlyList<Creature> GetCreaturesOnSide(CombatSide side)`
- `public virtual IReadOnlyList<Creature> GetOpponentsOf(Creature creature)`
- `public virtual Player GetPlayer(UInt64 playerId)`
- `public virtual IReadOnlyList<Creature> GetTeammatesOf(Creature creature)`
- `public virtual bool IsLiveCombat()`
- `public virtual IEnumerable<AbstractModel> IterateHookListeners()`
- `public virtual void RemoveCard(CardModel card)`
- `public virtual void RemoveCreature(Creature creature, bool unattach)`
- `public virtual void SetEnemyIndex(Creature creature, int index)`
- `public virtual void SortEnemiesBySlotName()`

### `class NullCombatState : ICombatState`

**Properties**

- `public IReadOnlyList<Creature> Allies` { get }
- `public IReadOnlyList<Creature> Creatures` { get }
- `public IReadOnlyList<Creature> CreaturesOnCurrentSide` { get }
- `public CombatSide CurrentSide` { get; set }
- `public EncounterModel Encounter` { get }
- `public IReadOnlyList<Creature> Enemies` { get }
- `public IReadOnlyList<Creature> EscapedCreatures` { get }
- `public IReadOnlyList<Creature> HittableEnemies` { get }
- `public IReadOnlyList<ModifierModel> Modifiers` { get }
- `public MultiplayerScalingModel MultiplayerScalingModel` { get }
- `public IReadOnlyList<Creature> PlayerCreatures` { get }
- `public IReadOnlyList<Player> Players` { get }
- `public int RoundNumber` { get; set }
- `public IRunState RunState` { get }

**Fields**

- `private Action<ICombatState> CreaturesChanged`

**Methods**

- `public void AddCard(CardModel card, Player owner)`
- `public void AddCreature(Creature creature)`
- `public void AddPlayer(Player player)`
- `public CardModel CloneCard(CardModel mutableCard)`
- `public bool ContainsCard(CardModel card)`
- `public bool ContainsCreature(Creature creature)`
- `public bool ContainsMonster()`
- `public T CreateCard(Player owner)`
- `public CardModel CreateCard(CardModel canonicalCard, Player owner)`
- `public Creature CreateCreature(MonsterModel monster, CombatSide side, string slot)`
- `public void CreatureEscaped(Creature creature)`
- `public Creature GetCreature(Nullable<UInt32> combatId)`
- `public Task<Creature> GetCreatureAsync(Nullable<UInt32> combatId, double timeoutSec)`
- `public IReadOnlyList<Creature> GetCreaturesOnSide(CombatSide side)`
- `public IReadOnlyList<Creature> GetOpponentsOf(Creature creature)`
- `public Player GetPlayer(UInt64 playerId)`
- `public IReadOnlyList<Creature> GetTeammatesOf(Creature creature)`
- `public bool IsLiveCombat()`
- `public IEnumerable<AbstractModel> IterateHookListeners()`
- `public void RemoveCard(CardModel card)`
- `public void RemoveCreature(Creature creature, bool unattach)`
- `public void SetEnemyIndex(Creature creature, int index)`
- `public void SortEnemiesBySlotName()`

### `sealed class PendingLossState : IEquatable<PendingLossState>`

**Properties**

- `internal Type EqualityContract` { get }
- `public CombatRoom Room` { get; set }
- `public CombatState State` { get; set }

**Methods**

- `public void Deconstruct(CombatState& State, CombatRoom& Room)`
- `public virtual bool Equals(object obj)`
- `public bool Equals(PendingLossState other)`
- `public virtual int GetHashCode()`
- `internal bool PrintMembers(StringBuilder builder)`
- `public virtual string ToString()`

### `enum PlayerTurnPhase`

Values: `None`, `Start`, `AutoPrePlay`, `Play`, `AutoPostPlay`, `End`

## MegaCrit.Sts2.Core.Entities.Creatures

### `sealed class <>c`

### `sealed class <>c__124`1`

### `sealed class <>c__126`1`

### `sealed class <>c__DisplayClass125_0`

**Fields**

- `public ModelId id`

### `sealed class <>c__DisplayClass127_0`

**Fields**

- `public ModelId id`

### `sealed class <>c__DisplayClass129_0`

**Fields**

- `public ModelId id`

### `sealed class <>c__DisplayClass130_0`

**Fields**

- `public ModelId id`

### `sealed class <>c__DisplayClass132_0`

**Fields**

- `public PowerModel power`

### `sealed class <AfterAddedToRoom>d__110 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <AfterTurnStart>d__138 : ValueType, IAsyncStateMachine`

**Fields**

- `public int roundNumber`
- `public CombatSide side`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <ClearBlock>d__141 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <TakeTurn>d__140 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `class Creature`

**Properties**

- `public int Block` { get; set }
- `public bool CanReceivePowers` { get }
- `public Nullable<UInt32> CombatId` { get; set }
- `public ICombatState CombatState` { get; set }
- `public int CurrentHp` { get; set }
- `public IEnumerable<IHoverTip> HoverTips` { get }
- `public HpDisplay HpDisplay` { get; set }
- `public bool IsAlive` { get }
- `public bool IsDead` { get }
- `public bool IsEnemy` { get }
- `public bool IsHittable` { get }
- `public bool IsMonster` { get }
- `public bool IsPet` { get }
- `public bool IsPlayer` { get }
- `public bool IsPrimaryEnemy` { get }
- `public bool IsSecondaryEnemy` { get }
- `public bool IsStunned` { get }
- `public string LogName` { get }
- `public int MaxHp` { get; set }
- `public ModelId ModelId` { get }
- `public MonsterModel Monster` { get }
- `public Nullable<int> MonsterMaxHpBeforeModification` { get; set }
- `public string Name` { get }
- `public Player PetOwner` { get; set }
- `public IReadOnlyList<Creature> Pets` { get }
- `public Player Player` { get }
- `public IReadOnlyList<PowerModel> Powers` { get }
- `public CombatSide Side` { get }
- `public string SlotName` { get; set }

**Fields**

- `private int _block`
- `private int _currentHp`
- `private int _maxHp`
- `private Player _petOwner`
- `private List<PowerModel> _powers`
- `private Action<int, int> BlockChanged`
- `private Action<int, int> CurrentHpChanged`
- `private Action<Creature> Died`
- `private Action<int, int> MaxHpChanged`
- `private Action<PowerModel> PowerApplied`
- `private Action<PowerModel, bool> PowerDecreased`
- `private Action<PowerModel, int, bool> PowerIncreased`
- `private Action<PowerModel> PowerRemoved`
- `private Action<Creature> Revived`

**Methods**

- `public Task AfterAddedToRoom()`
- `public Task AfterTurnStart(int roundNumber, CombatSide side)`
- `public void ApplyPowerInternal(PowerModel power)`
- `public void BeforeTurnStart(int roundNumber, CombatSide side)`
- `internal Task ClearBlock()`
- `public NCreatureVisuals CreateVisuals()`
- `public decimal DamageBlockInternal(decimal amount, ValueProp props)`
- `public void GainBlockInternal(decimal amount)`
- `public Control GetBackVfxContainer()`
- `public NCreature GetCreatureNode()`
- `public double GetHpPercentRemaining()`
- `public T GetPower()`
- `public PowerModel GetPower(ModelId id)`
- `public int GetPowerAmount()`
- `public PowerModel GetPowerById(ModelId id)`
- `public IEnumerable<T> GetPowerInstances()`
- `public IEnumerable<PowerModel> GetPowerInstances(ModelId id)`
- `public Control GetVfxContainer()`
- `public bool HasPower()`
- `public bool HasPower(ModelId id)`
- `public void HealInternal(decimal amount)`
- `public void InvokeDiedEvent()`
- `public void InvokePowerModified(PowerModel power, int change, bool silent)`
- `public void LoseBlockInternal(decimal amount)`
- `public DamageResult LoseHpInternal(decimal amount, ValueProp props)`
- `public void OnSideSwitch()`
- `public void PrepareForNextTurn(IEnumerable<Creature> targets, bool rollNewMove)`
- `public IEnumerable<PowerModel> RemoveAllPowersAfterDeath()`
- `public IEnumerable<PowerModel> RemoveAllPowersInternalExcept(IEnumerable<PowerModel> except)`
- `public void RemovePowerInternal(PowerModel power)`
- `public void Reset()`
- `public void ScaleMonsterHpForMultiplayer(EncounterModel encounter, int playerCount, int actIndex)`
- `public void SetCurrentHpInternal(decimal amount)`
- `public void SetMaxHpInternal(decimal amount)`
- `public void SetUniqueMonsterHpValue(IReadOnlyList<Creature> creaturesOnSide, Rng rng)`
- `public void StunInternal(Func<IReadOnlyList<Creature>, Task> stunMove, string nextMoveId)`
- `public Task TakeTurn()`
- `public virtual string ToString()`

### `class DamageResult`

**Properties**

- `public int BlockedDamage` { get; set }
- `public int OverkillDamage` { get; set }
- `public ValueProp Props` { get }
- `public Creature Receiver` { get }
- `public int TotalDamage` { get }
- `public int UnblockedDamage` { get; set }
- `public bool WasBlockBroken` { get; set }
- `public bool WasFullyBlocked` { get; set }
- `public bool WasTargetKilled` { get; set }

### `enum HpDisplay`

Values: `Normal`, `InfiniteWithNumbers`, `InfiniteWithoutNumbers`

### `sealed class HpDisplayExtensions`

### `class SummonResult`

**Properties**

- `public decimal Amount` { get; set }
- `public Creature Creature` { get; set }

## MegaCrit.Sts2.Core.Models

### `sealed class <<RelicOption>g__OnChosen|0>d : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c__DisplayClass26_0`

**Fields**

- `public ICombatState combatState`

### `sealed class <>c__DisplayClass26_1`

**Fields**

- `public string s`

### `sealed class <>c__DisplayClass69_0`

**Fields**

- `public string customDonePage`
- `public RelicModel relic`

### `sealed class <>c__DisplayClass88_0`

**Fields**

- `public string backgroundsPath`

### `sealed class <>c__DisplayClass89_0`

**Fields**

- `public EventModel e`

### `sealed class <>c__DisplayClass89_1`

**Fields**

- `public EventModel e`

### `sealed class <>c__DisplayClass89_2`

**Fields**

- `public EventModel e`

### `sealed class <>c__DisplayClass93_0`

**Fields**

- `public ICollection<EncounterModel> encounters`

### `sealed class <>O`

### `sealed class <BeforeEventStarted>d__62 : ValueType, IAsyncStateMachine`

**Fields**

- `public bool isPreFinished`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <BeginEvent>d__29 : ValueType, IAsyncStateMachine`

**Fields**

- `public bool isPreFinished`
- `public Player player`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <GetAllMoves>d__93 : IEnumerable<string>, IEnumerable, IEnumerator<string>, IEnumerator, IDisposable`

**Properties**

- `internal string System.Collections.Generic.IEnumerator<System.String>.Current` { get }
- `internal object System.Collections.IEnumerator.Current` { get }

**Fields**

- `private MonsterMoveStateMachine machine`

**Methods**

- `internal bool MoveNext()`
- `internal IEnumerator<string> System.Collections.Generic.IEnumerable<System.String>.GetEnumerator()`
- `internal IEnumerator System.Collections.IEnumerable.GetEnumerator()`
- `internal void System.Collections.IEnumerator.Reset()`
- `internal void System.IDisposable.Dispose()`

### `sealed class <MoveToResultPileWithoutPlaying>d__336 : ValueType, IAsyncStateMachine`

**Fields**

- `public PlayerChoiceContext choiceContext`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <OnPlayWrapper>d__333 : ValueType, IAsyncStateMachine`

**Fields**

- `public PlayerChoiceContext choiceContext`
- `public bool isAutoPlay`
- `public ResourceInfo resources`
- `public bool skipCardPileVisuals`
- `public Creature target`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <OnTurnEndInHandWrapper>d__322 : ValueType, IAsyncStateMachine`

**Fields**

- `public PlayerChoiceContext choiceContext`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <OnUseWrapper>d__70 : ValueType, IAsyncStateMachine`

**Fields**

- `public PlayerChoiceContext choiceContext`
- `public Creature target`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <PerformMove>d__101 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <PlayPowerCardFlyVfx>d__334 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SpendEnergy>d__331 : ValueType, IAsyncStateMachine`

**Fields**

- `public int amount`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SpendResources>d__330 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SpendStars>d__332 : ValueType, IAsyncStateMachine`

**Fields**

- `public int amount`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `abstract class AbstractModel : IComparable<AbstractModel>`

**Properties**

- `public int CategorySortingId` { get; set }
- `public int EntrySortingId` { get; set }
- `public ModelId Id` { get }
- `public bool IsCanonical` { get }
- `public bool IsMutable` { get; set }
- `public bool PreviewOutsideOfCombat` { get }
- `public bool ShouldReceiveCombatHooks` { get }

**Fields**

- `private Action<AbstractModel> ExecutionFinished`

**Methods**

- `public virtual Task AfterActEntered()`
- `public virtual Task AfterAddToDeckPrevented(CardModel card)`
- `public virtual Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)`
- `public virtual Task AfterAutoPostPlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)`
- `public virtual Task AfterAutoPrePlayPhaseEntered(PlayerChoiceContext choiceContext, Player player)`
- `public virtual Task AfterAutoPrePlayPhaseEnteredEarly(PlayerChoiceContext choiceContext, Player player)`
- `public virtual Task AfterAutoPrePlayPhaseEnteredLate(PlayerChoiceContext choiceContext, Player player)`
- `public virtual Task AfterBlockBroken(Creature creature)`
- `public virtual Task AfterBlockCleared(Creature creature)`
- `public virtual Task AfterBlockGained(Creature creature, decimal amount, ValueProp props, CardModel cardSource)`
- `public virtual Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel source)`
- `public virtual Task AfterCardChangedPilesLate(CardModel card, PileType oldPileType, AbstractModel source)`
- `public virtual Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)`
- `public virtual Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)`
- `public virtual Task AfterCardDrawnEarly(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)`
- `public virtual Task AfterCardEnteredCombat(CardModel card)`
- `public virtual Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)`
- `public virtual Task AfterCardGeneratedForCombat(CardModel card, Player creator)`
- `public virtual Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)`
- `public virtual Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)`
- `protected virtual void AfterCloned()`
- `public virtual Task AfterCombatEnd(CombatRoom room)`
- `public virtual Task AfterCombatVictory(CombatRoom room)`
- `public virtual Task AfterCombatVictoryEarly(CombatRoom room)`
- `public virtual Task AfterCreatureAddedToCombat(Creature creature)`
- `public virtual Task AfterCurrentHpChanged(Creature creature, decimal delta)`
- `public virtual Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature dealer, DamageResult result, ValueProp props, Creature target, CardModel cardSource)`
- `public virtual Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature dealer, CardModel cardSource)`
- `public virtual Task AfterDamageReceivedLate(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature dealer, CardModel cardSource)`
- `public virtual Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)`
- `public virtual Task AfterDiedToDoom(PlayerChoiceContext choiceContext, IReadOnlyList<Creature> creatures)`
- `public virtual Task AfterEnergyReset(Player player)`
- `public virtual Task AfterEnergyResetLate(Player player)`
- `public virtual Task AfterEnergySpent(CardModel card, int amount)`
- `public virtual Task AfterFlush(PlayerChoiceContext choiceContext, Player player, IReadOnlyCollection<CardModel> flushedCards, IReadOnlyCollection<CardModel> retainedCards)`
- `public virtual Task AfterForge(decimal amount, Player forger, AbstractModel source)`
- `public virtual Task AfterGoldGained(Player player)`
- `public virtual Task AfterHandEmptied(PlayerChoiceContext choiceContext, Player player)`
- `public virtual Task AfterItemPurchased(Player player, MerchantEntry itemPurchased, int goldSpent)`
- `public virtual Task AfterMapGenerated(ActMap map, int actIndex)`
- `public virtual Task AfterModifyingBlockAmount(decimal modifiedAmount, CardModel cardSource, CardPlay cardPlay)`
- `public virtual Task AfterModifyingCardPlayCount(CardModel card)`
- `public virtual Task AfterModifyingCardPlayResultPileOrPosition(CardModel card, PileType pileType, CardPilePosition position)`
- `public virtual Task AfterModifyingCardRewardOptions()`
- `public virtual Task AfterModifyingDamageAmount(CardModel cardSource)`
- `public virtual Task AfterModifyingEnergyGain()`
- `public virtual Task AfterModifyingHandDraw()`
- `public virtual Task AfterModifyingHpLostAfterOsty()`
- `public virtual Task AfterModifyingHpLostBeforeOsty()`
- `public virtual Task AfterModifyingOrbPassiveTriggerCount(OrbModel orb)`
- `public virtual Task AfterModifyingPowerAmountGiven(PowerModel power)`
- `public virtual Task AfterModifyingPowerAmountReceived(PowerModel power)`
- `public virtual Task AfterModifyingRewards()`
- `public virtual Task AfterOrbChanneled(PlayerChoiceContext choiceContext, Player player, OrbModel orb)`
- `public virtual Task AfterOrbEvoked(PlayerChoiceContext choiceContext, OrbModel orb, IEnumerable<Creature> targets)`
- `public virtual Task AfterOstyRevived(Creature osty)`
- `public virtual Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)`
- `public virtual Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)`
- `public virtual Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)`
- `public virtual Task AfterPotionDiscarded(PotionModel potion)`
- `public virtual Task AfterPotionProcured(PotionModel potion)`
- `public virtual Task AfterPotionUsed(PotionModel potion, Creature target)`
- `public virtual Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature applier, CardModel cardSource)`
- `public virtual Task AfterPreventingBlockClear(AbstractModel preventer, Creature creature)`
- `public virtual Task AfterPreventingDeath(Creature creature)`
- `public virtual Task AfterPreventingDraw()`
- `public virtual Task AfterRestSiteHeal(Player player, bool isMimicked)`
- `public virtual Task AfterRestSiteSmith(Player player)`
- `public virtual Task AfterRewardTaken(Player player, Reward reward)`
- `public virtual Task AfterRoomEntered(AbstractRoom room)`
- `public virtual Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)`
- `public virtual Task AfterSideTurnStart(CombatSide side, ICombatState combatState)`
- `public virtual Task AfterSideTurnStartLate(CombatSide side, ICombatState combatState)`
- `public virtual Task AfterStarsGained(int amount, Player gainer)`
- `public virtual Task AfterStarsSpent(int amount, Player spender)`
- `public virtual Task AfterSummon(PlayerChoiceContext choiceContext, Player summoner, decimal amount)`
- `public virtual Task AfterTakingExtraTurn(Player player)`
- `public virtual Task AfterTargetingBlockedVfx(Creature blocker)`
- `public virtual Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)`
- `public virtual Task AfterTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side)`
- `public void AssertCanonical()`
- `public void AssertMutable()`
- `public virtual Task BeforeAttack(AttackCommand command)`
- `public virtual Task BeforeBlockGained(Creature creature, decimal amount, ValueProp props, CardModel cardSource)`
- `public virtual Task BeforeCardAutoPlayed(CardModel card, Creature target, AutoPlayType type)`
- `public virtual Task BeforeCardPlayed(CardPlay cardPlay)`
- `public virtual Task BeforeCardRemoved(CardModel card)`
- `public virtual Task BeforeCombatStart()`
- `public virtual Task BeforeCombatStartLate()`
- `public virtual Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource)`
- `public virtual Task BeforeDeath(Creature creature)`
- `public virtual Task BeforeFlush(PlayerChoiceContext choiceContext, Player player)`
- `public virtual Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)`
- `public virtual Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)`
- `public virtual Task BeforeHandDrawLate(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)`
- `public virtual Task BeforePotionUsed(PotionModel potion, Creature target)`
- `public virtual Task BeforePowerAmountChanged(PowerModel power, decimal amount, Creature target, Creature applier, CardModel cardSource)`
- `public virtual Task BeforeRoomEntered(AbstractRoom room)`
- `public virtual Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, ICombatState combatState)`
- `public virtual Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)`
- `public virtual Task BeforeTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side)`
- `public virtual Task BeforeTurnEndVeryEarly(PlayerChoiceContext choiceContext, CombatSide side)`
- `public AbstractModel ClonePreservingMutability()`
- `public virtual int CompareTo(AbstractModel other)`
- `protected virtual void DeepCloneFields()`
- `public void InitId(ModelId id)`
- `public void InvokeExecutionFinished()`
- `public virtual int ModifyAttackHitCount(AttackCommand attack, int hitCount)`
- `public virtual decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel cardSource, CardPlay cardPlay)`
- `public virtual decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel cardSource, CardPlay cardPlay)`
- `public virtual int ModifyCardPlayCount(CardModel card, Creature target, int playCount)`
- `public virtual ValueTuple<PileType, CardPilePosition> ModifyCardPlayResultPileTypeAndPosition(CardModel card, bool isAutoPlay, ResourceInfo resources, PileType pileType, CardPilePosition position)`
- `public virtual CardCreationOptions ModifyCardRewardCreationOptions(Player player, CardCreationOptions options)`
- `public virtual CardCreationOptions ModifyCardRewardCreationOptionsLate(Player player, CardCreationOptions options)`
- `public virtual decimal ModifyCardRewardUpgradeOdds(Player player, CardModel card, decimal odds)`
- `public virtual decimal ModifyDamageAdditive(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource)`
- `public virtual decimal ModifyDamageCap(Creature target, ValueProp props, Creature dealer, CardModel cardSource)`
- `public virtual decimal ModifyDamageMultiplicative(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource)`
- `public virtual decimal ModifyEnergyGain(Player player, decimal amount)`
- `public virtual IReadOnlyList<LocString> ModifyExtraRestSiteHealText(Player player, IReadOnlyList<LocString> currentExtraText)`
- `public virtual ActMap ModifyGeneratedMap(IRunState runState, ActMap map, int actIndex)`
- `public virtual ActMap ModifyGeneratedMapLate(IRunState runState, ActMap map, int actIndex)`
- `public virtual decimal ModifyHandDraw(Player player, decimal count)`
- `public virtual decimal ModifyHandDrawLate(Player player, decimal count)`
- `public virtual decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource)`
- `public virtual decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource)`
- `public virtual decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource)`
- `public virtual decimal ModifyHpLostBeforeOstyLate(Creature target, decimal amount, ValueProp props, Creature dealer, CardModel cardSource)`
- `public virtual decimal ModifyMaxEnergy(Player player, decimal amount)`
- `public virtual void ModifyMerchantCardCreationResults(Player player, List<CardCreationResult> cards)`
- `public virtual IEnumerable<CardModel> ModifyMerchantCardPool(Player player, IEnumerable<CardModel> options)`
- `public virtual CardRarity ModifyMerchantCardRarity(Player player, CardRarity rarity)`
- `public virtual decimal ModifyMerchantPrice(Player player, MerchantEntry entry, decimal cost)`
- `public virtual EventModel ModifyNextEvent(EventModel currentEvent)`
- `public virtual float ModifyOddsIncreaseForUnrolledRoomType(RoomType roomType, float oddsIncrease)`
- `public virtual int ModifyOrbPassiveTriggerCounts(OrbModel orb, int triggerCount)`
- `public virtual decimal ModifyOrbValue(OrbModel orb, decimal value)`
- `public virtual decimal ModifyPowerAmountGiven(PowerModel power, Creature giver, decimal amount, Creature target, CardModel cardSource)`
- `public virtual decimal ModifyRestSiteHealAmount(Creature creature, decimal amount)`
- `public virtual void ModifyShuffleOrder(Player player, List<CardModel> cards, bool isInitialShuffle)`
- `public virtual decimal ModifySummonAmount(Player summoner, decimal amount, AbstractModel source)`
- `public virtual Creature ModifyUnblockedDamageTarget(Creature target, decimal amount, ValueProp props, Creature dealer)`
- `public virtual IReadOnlySet<RoomType> ModifyUnknownMapPointRoomTypes(IReadOnlySet<RoomType> roomTypes)`
- `public virtual int ModifyXValue(CardModel card, int originalValue)`
- `public AbstractModel MutableClone()`
- `protected void NeverEverCallThisOutsideOfTests_SetIsMutable(bool isMutable)`
- `public virtual bool ShouldAddToDeck(CardModel card)`
- `public virtual bool ShouldAfflict(CardModel card, AfflictionModel affliction)`
- `public virtual bool ShouldAllowAncient(Player player, AncientEventModel ancient)`
- `public virtual bool ShouldAllowFreeTravel()`
- `public virtual bool ShouldAllowHitting(Creature creature)`
- `public virtual bool ShouldAllowMerchantCardRemoval(Player player)`
- `public virtual bool ShouldAllowSelectingMoreCardRewards(Player player, CardReward cardReward)`
- `public virtual bool ShouldAllowTargeting(Creature target)`
- `public virtual bool ShouldClearBlock(Creature creature)`
- `public virtual bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)`
- `public virtual bool ShouldDie(Creature creature)`
- `public virtual bool ShouldDieLate(Creature creature)`
- `public virtual bool ShouldDisableRemainingRestSiteOptions(Player player)`
- `public virtual bool ShouldDraw(Player player, bool fromHandDraw)`
- `public virtual bool ShouldEtherealTrigger(CardModel card)`
- `public virtual bool ShouldFlush(Player player)`
- `public virtual bool ShouldForcePotionReward(Player player, RoomType roomType)`
- `public virtual bool ShouldGainGold(decimal amount, Player player)`
- `public virtual bool ShouldGainStars(decimal amount, Player player)`
- `public virtual bool ShouldGenerateTreasure(Player player)`
- `public virtual bool ShouldPayExcessEnergyCostWithStars(Player player)`
- `public virtual bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)`
- `public virtual bool ShouldPlayerResetEnergy(Player player)`
- `public virtual bool ShouldPowerBeRemovedOnDeath(PowerModel power)`
- `public virtual bool ShouldProceedToNextMapPoint()`
- `public virtual bool ShouldProcurePotion(PotionModel potion, Player player)`
- `public virtual bool ShouldRefillMerchantEntry(MerchantEntry entry, Player player)`
- `public virtual bool ShouldStopCombatFromEnding()`
- `public virtual bool ShouldTakeExtraTurn(Player player)`
- `public virtual string ToString()`
- `public virtual bool TryModifyCardBeingAddedToDeck(CardModel card, CardModel& newCard)`
- `public virtual bool TryModifyCardBeingAddedToDeckLate(CardModel card, CardModel& newCard)`
- `public virtual bool TryModifyCardRewardAlternatives(Player player, CardReward cardReward, List<CardRewardAlternative> alternatives)`
- `public virtual bool TryModifyCardRewardOptions(Player player, List<CardCreationResult> cardRewardOptions, CardCreationOptions creationOptions)`
- `public virtual bool TryModifyCardRewardOptionsLate(Player player, List<CardCreationResult> cardRewardOptions, CardCreationOptions creationOptions)`
- `public virtual bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, Decimal& modifiedCost)`
- `public virtual bool TryModifyEnergyCostInCombatLate(CardModel card, decimal originalCost, Decimal& modifiedCost)`
- `public virtual bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature applier, Decimal& modifiedAmount)`
- `public virtual bool TryModifyRestSiteHealRewards(Player player, List<Reward> rewards, bool isMimicked)`
- `public virtual bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)`
- `public virtual bool TryModifyRewards(Player player, List<Reward> rewards, AbstractRoom room)`
- `public virtual bool TryModifyRewardsLate(Player player, List<Reward> rewards, AbstractRoom room)`
- `public virtual bool TryModifyStarCost(CardModel card, decimal originalCost, Decimal& modifiedCost)`

### `sealed class AbstractModelSubtypes`

### `abstract class AchievementModel : AbstractModel`

**Properties**

- `public bool ShouldReceiveCombatHooks` { get }

### `abstract class ActModel : AbstractModel`

**Properties**

- `public IEnumerable<AncientEventModel> AllAncients` { get }
- `public IEnumerable<EncounterModel> AllBossEncounters` { get }
- `public IEnumerable<EncounterModel> AllEliteEncounters` { get }
- `public IEnumerable<EncounterModel> AllEncounters` { get }
- `public IEnumerable<EventModel> AllEvents` { get }
- `public IEnumerable<MonsterModel> AllMonsters` { get }
- `public IEnumerable<EncounterModel> AllRegularEncounters` { get }
- `public IEnumerable<EncounterModel> AllWeakEncounters` { get }
- `public string AmbientSfx` { get }
- `public AncientEventModel Ancient` { get }
- `public IEnumerable<string> AssetPaths` { get }
- `public string BackgroundScenePath` { get }
- `internal int BaseNumberOfRooms` { get }
- `public String[] BgMusicOptions` { get }
- `public IEnumerable<EncounterModel> BossDiscoveryOrder` { get }
- `public EncounterModel BossEncounter` { get }
- `public ActModel CanonicalInstance` { get; set }
- `public string ChestOpenSfx` { get }
- `public MegaSkeletonDataResource ChestSpineResource` { get }
- `public string ChestSpineResourcePath` { get }
- `public string ChestSpineSkinNameNormal` { get }
- `public string ChestSpineSkinNameStroke` { get }
- `public Achievement DefeatedAllEnemiesAchievement` { get }
- `internal string FilePathIdentifier` { get }
- `public bool HasSecondBoss` { get }
- `public Color MapBgColor` { get }
- `public Texture2D MapBotBg` { get }
- `public string MapBotBgPath` { get }
- `public Texture2D MapMidBg` { get }
- `public string MapMidBgPath` { get }
- `public Texture2D MapTopBg` { get }
- `public string MapTopBgPath` { get }
- `public Color MapTraveledColor` { get }
- `public Color MapUntraveledColor` { get }
- `public String[] MusicBankPaths` { get }
- `internal int NumberOfWeakEncounters` { get }
- `public string RestSiteBackgroundPath` { get }
- `public EncounterModel SecondBossEncounter` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public LocString Title` { get }

**Fields**

- `private IEnumerable<EncounterModel> _allBossEncounters`
- `private IEnumerable<EncounterModel> _allEliteEncounters`
- `private IEnumerable<EncounterModel> _allEncounters`
- `private IEnumerable<MonsterModel> _allMonsters`
- `private IEnumerable<EncounterModel> _allRegularEncounters`
- `private IEnumerable<EncounterModel> _allWeakEncounters`
- `private ActModel _canonicalInstance`
- `protected RoomSet _rooms`
- `private List<AncientEventModel> _sharedAncientSubset`

**Methods**

- `protected virtual void ApplyActDiscoveryOrderModifications(UnlockState unlockState)`
- `public void ApplyDiscoveryOrderModifications(UnlockState unlockState)`
- `public ActMap CreateMap(RunState runState, bool replaceTreasureWithElites)`
- `public Control CreateRestSiteBackground()`
- `protected virtual void DeepCloneFields()`
- `public virtual IEnumerable<EncounterModel> GenerateAllEncounters()`
- `public BackgroundAssets GenerateBackgroundAssets(Rng rng)`
- `public void GenerateRooms(Rng rng, UnlockState unlockState, bool isMultiplayer)`
- `public IEnumerable<string> GetAllBackgroundLayerPaths()`
- `protected string GetFullLayerPath(string layerName)`
- `public virtual MapPointTypeCounts GetMapPointTypes(Rng mapRng)`
- `public int GetNumberOfFloors(bool isMultiplayer)`
- `public int GetNumberOfRooms(bool isMultiplayer)`
- `public virtual IEnumerable<AncientEventModel> GetUnlockedAncients(UnlockState state)`
- `public void MarkRoomVisited(RoomType roomType)`
- `public EventModel PullAncient()`
- `public EncounterModel PullNextEncounter(RoomType roomType)`
- `public EventModel PullNextEvent(RunState runState)`
- `public void RemoveEventFromSet(EventModel eventModel)`
- `public void SetBossEncounter(EncounterModel encounter)`
- `public void SetSecondBossEncounter(EncounterModel encounter)`
- `public void SetSharedAncientSubset(List<AncientEventModel> sharedAncientSubset)`
- `public ActModel ToMutable()`
- `public SerializableActModel ToSave()`
- `public void ValidateRoomsAfterLoad(Rng rng)`

### `abstract class AfflictionModel : AbstractModel`

**Properties**

- `public int Amount` { get; set }
- `public bool CanAfflictUnplayableCards` { get }
- `public AfflictionModel CanonicalInstance` { get; set }
- `public CardModel Card` { get; set }
- `public ICombatState CombatState` { get }
- `internal LocString Description` { get }
- `public LocString DynamicDescription` { get }
- `public LocString DynamicExtraCardText` { get }
- `internal LocString ExtraCardText` { get }
- `internal IEnumerable<IHoverTip> ExtraHoverTips` { get }
- `public bool HasCard` { get }
- `public bool HasExtraCardText` { get }
- `public bool HasOverlay` { get }
- `public HoverTip HoverTip` { get }
- `public IEnumerable<IHoverTip> HoverTips` { get }
- `public bool IsStackable` { get }
- `public string OverlayPath` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public LocString Title` { get }

**Fields**

- `private int _amount`
- `private AfflictionModel _canonicalInstance`
- `private CardModel _card`
- `private Action<int, int> AmountChanged`

**Methods**

- `public virtual void AfterApplied()`
- `protected virtual void AfterCloned()`
- `public virtual void BeforeRemoved()`
- `public virtual bool CanAfflict(CardModel card)`
- `public virtual bool CanAfflictCardType(CardType cardType)`
- `public void ClearInternal()`
- `public Control CreateOverlay()`
- `public virtual Task OnPlay(PlayerChoiceContext choiceContext, Creature target)`
- `public IReadOnlyList<CardModel> PickRandomTargets(RunRngSet rngSet, IEnumerable<CardModel> cards, int count)`
- `public AfflictionModel ToMutable()`

### `abstract class AncientEventModel : EventModel`

**Properties**

- `public IEnumerable<EventOption> AllPossibleOptions` { get }
- `public string AmbientBgm` { get }
- `public IEnumerable<CharacterModel> AnyCharacterDialogueBlacklist` { get }
- `public Color ButtonColor` { get }
- `internal string CustomDonePage` { get; set }
- `public string DebugOption` { get; set }
- `public Color DialogueColor` { get }
- `public AncientDialogueSet DialogueSet` { get }
- `public LocString Epithet` { get }
- `internal Color EventButtonColor` { get; set }
- `internal List<EventOption> GeneratedOptions` { get; set }
- `public bool HasAmbientBgm` { get }
- `public int HealedAmount` { get; set }
- `public LocString InitialDescription` { get }
- `public EventLayoutType LayoutType` { get }
- `public string LocTable` { get }
- `public Texture2D MapIcon` { get }
- `public Texture2D MapIconOutline` { get }
- `internal string MapIconOutlinePath` { get }
- `internal string MapIconPath` { get }
- `public IEnumerable<string> MapNodeAssetPaths` { get }
- `public Texture2D RunHistoryIcon` { get }
- `public Texture2D RunHistoryIconOutline` { get }
- `internal string RunHistoryIconOutlinePath` { get }

**Fields**

- `private string _customDonePage`
- `private string _debugOption`
- `private AncientDialogueSet _dialogueSet`
- `private List<EventOption> _generatedOptions`

**Methods**

- `protected virtual Task BeforeEventStarted(bool isPreFinished)`
- `protected virtual AncientDialogueSet DefineDialogues()`
- `protected void Done()`
- `protected IReadOnlyList<EventOption> GenerateInitialOptionsWrapper()`
- `protected EventOption RelicOption(string pageName, string customDonePage)`
- `protected EventOption RelicOption(RelicModel relic, string pageName, string customDonePage)`
- `protected virtual void SetInitialEventState(bool isPreFinished)`
- `public void StartPreFinished()`
- `internal void UpdateRunHistory()`

### `abstract class BadgeModel : AbstractModel`

**Properties**

- `public bool ShouldReceiveCombatHooks` { get }

### `sealed class BestiaryMonsterMove : ValueType`

**Fields**

- `public Func<Task> action`
- `public string animId`
- `public string displayName`
- `public Func<IReadOnlyList<Creature>, Task> nonStateMove`
- `public string sfx`
- `public string stateId`
- `public bool stopSfxLoops`

**Methods**

- `public BestiaryMonsterMove StopOtherSfx()`

### `abstract class CardModel : AbstractModel`

**Properties**

- `public AfflictionModel Affliction` { get; set }
- `public IEnumerable<string> AllPortraitPaths` { get }
- `public Texture2D AncientBorder` { get }
- `public Texture2D AncientTextBg` { get }
- `internal string AncientTextBgPath` { get }
- `public Material BannerMaterial` { get }
- `internal string BannerMaterialPath` { get }
- `public Texture2D BannerTexture` { get }
- `internal string BannerTexturePath` { get }
- `public int BaseReplayCount` { get; set }
- `public int BaseStarCost` { get; set }
- `public string BetaPortraitPath` { get }
- `internal string BetaPortraitPngPath` { get }
- `public bool CanBeGeneratedByModifiers` { get }
- `public bool CanBeGeneratedInCombat` { get }
- `internal int CanonicalEnergyCost` { get }
- `public CardModel CanonicalInstance` { get; set }
- `public IEnumerable<CardKeyword> CanonicalKeywords` { get }
- `public int CanonicalStarCost` { get }
- `internal HashSet<CardTag> CanonicalTags` { get }
- `internal IEnumerable<DynamicVar> CanonicalVars` { get }
- `public ICardScope CardScope` { get }
- `public CardModel CloneOf` { get }
- `public ICombatState CombatState` { get }
- `public int CurrentStarCost` { get }
- `public Creature CurrentTarget` { get; set }
- `public int CurrentUpgradeLevel` { get; set }
- `public CardModel DeckVersion` { get; set }
- `public LocString Description` { get }
- `public CardModel DupeOf` { get }
- `public DynamicVarSet DynamicVars` { get }
- `public EnchantmentModel Enchantment` { get; set }
- `public CardEnergyCost EnergyCost` { get }
- `internal IHoverTip EnergyHoverTip` { get }
- `public Texture2D EnergyIcon` { get }
- `internal string EnergyIconPath` { get }
- `public bool ExhaustOnNextPlay` { get; set }
- `internal IEnumerable<IHoverTip> ExtraHoverTips` { get }
- `internal IEnumerable<string> ExtraRunAssetPaths` { get }
- `public Nullable<int> FloorAddedToDeck` { get; set }
- `public Texture2D Frame` { get }
- `public Material FrameMaterial` { get }
- `internal string FramePath` { get }
- `public bool GainsBlock` { get }
- `public bool HasBeenRemovedFromState` { get; set }
- `public bool HasBetaPortrait` { get }
- `public bool HasBuiltInOverlay` { get }
- `internal bool HasEnergyCostX` { get }
- `public bool HasPortrait` { get }
- `internal bool HasSingleTurnRetain` { get; set }
- `internal bool HasSingleTurnSly` { get; set }
- `public bool HasStarCostX` { get }
- `public bool HasTurnEndInHandEffect` { get }
- `public IEnumerable<IHoverTip> HoverTips` { get }
- `public bool IsBasicStrikeOrDefend` { get }
- `public bool IsClone` { get }
- `public bool IsDupe` { get; set }
- `public bool IsEnchantmentPreview` { get; set }
- `public bool IsInCombat` { get }
- `internal bool IsPlayable` { get }
- `public bool IsRemovable` { get }
- `public bool IsSlyThisTurn` { get }
- `public bool IsTransformable` { get }
- `public bool IsUpgradable` { get }
- `public bool IsUpgraded` { get }
- `public IReadOnlySet<CardKeyword> Keywords` { get }
- `public int LastStarsSpent` { get; set }
- `public int MaxUpgradeLevel` { get }
- `public CardMultiplayerConstraint MultiplayerConstraint` { get }
- `public OrbEvokeType OrbEvokeType` { get }
- `public string OverlayPath` { get }
- `public Player Owner` { get; set }
- `public CardPile Pile` { get }
- `public CardPoolModel Pool` { get }
- `public Texture2D Portrait` { get }
- `public Texture2D PortraitBorder` { get }
- `internal string PortraitBorderPath` { get }
- `public string PortraitPath` { get }
- `internal string PortraitPngPath` { get }
- `public CardRarity Rarity` { get }
- `public IEnumerable<string> RunAssetPaths` { get }
- `public IRunState RunState` { get }
- `internal LocString SelectionScreenPrompt` { get }
- `public bool ShouldGlowGold` { get }
- `internal bool ShouldGlowGoldInternal` { get }
- `public bool ShouldGlowRed` { get }
- `internal bool ShouldGlowRedInternal` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public bool ShouldRetainThisTurn` { get }
- `public bool ShouldShowInCardLibrary` { get }
- `public IEnumerable<CardTag> Tags` { get }
- `public TargetType TargetType` { get }
- `public TemporaryCardCost TemporaryStarCost` { get }
- `public string Title` { get }
- `public LocString TitleLocString` { get }
- `public CardType Type` { get }
- `public CardUpgradePreviewType UpgradePreviewType` { get; set }
- `public CardPoolModel VisualCardPool` { get }
- `public bool WasStarCostJustUpgraded` { get }

**Fields**

- `private int _baseReplayCount`
- `private int _baseStarCost`
- `private CardModel _canonicalInstance`
- `private CardModel _cloneOf`
- `private Creature _currentTarget`
- `private int _currentUpgradeLevel`
- `private CardModel _deckVersion`
- `private DynamicVarSet _dynamicVars`
- `private CardEnergyCost _energyCost`
- `private bool _exhaustOnNextPlay`
- `private Nullable<int> _floorAddedToDeck`
- `private bool _hasSingleTurnRetain`
- `private bool _hasSingleTurnSly`
- `private bool _isDupe`
- `private bool _isEnchantmentPreview`
- `private HashSet<CardKeyword> _keywords`
- `private int _lastStarsSpent`
- `private Player _owner`
- `private CardPoolModel _pool`
- `private bool _starCostSet`
- `private HashSet<CardTag> _tags`
- `private List<TemporaryCardCost> _temporaryStarCosts`
- `private LocString _titleLocString`
- `private CardUpgradePreviewType _upgradePreviewType`
- `private bool _wasStarCostJustUpgraded`
- `private Action AfflictionChanged`
- `private Action Drawn`
- `private Action EnchantmentChanged`
- `private Action EnergyCostChanged`
- `private Action Forged`
- `private Action KeywordsChanged`
- `private Action Played`
- `private Action ReplayCountChanged`
- `private Action StarCostChanged`
- `private Action Upgraded`

**Methods**

- `protected virtual void AddExtraArgsToDescription(LocString description)`
- `public void AddKeyword(CardKeyword keyword)`
- `internal void AddTemporaryStarCost(TemporaryCardCost cost)`
- `public void AfflictInternal(AfflictionModel affliction, decimal amount)`
- `protected virtual void AfterCloned()`
- `public virtual void AfterCreated()`
- `protected virtual void AfterDeserialized()`
- `protected virtual void AfterDowngraded()`
- `public void AfterForged()`
- `public virtual void AfterTransformedFrom()`
- `public virtual void AfterTransformedTo()`
- `public bool CanPlay()`
- `public bool CanPlay(UnplayableReason& reason, AbstractModel& preventer)`
- `public bool CanPlayTargeting(Creature target)`
- `public void ClearAfflictionInternal()`
- `public void ClearEnchantmentInternal()`
- `public virtual int CompareTo(AbstractModel other)`
- `public bool CostsEnergyOrStars(bool includeGlobalModifiers)`
- `public CardModel CreateClone()`
- `public CardModel CreateDupe()`
- `public Control CreateOverlay()`
- `protected virtual void DeepCloneFields()`
- `public void DowngradeInternal()`
- `public void EnchantInternal(EnchantmentModel enchantment, decimal amount)`
- `public void EndOfTurnCleanup()`
- `internal void EnqueueManualPlay(Creature target)`
- `public void FinalizeUpgradeInternal()`
- `public string GetDescriptionForPile(PileType pileType, Creature target)`
- `internal string GetDescriptionForPile(PileType pileType, DescriptionPreviewType previewType, Creature target)`
- `public string GetDescriptionForUpgradePreview()`
- `public int GetEnchantedReplayCount()`
- `protected virtual PileType GetResultPileTypeForCardPlay()`
- `protected virtual PileType GetResultPileTypeForOnTurnEndInHandEffect()`
- `public int GetStarCostThisCombat()`
- `public int GetStarCostWithModifiers()`
- `public void GiveSingleTurnRetain()`
- `public void GiveSingleTurnSly()`
- `public void InvokeDrawn()`
- `public void InvokeEnergyCostChanged()`
- `public bool IsValidTarget(Creature target)`
- `protected void MockSetEnergyCost(CardEnergyCost cost)`
- `public Task MoveToResultPileWithoutPlaying(PlayerChoiceContext choiceContext)`
- `protected void NeverEverCallThisOutsideOfTests_ClearOwner()`
- `public virtual Task OnEnqueuePlayVfx(Creature target)`
- `protected virtual Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)`
- `public Task OnPlayWrapper(PlayerChoiceContext choiceContext, Creature target, bool isAutoPlay, ResourceInfo resources, bool skipCardPileVisuals)`
- `protected virtual Task OnTurnEndInHand(PlayerChoiceContext choiceContext)`
- `public Task OnTurnEndInHandWrapper(PlayerChoiceContext choiceContext)`
- `protected virtual void OnUpgrade()`
- `internal Task PlayPowerCardFlyVfx()`
- `public void RemoveFromCurrentPile(bool silent)`
- `public void RemoveFromState()`
- `public void RemoveKeyword(CardKeyword keyword)`
- `public int ResolveEnergyXValue()`
- `public int ResolveStarXValue()`
- `public void SetStarCostThisCombat(int cost)`
- `public void SetStarCostThisTurn(int cost)`
- `public void SetStarCostUntilPlayed(int cost)`
- `public void SetToFreeThisCombat()`
- `public void SetToFreeThisTurn()`
- `internal Task SpendEnergy(int amount)`
- `public Task<ValueTuple<int, int>> SpendResources()`
- `internal Task SpendStars(int amount)`
- `public CardModel ToMutable()`
- `public SerializableCard ToSerializable()`
- `public bool TryManualPlay(Creature target)`
- `public void UpdateDynamicVarPreview(CardPreviewMode previewMode, Creature target, DynamicVarSet dynamicVarSet)`
- `public void UpgradeInternal()`
- `protected void UpgradeStarCostBy(int addend)`

### `abstract class CardPoolModel : AbstractModel, IPoolModel`

**Properties**

- `public IEnumerable<ModelId> AllCardIds` { get }
- `public IEnumerable<CardModel> AllCards` { get }
- `public string CardFrameMaterialPath` { get }
- `public Color DeckEntryCardColor` { get }
- `public string EnergyColorName` { get }
- `public string EnergyIconPath` { get }
- `public Color EnergyOutlineColor` { get }
- `public Material FrameMaterial` { get }
- `public string FrameMaterialPath` { get }
- `public bool IsColorless` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public string Title` { get }

**Fields**

- `private HashSet<ModelId> _allCardIds`
- `private CardModel[] _allCards`

**Methods**

- `protected virtual IEnumerable<CardModel> FilterThroughEpochs(UnlockState unlockState, IEnumerable<CardModel> cards)`
- `protected virtual CardModel[] GenerateAllCards()`
- `public IEnumerable<CardModel> GetUnlockedCards(UnlockState unlockState, CardMultiplayerConstraint multiplayerConstraint)`
- `public CardPoolModel ToMutable()`

### `abstract class CharacterModel : AbstractModel`

**Properties**

- `public Texture2D ArmPaperTexture` { get }
- `internal string ArmPaperTexturePath` { get }
- `public Texture2D ArmPointingTexture` { get }
- `internal string ArmPointingTexturePath` { get }
- `public Texture2D ArmRockTexture` { get }
- `internal string ArmRockTexturePath` { get }
- `public Texture2D ArmScissorsTexture` { get }
- `internal string ArmScissorsTexturePath` { get }
- `public IEnumerable<string> AssetPaths` { get }
- `public IEnumerable<string> AssetPathsCharacterSelect` { get }
- `public float AttackAnimDelay` { get }
- `public string AttackSfx` { get }
- `public int BaseOrbSlotCount` { get }
- `public CardPoolModel CardPool` { get }
- `public LocString CardsModifierDescription` { get }
- `public LocString CardsModifierTitle` { get }
- `public float CastAnimDelay` { get }
- `public string CastSfx` { get }
- `public string CharacterSelectBg` { get }
- `public string CharacterSelectDesc` { get }
- `public CompressedTexture2D CharacterSelectIcon` { get }
- `internal string CharacterSelectIconPath` { get }
- `public CompressedTexture2D CharacterSelectLockedIcon` { get }
- `internal string CharacterSelectLockedIconPath` { get }
- `public string CharacterSelectSfx` { get }
- `public string CharacterSelectTitle` { get }
- `public string CharacterSelectTransitionPath` { get }
- `public string CharacterTransitionSfx` { get }
- `public string DeathSfx` { get }
- `public Color DialogueColor` { get }
- `public string EnergyCounterPath` { get }
- `public Color EnergyLabelOutlineColor` { get }
- `public LocString EventDeathPreventionLine` { get }
- `internal IEnumerable<string> ExtraAssetPaths` { get }
- `public CharacterGender Gender` { get }
- `public Control Icon` { get }
- `public Texture2D IconOutlineTexture` { get }
- `internal string IconOutlineTexturePath` { get }
- `internal string IconPath` { get }
- `public Texture2D IconTexture` { get }
- `internal string IconTexturePath` { get }
- `public bool IsPlayable` { get }
- `public Color MapDrawingColor` { get }
- `public CompressedTexture2D MapMarker` { get }
- `internal string MapMarkerPath` { get }
- `public int MaxEnergy` { get }
- `public string MerchantAnimPath` { get }
- `public Color NameColor` { get }
- `public LocString PossessiveAdjective` { get }
- `public PotionPoolModel PotionPool` { get }
- `public LocString PronounObject` { get }
- `public LocString PronounPossessive` { get }
- `public LocString PronounSubject` { get }
- `public RelicPoolModel RelicPool` { get }
- `public Color RemoteTargetingLineColor` { get }
- `public Color RemoteTargetingLineOutline` { get }
- `public string RestSiteAnimPath` { get }
- `public Achievement RunWonAchievement` { get }
- `public bool ShouldAlwaysShowStarCounter` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public VfxColor SpeechBubbleColor` { get }
- `public IEnumerable<CardModel> StartingDeck` { get }
- `public int StartingGold` { get }
- `public int StartingHp` { get }
- `public IReadOnlyList<PotionModel> StartingPotions` { get }
- `public IReadOnlyList<RelicModel> StartingRelics` { get }
- `public LocString Title` { get }
- `public LocString TitleObject` { get }
- `public string TrailPath` { get }
- `internal CharacterModel UnlocksAfterRunAs` { get }
- `internal string VisualsPath` { get }

**Methods**

- `public void AddDetailsTo(LocString str)`
- `public NCreatureVisuals CreateVisuals()`
- `public virtual CreatureAnimator GenerateAnimator(MegaSprite controller)`
- `public virtual List<string> GetArchitectAttackVfx()`
- `public LocString GetUnlockText()`

### `enum DescriptionPreviewType`

Values: `None`, `Upgrade`

### `abstract class EnchantmentModel : AbstractModel`

**Properties**

- `public int Amount` { get; set }
- `internal string BetaIconPath` { get }
- `public EnchantmentModel CanonicalInstance` { get; set }
- `internal IEnumerable<DynamicVar> CanonicalVars` { get }
- `public CardModel Card` { get; set }
- `internal LocString Description` { get }
- `public int DisplayAmount` { get }
- `public LocString DynamicDescription` { get }
- `public LocString DynamicExtraCardText` { get }
- `public DynamicVarSet DynamicVars` { get }
- `internal LocString ExtraCardText` { get }
- `internal IEnumerable<IHoverTip> ExtraHoverTips` { get }
- `public bool HasCard` { get }
- `public bool HasExtraCardText` { get }
- `public HoverTip HoverTip` { get }
- `public IEnumerable<IHoverTip> HoverTips` { get }
- `public CompressedTexture2D Icon` { get }
- `public string IconPath` { get }
- `public string IntendedIconPath` { get }
- `public bool IsStackable` { get }
- `public bool PreviewOutsideOfCombat` { get }
- `public SavedProperties Props` { get; set }
- `public bool ShouldGlowGold` { get }
- `public bool ShouldGlowRed` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public bool ShouldStartAtBottomOfDrawPile` { get }
- `public bool ShowAmount` { get }
- `public EnchantmentStatus Status` { get; set }
- `public LocString Title` { get }

**Fields**

- `private int _amount`
- `private EnchantmentModel _canonicalInstance`
- `private CardModel _card`
- `private DynamicVarSet _dynamicVars`
- `private string _iconPath`
- `private EnchantmentStatus _status`
- `private Action StatusChanged`

**Methods**

- `public void ApplyInternal(CardModel card, decimal amount)`
- `public virtual bool CanEnchant(CardModel card)`
- `public virtual bool CanEnchantCardType(CardType cardType)`
- `public void ClearInternal()`
- `protected virtual void DeepCloneFields()`
- `public virtual decimal EnchantBlockAdditive(decimal originalBlock, ValueProp props)`
- `public virtual decimal EnchantBlockMultiplicative(decimal originalBlock, ValueProp props)`
- `public virtual decimal EnchantDamageAdditive(decimal originalDamage, ValueProp props)`
- `public virtual decimal EnchantDamageMultiplicative(decimal originalDamage, ValueProp props)`
- `public virtual int EnchantPlayCount(int originalPlayCount)`
- `public void ModifyCard()`
- `protected virtual void OnEnchant()`
- `public virtual Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)`
- `public virtual void RecalculateValues()`
- `public EnchantmentModel ToMutable()`
- `public SerializableEnchantment ToSerializable()`

### `abstract class EncounterModel : AbstractModel`

**Properties**

- `public IEnumerable<MonsterModel> AllPossibleMonsters` { get }
- `public string AmbientSfx` { get }
- `public string BossNodePath` { get }
- `public MegaSkeletonDataResource BossNodeSpineResource` { get }
- `public EncounterModel CanonicalInstance` { get; set }
- `public string CustomBgm` { get }
- `public LocString CustomRewardDescription` { get }
- `public IEnumerable<string> ExtraAssetPaths` { get }
- `public bool FullyCenterPlayers` { get }
- `public bool HasAmbientSfx` { get }
- `public bool HasBgm` { get }
- `internal bool HasCustomBackground` { get }
- `public bool HasScene` { get }
- `public bool HaveMonstersBeenGenerated` { get }
- `public bool IsDebugEncounter` { get }
- `public bool IsWeak` { get }
- `public IEnumerable<string> MapNodeAssetPaths` { get }
- `public int MaxGoldReward` { get }
- `public int MinGoldReward` { get }
- `public IReadOnlyList<ValueTuple<MonsterModel, string>> MonstersWithSlots` { get }
- `internal Rng Rng` { get }
- `public RoomType RoomType` { get }
- `internal string ScenePath` { get }
- `public bool ShouldGiveRewards` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public IReadOnlyList<string> Slots` { get }
- `public IEnumerable<EncounterTag> Tags` { get }
- `public LocString Title` { get }

**Fields**

- `private BackgroundAssets _backgroundAssets`
- `private EncounterModel _canonicalInstance`
- `private IReadOnlyList<ValueTuple<MonsterModel, string>> _monstersWithSlots`
- `private Rng _rng`

**Methods**

- `public NCombatBackground CreateBackground(ActModel parentAct, Rng rng)`
- `internal BackgroundAssets CreateBackgroundAssetsForCustom(Rng rng)`
- `public Control CreateScene()`
- `public void DebugRandomizeRng()`
- `protected virtual IReadOnlyList<ValueTuple<MonsterModel, string>> GenerateMonsters()`
- `public void GenerateMonstersWithSlots(IRunState runState)`
- `public IEnumerable<string> GetAssetPaths(IRunState runState)`
- `internal BackgroundAssets GetBackgroundAssets(ActModel parentAct, Rng rng)`
- `public virtual Vector2 GetCameraOffset()`
- `public virtual float GetCameraScaling()`
- `public LocString GetLossMessageFor(CharacterModel character)`
- `public string GetNextSlot(ICombatState combatState)`
- `public virtual void LoadCustomState(Dictionary<string, string> state)`
- `public virtual Dictionary<string, string> SaveCustomState()`
- `public bool SharesTagsWith(EncounterModel other)`
- `public EncounterModel ToMutable()`

### `abstract class EventModel : AbstractModel`

**Properties**

- `internal string BackgroundScenePath` { get }
- `public Color ButtonColor` { get }
- `public EncounterModel CanonicalEncounter` { get }
- `public EventModel CanonicalInstance` { get; set }
- `internal IEnumerable<DynamicVar> CanonicalVars` { get }
- `public IReadOnlyList<EventOption> CurrentOptions` { get }
- `public LocString Description` { get; set }
- `public DynamicVarSet DynamicVars` { get }
- `public IEnumerable<LocString> GameInfoOptions` { get }
- `public bool HasPhobiaModePortrait` { get }
- `public bool HasVfx` { get }
- `public LocString InitialDescription` { get }
- `internal string InitialPhobiaModePortraitPath` { get }
- `internal string InitialPortraitPath` { get }
- `public bool IsDeterministic` { get }
- `public bool IsFinished` { get; set }
- `public bool IsShared` { get }
- `internal string LayoutScenePath` { get }
- `public EventLayoutType LayoutType` { get }
- `public string LocTable` { get }
- `public Control Node` { get; set }
- `public Player Owner` { get; set }
- `public Rng Rng` { get; set }
- `public bool ShouldReceiveCombatHooks` { get }
- `public LocString Title` { get }
- `internal string VfxPath` { get }

**Fields**

- `private EventModel _canonicalInstance`
- `private bool _cleanupCalled`
- `protected CombatState _combatStateForCombatLayout`
- `private List<EventOption> _currentOptions`
- `private DynamicVarSet _dynamicVars`
- `private bool _isFinished`
- `private EncounterModel _mutableEncounter`
- `private Action EnteringEventCombat`
- `private Action<EventModel> StateChanged`

**Methods**

- `protected virtual void AfterCloned()`
- `public virtual Task AfterEventStarted()`
- `protected virtual Task BeforeEventStarted(bool isPreFinished)`
- `public Task BeginEvent(Player player, bool isPreFinished)`
- `public virtual void CalculateVars()`
- `protected void ClearCurrentOptions()`
- `public PackedScene CreateBackgroundScene()`
- `public ICombatRoomVisuals CreateCombatRoomVisuals(IEnumerable<Player> players, ActModel act)`
- `public Texture2D CreateInitialPhobiaModePortrait()`
- `public Texture2D CreateInitialPortrait()`
- `public PackedScene CreateScene()`
- `public Node2D CreateVfx()`
- `protected virtual void DeepCloneFields()`
- `public void EnsureCleanup()`
- `protected void EnterCombatWithoutExitingEvent(IReadOnlyList<Reward> extraRewards, bool shouldResumeAfterCombat)`
- `protected void EnterCombatWithoutExitingEvent(EncounterModel mutableEncounter, IReadOnlyList<Reward> extraRewards, bool shouldResumeAfterCombat)`
- `protected virtual IReadOnlyList<EventOption> GenerateInitialOptions()`
- `protected virtual IReadOnlyList<EventOption> GenerateInitialOptionsWrapper()`
- `public void GenerateInternalCombatState(IRunState runState)`
- `public virtual IEnumerable<string> GetAssetPaths(IRunState runState)`
- `public LocString GetOptionDescription(string key)`
- `public LocString GetOptionTitle(string key)`
- `protected string InitialOptionKey(string optionName)`
- `public virtual bool IsAllowed(IRunState runState)`
- `protected LocString L10NLookup(string entryName)`
- `protected virtual void OnEventFinished()`
- `public virtual void OnRoomEnter()`
- `internal string OptionKey(string pageName, string optionName)`
- `protected EventOption RelicOption(Func<Task> onChosen, string pageName)`
- `protected EventOption RelicOption(RelicModel relic, Func<Task> onChosen, string pageName)`
- `protected void ReplaceNullOptions(List<EventOption> options)`
- `public void ResetInternalCombatState()`
- `public virtual Task Resume(AbstractRoom exitedRoom)`
- `protected void SetEventFinished(LocString description)`
- `protected virtual void SetEventState(LocString description, IEnumerable<EventOption> eventOptions)`
- `protected virtual void SetInitialEventState(bool isPreFinished)`
- `public void SetNode(Control node)`
- `public EventModel ToMutable()`

### `interface IPoolModel`

**Properties**

- `public string EnergyColorName` { get }

### `interface ITemporaryPower`

**Properties**

- `public PowerModel InternallyAppliedPower` { get }
- `public AbstractModel OriginModel` { get }

**Methods**

- `public virtual void IgnoreNextInstance()`

### `sealed class ModelDb`

### `class ModelId : IComparable<ModelId>, IEquatable<ModelId>`

**Properties**

- `public string Category` { get }
- `public string Entry` { get }
- `internal Type EqualityContract` { get }

**Methods**

- `public int CompareTo(ModelId other)`
- `public virtual bool Equals(object obj)`
- `public virtual bool Equals(ModelId other)`
- `public virtual int GetHashCode()`
- `protected virtual bool PrintMembers(StringBuilder builder)`
- `public virtual string ToString()`

### `abstract class ModifierModel : AbstractModel`

**Properties**

- `internal LocString AdditionalRestSiteHealText` { get }
- `public bool ClearsPlayerDeck` { get }
- `public LocString Description` { get }
- `public IEnumerable<IHoverTip> HoverTips` { get }
- `public Texture2D Icon` { get }
- `internal string IconPath` { get }
- `public LocString NeowOptionDescription` { get }
- `public LocString NeowOptionTitle` { get }
- `internal RunState RunState` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public LocString Title` { get }

**Fields**

- `private RunState _runState`

**Methods**

- `protected virtual void AfterRunCreated(RunState runState)`
- `protected virtual void AfterRunLoaded(RunState runState)`
- `public virtual Func<Task> GenerateNeowOption(EventModel eventModel)`
- `public virtual bool IsEquivalent(ModifierModel other)`
- `public void OnRunCreated(RunState runState)`
- `public void OnRunLoaded(RunState runState)`
- `public ModifierModel ToMutable()`
- `public SerializableModifier ToSerializable()`

### `abstract class MonsterModel : AbstractModel`

**Properties**

- `public IEnumerable<string> AssetPaths` { get }
- `internal string AttackSfx` { get }
- `public bool CanChangeScale` { get }
- `public MonsterModel CanonicalInstance` { get; set }
- `internal string CastSfx` { get }
- `public ICombatState CombatState` { get }
- `public Creature Creature` { get; set }
- `public float DeathAnimLengthOverride` { get }
- `public string DeathSfx` { get }
- `public Vector2 ExtraDeathVfxPadding` { get }
- `public bool HasDeathAnimLengthOverride` { get }
- `public bool HasDeathSfx` { get }
- `public bool HasHurtSfx` { get }
- `public float HpBarSizeReduction` { get }
- `public string HurtSfx` { get }
- `public bool IntendsToAttack` { get }
- `public bool IsHealthBarVisible` { get }
- `public bool IsPerformingMove` { get; set }
- `public int MaxInitialHp` { get }
- `public int MinInitialHp` { get }
- `public MonsterMoveStateMachine MoveStateMachine` { get; set }
- `public MoveState NextMove` { get; set }
- `public Rng Rng` { get; set }
- `public RunRngSet RunRng` { get; set }
- `public bool ShouldDisappearFromDoom` { get }
- `public bool ShouldFadeAfterDeath` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public bool ShouldShowInCompendium` { get }
- `public bool SpawnedThisTurn` { get; set }
- `public string TakeDamageSfx` { get }
- `public DamageSfxType TakeDamageSfxType` { get }
- `public LocString Title` { get }
- `internal string VisualsPath` { get }

**Fields**

- `private MonsterModel _canonicalInstance`
- `private Creature _creature`
- `private bool _isPerformingMove`
- `private MonsterMoveStateMachine _moveStateMachine`
- `private Rng _rng`
- `private RunRngSet _runRng`
- `private bool _spawnedThisTurn`

**Methods**

- `public virtual Task AfterAddedToRoom()`
- `public virtual void BeforeRemovedFromRoom()`
- `internal NCreatureVisuals CreateFallbackVisuals()`
- `public NCreatureVisuals CreateVisuals()`
- `public virtual CreatureAnimator GenerateAnimator(MegaSprite controller)`
- `public virtual List<BestiaryMonsterMove> GenerateBestiaryMoveList(NCreatureVisuals creatureVisuals)`
- `protected virtual MonsterMoveStateMachine GenerateMoveStateMachine()`
- `internal IEnumerable<string> GetAllMoves(MonsterMoveStateMachine machine)`
- `protected LocString GetBestiaryMoveName(string moveId)`
- `internal List<AbstractIntent> GetIntents()`
- `public virtual void OnDieToDoom()`
- `public void OnSideSwitch()`
- `public Task PerformMove()`
- `public void ResetStateMachine()`
- `public void RollMove(IEnumerable<Creature> targets)`
- `public void SetMoveImmediate(MoveState state, bool forceTransition)`
- `public void SetUpForCombat()`
- `public virtual void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)`
- `protected virtual bool ShouldShowMoveInBestiary(string moveStateId)`
- `public MonsterModel ToMutable()`

### `abstract class OrbModel : AbstractModel`

**Properties**

- `public IEnumerable<string> AssetPaths` { get }
- `internal OrbModel CanonicalInstance` { get; set }
- `internal string ChannelSfx` { get }
- `public ICombatState CombatState` { get }
- `public Color DarkenedColor` { get }
- `internal string DebugChannelSfx` { get }
- `internal string DebugEvokeSfx` { get }
- `internal string DebugPassiveSfx` { get }
- `public LocString Description` { get }
- `public HoverTip DumbHoverTip` { get }
- `internal string EvokeSfx` { get }
- `public decimal EvokeVal` { get }
- `internal IEnumerable<IHoverTip> ExtraHoverTips` { get }
- `public bool HasBeenRemovedFromState` { get; set }
- `public bool HasSmartDescription` { get }
- `public IEnumerable<IHoverTip> HoverTips` { get }
- `public CompressedTexture2D Icon` { get }
- `internal string IconPath` { get }
- `public Player Owner` { get; set }
- `internal string PassiveSfx` { get }
- `public decimal PassiveVal` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public LocString SmartDescription` { get }
- `internal string SmartDescriptionLocKey` { get }
- `internal string SpritePath` { get }
- `public LocString Title` { get }

**Fields**

- `private OrbModel _canonicalInstance`
- `private Player _owner`
- `private Action Triggered`

**Methods**

- `protected virtual void AfterCloned()`
- `public virtual Task AfterTurnStartOrbTrigger(PlayerChoiceContext choiceContext)`
- `public virtual Task BeforeTurnEndOrbTrigger(PlayerChoiceContext choiceContext)`
- `public Node2D CreateSprite()`
- `public virtual Task<IEnumerable<Creature>> Evoke(PlayerChoiceContext playerChoiceContext)`
- `protected decimal ModifyOrbValue(decimal result)`
- `public virtual Task Passive(PlayerChoiceContext choiceContext, Creature target)`
- `public void PlayChannelSfx()`
- `protected void PlayEvokeSfx()`
- `protected void PlayPassiveSfx()`
- `public void RemoveInternal()`
- `public OrbModel ToMutable(int initialAmount)`
- `public void Trigger()`

### `abstract class PotionModel : AbstractModel`

**Properties**

- `public bool CanBeGeneratedInCombat` { get }
- `public PotionModel CanonicalInstance` { get; set }
- `internal IEnumerable<DynamicVar> CanonicalVars` { get }
- `internal LocString Description` { get }
- `public LocString DynamicDescription` { get }
- `public DynamicVarSet DynamicVars` { get }
- `public IEnumerable<IHoverTip> ExtraHoverTips` { get }
- `public bool HasBeenRemovedFromState` { get; set }
- `public HoverTip HoverTip` { get }
- `public IEnumerable<IHoverTip> HoverTips` { get }
- `public Texture2D Image` { get }
- `public string ImagePath` { get }
- `public bool IsQueued` { get; set }
- `public Texture2D Outline` { get }
- `public string OutlinePath` { get }
- `public Player Owner` { get; set }
- `internal string PackedImagePath` { get }
- `internal string PackedOutlinePath` { get }
- `public bool PassesCustomUsabilityCheck` { get }
- `public PotionPoolModel Pool` { get }
- `public PotionRarity Rarity` { get }
- `public LocString SelectionScreenPrompt` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public TargetType TargetType` { get }
- `public LocString Title` { get }
- `public PotionUsage Usage` { get }

**Fields**

- `private PotionModel _canonicalInstance`
- `private DynamicVarSet _dynamicVars`
- `private Player _owner`
- `private Action BeforeUse`

**Methods**

- `protected virtual void AfterCloned()`
- `public void AfterUsageCanceled()`
- `public bool CanThrowAtAlly()`
- `public void Discard()`
- `public void EnqueueManualUse(Creature target)`
- `protected virtual Task OnUse(PlayerChoiceContext choiceContext, Creature target)`
- `public Task OnUseWrapper(PlayerChoiceContext choiceContext, Creature target)`
- `public void RemoveBeforeUse()`
- `public PotionModel ToMutable()`
- `public SerializablePotion ToSerializable(int slotIndex)`

### `abstract class PotionPoolModel : AbstractModel, IPoolModel`

**Properties**

- `public IEnumerable<ModelId> AllPotionIds` { get }
- `public IEnumerable<PotionModel> AllPotions` { get }
- `public string EnergyColorName` { get }
- `public Color LabOutlineColor` { get }
- `public bool ShouldReceiveCombatHooks` { get }

**Fields**

- `private HashSet<ModelId> _allPotionIds`
- `private IEnumerable<PotionModel> _allPotions`

**Methods**

- `protected virtual IEnumerable<PotionModel> GenerateAllPotions()`
- `public virtual IEnumerable<PotionModel> GetUnlockedPotions(UnlockState unlockState)`

### `abstract class PowerModel : AbstractModel`

**Properties**

- `public bool AllowNegative` { get }
- `public int Amount` { get; set }
- `public Color AmountLabelColor` { get }
- `public int AmountOnTurnStart` { get; set }
- `public Creature Applier` { get; set }
- `internal string BigBetaIconPath` { get }
- `public Texture2D BigIcon` { get }
- `internal string BigIconPath` { get }
- `internal PowerModel CanonicalInstance` { get; set }
- `internal IEnumerable<DynamicVar> CanonicalVars` { get }
- `public ICombatState CombatState` { get }
- `public LocString Description` { get }
- `public int DisplayAmount` { get }
- `public HoverTip DumbHoverTip` { get }
- `public DynamicVarSet DynamicVars` { get }
- `internal IEnumerable<IHoverTip> ExtraHoverTips` { get }
- `public bool HasRemoteDescription` { get }
- `public bool HasSmartDescription` { get }
- `public IEnumerable<IHoverTip> HoverTips` { get }
- `public Texture2D Icon` { get }
- `public string IconPath` { get }
- `public PowerInstanceType InstanceType` { get }
- `public bool IsVisible` { get }
- `internal bool IsVisibleInternal` { get }
- `public Creature Owner` { get; set }
- `public bool OwnerIsSecondaryEnemy` { get }
- `public string PackedIconPath` { get }
- `public LocString RemoteDescription` { get }
- `internal string RemoteDescriptionLocKey` { get }
- `public string ResolvedBigIconPath` { get }
- `internal LocString SelectionScreenPrompt` { get }
- `public bool ShouldPlayVfx` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public bool ShouldScaleInMultiplayer` { get }
- `public bool SkipNextDurationTick` { get; set }
- `public LocString SmartDescription` { get }
- `internal string SmartDescriptionLocKey` { get }
- `public PowerStackType StackType` { get }
- `public Creature Target` { get; set }
- `public LocString Title` { get }
- `public PowerType Type` { get }
- `public PowerType TypeForCurrentAmount` { get }

**Fields**

- `private int _amount`
- `private int _amountOnTurnStart`
- `private Creature _applier`
- `private PowerModel _canonicalInstance`
- `private DynamicVarSet _dynamicVars`
- `private object _internalData`
- `private Creature _owner`
- `private string _resolvedBigIconPath`
- `private bool _skipNextDurationTick`
- `private Creature _target`
- `private Action DisplayAmountChanged`
- `private Action<PowerModel> Flashed`
- `private Action PulsingStarted`
- `private Action PulsingStopped`
- `private Action Removed`

**Methods**

- `internal void AddDumbVariablesToDescription(LocString description)`
- `public virtual Task AfterApplied(Creature applier, CardModel cardSource)`
- `protected virtual void AfterCloned()`
- `public virtual Task AfterRemoved(Creature oldOwner)`
- `public void ApplyInternal(Creature owner, decimal amount, bool silent)`
- `public virtual Task BeforeApplied(Creature target, decimal amount, Creature applier, CardModel cardSource)`
- `protected virtual void DeepCloneFields()`
- `protected void Flash()`
- `protected T GetInternalData()`
- `public virtual decimal GetScaledAmountForMultiplayer(ICombatState combatState, Creature applier, decimal amount, Creature target, CardModel cardSource)`
- `public PowerType GetTypeForAmount(decimal customAmount)`
- `protected virtual object InitInternalData()`
- `protected void InvokeDisplayAmountChanged()`
- `public void RemoveInternal()`
- `public void SetAmount(int amount, bool silent)`
- `public virtual bool ShouldOwnerDeathTriggerFatal()`
- `public virtual bool ShouldPowerBeRemovedAfterOwnerDeath()`
- `public bool ShouldRemoveDueToAmount()`
- `public void StartPulsing()`
- `public void StopPulsing()`
- `public PowerModel ToMutable(int initialAmount)`

### `abstract class RelicModel : AbstractModel`

**Properties**

- `internal LocString AdditionalRestSiteHealText` { get }
- `public bool AddsPet` { get }
- `internal string BigBetaIconPath` { get }
- `public Texture2D BigIcon` { get }
- `internal string BigIconPath` { get }
- `public RelicModel CanonicalInstance` { get; set }
- `internal IEnumerable<DynamicVar> CanonicalVars` { get }
- `internal LocString Description` { get }
- `public int DisplayAmount` { get }
- `public LocString DynamicDescription` { get }
- `public LocString DynamicEventDescription` { get }
- `public DynamicVarSet DynamicVars` { get }
- `internal LocString EventDescription` { get }
- `internal IEnumerable<IHoverTip> ExtraHoverTips` { get }
- `public string FlashSfx` { get }
- `public LocString Flavor` { get }
- `public int FloorAddedToDeck` { get; set }
- `public bool HasBeenRemovedFromState` { get; set }
- `public bool HasUponPickupEffect` { get }
- `public HoverTip HoverTip` { get }
- `public IEnumerable<IHoverTip> HoverTips` { get }
- `public IEnumerable<IHoverTip> HoverTipsExcludingRelic` { get }
- `public Texture2D Icon` { get }
- `internal string IconBaseName` { get }
- `public Texture2D IconOutline` { get }
- `public string IconPath` { get }
- `public bool IsAllowedInShops` { get }
- `public bool IsMelted` { get; set }
- `public bool IsStackable` { get }
- `public bool IsTradable` { get }
- `public bool IsUsedUp` { get }
- `public bool IsWax` { get; set }
- `public int MerchantCost` { get }
- `public Player Owner` { get; set }
- `internal string PackedIconOutlinePath` { get }
- `public string PackedIconPath` { get }
- `public RelicPoolModel Pool` { get }
- `public RelicRarity Rarity` { get }
- `internal string ResolvedBigIconPath` { get }
- `internal LocString SelectionScreenPrompt` { get }
- `public bool ShouldFlashOnPlayer` { get }
- `public bool ShouldReceiveCombatHooks` { get }
- `public bool ShowCounter` { get }
- `public bool SpawnsPets` { get }
- `public int StackCount` { get; set }
- `public RelicStatus Status` { get; set }
- `public LocString Title` { get }

**Fields**

- `private RelicModel _canonicalInstance`
- `private DynamicVarSet _dynamicVars`
- `private int _floorAddedToDeck`
- `private bool _isMelted`
- `private bool _isWax`
- `private Player _owner`
- `private string _resolvedBigIconPath`
- `private RelicStatus _status`
- `private Action DisplayAmountChanged`
- `private Action<RelicModel, IEnumerable<Creature>> Flashed`
- `private Action StatusChanged`

**Methods**

- `protected virtual void AfterCloned()`
- `public virtual Task AfterObtained()`
- `public virtual Task AfterRemoved()`
- `protected virtual void DeepCloneFields()`
- `public void Flash()`
- `public void Flash(IEnumerable<Creature> targets)`
- `public void IncrementStackCount()`
- `protected void InvokeDisplayAmountChanged()`
- `public virtual bool IsAllowed(IRunState runState)`
- `public virtual bool IsAllowedAtNeow(Player player)`
- `protected void RelicIconChanged()`
- `public void RemoveInternal()`
- `public RelicModel ToMutable()`
- `public SerializableRelic ToSerializable()`
- `public void UpdateTexture(TextureRect texture)`

### `abstract class RelicPoolModel : AbstractModel, IPoolModel`

**Properties**

- `public HashSet<ModelId> AllRelicIds` { get }
- `public IEnumerable<RelicModel> AllRelics` { get }
- `public string EnergyColorName` { get }
- `public Color LabOutlineColor` { get }
- `public bool ShouldReceiveCombatHooks` { get }

**Fields**

- `private HashSet<ModelId> _allRelicIds`
- `private IEnumerable<RelicModel> _relics`

**Methods**

- `protected virtual IEnumerable<RelicModel> GenerateAllRelics()`
- `public virtual IEnumerable<RelicModel> GetUnlockedRelics(UnlockState unlockState)`

### `abstract class SingletonModel : AbstractModel`

## MegaCrit.Sts2.Core.MonsterMoves.Intents

### `sealed class <>c__DisplayClass4_0`

**Fields**

- `public int damage`

### `sealed class <>c__DisplayClass6_0`

**Fields**

- `public int damage`

### `sealed class <>c__DisplayClass7_0`

**Fields**

- `public int damage`

### `abstract class AbstractIntent`

**Properties**

- `public IEnumerable<string> AssetPaths` { get }
- `public bool HasIntentTip` { get }
- `internal LocString IntentLabelFormat` { get }
- `internal string IntentPrefix` { get }
- `internal LocString IntentTitle` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

**Fields**

- `protected string _cachedAnimationName`

**Methods**

- `public virtual string GetAnimation(IEnumerable<Creature> targets, Creature owner)`
- `public HoverTip GetHoverTip(IEnumerable<Creature> targets, Creature owner)`
- `protected virtual LocString GetIntentDescription(IEnumerable<Creature> targets, Creature owner)`
- `public virtual LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)`
- `public virtual Texture2D GetTexture(IEnumerable<Creature> targets, Creature owner)`

### `abstract class AttackIntent : AbstractIntent`

**Properties**

- `public IEnumerable<string> AssetPaths` { get }
- `public Func<decimal> DamageCalc` { get; set }
- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `public int Repeats` { get }
- `internal string SpritePath` { get }

**Methods**

- `public virtual string GetAnimation(IEnumerable<Creature> targets, Creature owner)`
- `protected virtual LocString GetIntentDescription(IEnumerable<Creature> targets, Creature owner)`
- `public int GetSingleDamage(IEnumerable<Creature> targets, Creature owner)`
- `public virtual Texture2D GetTexture(IEnumerable<Creature> targets, Creature owner)`
- `public virtual int GetTotalDamage(IEnumerable<Creature> targets, Creature owner)`

### `class BuffIntent : AbstractIntent`

**Properties**

- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

### `class CardDebuffIntent : AbstractIntent`

**Properties**

- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

### `class DeathBlowIntent : SingleAttackIntent`

**Properties**

- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

**Methods**

- `public virtual string GetAnimation(IEnumerable<Creature> targets, Creature owner)`
- `public virtual Texture2D GetTexture(IEnumerable<Creature> targets, Creature owner)`

### `class DebuffIntent : AbstractIntent`

**Properties**

- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

**Fields**

- `private bool _strong`

### `class DefendIntent : AbstractIntent`

**Properties**

- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

### `class EscapeIntent : AbstractIntent`

**Properties**

- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

### `class HealIntent : AbstractIntent`

**Properties**

- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

### `class HiddenIntent : AbstractIntent`

**Properties**

- `public bool HasIntentTip` { get }
- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

### `enum IntentType`

Values: `Attack`, `Buff`, `Debuff`, `DebuffStrong`, `Defend`, `Escape`, `Heal`, `Hidden`, `Summon`, `Sleep`, `Stun`, `StatusCard`, `CardDebuff`, `DeathBlow`, `Unknown`

### `class MultiAttackIntent : AttackIntent`

**Properties**

- `internal LocString IntentLabelFormat` { get }
- `public int Repeats` { get }

**Fields**

- `private int _repeat`
- `private Func<int> _repeatCalc`

**Methods**

- `public virtual LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)`
- `public virtual int GetTotalDamage(IEnumerable<Creature> targets, Creature owner)`

### `class SingleAttackIntent : AttackIntent`

**Properties**

- `internal LocString IntentLabelFormat` { get }
- `public int Repeats` { get }

**Methods**

- `public virtual LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)`
- `public virtual int GetTotalDamage(IEnumerable<Creature> targets, Creature owner)`

### `class SleepIntent : AbstractIntent`

**Properties**

- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

### `class StatusIntent : AbstractIntent`

**Properties**

- `public int CardCount` { get }
- `internal LocString IntentLabelFormat` { get }
- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

**Methods**

- `protected virtual LocString GetIntentDescription(IEnumerable<Creature> targets, Creature owner)`
- `public virtual LocString GetIntentLabel(IEnumerable<Creature> _, Creature __)`

### `class StunIntent : AbstractIntent`

**Properties**

- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

### `class SummonIntent : AbstractIntent`

**Properties**

- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

### `class UnknownIntent : AbstractIntent`

**Properties**

- `internal string IntentPrefix` { get }
- `public IntentType IntentType` { get }
- `internal string SpritePath` { get }

## MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine

### `sealed class <>c`

### `sealed class <>c__DisplayClass14_0`

**Fields**

- `public float weight`

### `sealed class <>c__DisplayClass15_0`

**Fields**

- `public float weight`

### `sealed class <>c__DisplayClass17_0`

**Fields**

- `public float weight`

### `sealed class <>c__DisplayClass21_0`

**Fields**

- `public Creature owner`

### `sealed class <>c__DisplayClass22_0`

**Fields**

- `public StateWeight stateWeight`

### `sealed class <>O`

### `sealed class <PerformMove>d__25 : ValueType, IAsyncStateMachine`

**Fields**

- `public IEnumerable<Creature> targets`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class ConditionalBranch : ValueType`

**Fields**

- `private Func<bool> _conditionalLambda`
- `public string id`

**Methods**

- `public float Evaluate()`

### `class ConditionalBranchState : MonsterState`

**Properties**

- `public string BranchId` { get }
- `public string Id` { get }
- `public bool ShouldAppearInLogs` { get }
- `internal List<ConditionalBranch> States` { get }

**Methods**

- `public void AddState(MonsterState move, Func<bool> condition)`
- `public virtual string GetNextState(Creature _, Rng __)`
- `public virtual void RegisterStates(Dictionary<string, MonsterState> monsterStates)`

### `class MonsterMoveStateMachine`

**Properties**

- `public List<MonsterState> StateLog` { get }
- `public Dictionary<string, MonsterState> States` { get }

**Fields**

- `private MonsterState _currentState`
- `private MonsterState _initialState`
- `private bool _performedFirstMove`

**Methods**

- `internal void FindNextMoveState(IEnumerable<Creature> targets, Creature owner, Rng rng, bool logMove)`
- `public void ForceCurrentState(MonsterState state)`
- `public void OnMovePerformed(MoveState _)`
- `public MoveState RollMove(IEnumerable<Creature> targets, Creature owner, Rng rng)`
- `internal void SetCurrentState(MonsterState state)`

### `abstract class MonsterState`

**Properties**

- `public bool CanTransitionAway` { get }
- `public string Id` { get }
- `public bool IsMove` { get }
- `public bool ShouldAppearInLogs` { get }

**Methods**

- `public virtual string GetNextState(Creature owner, Rng rng)`
- `public virtual void OnEnterState()`
- `public virtual void OnExitState()`
- `public virtual void RegisterStates(Dictionary<string, MonsterState> monsterStates)`

### `class MoveState : MonsterState`

**Properties**

- `public bool CanTransitionAway` { get }
- `public MonsterState FollowUpState` { get; set }
- `public string FollowUpStateId` { get; set }
- `public string Id` { get }
- `public IReadOnlyList<AbstractIntent> Intents` { get; set }
- `public bool IsMove` { get }
- `public bool MustPerformOnceBeforeTransitioning` { get; set }
- `public string StateId` { get }

**Fields**

- `private Func<IReadOnlyList<Creature>, Task> _onPerform`
- `private bool _performedAtLeastOnce`

**Methods**

- `public virtual string GetNextState(Creature owner, Rng rng)`
- `public virtual void OnExitState()`
- `public Task PerformMove(IEnumerable<Creature> targets)`
- `public virtual void RegisterStates(Dictionary<string, MonsterState> monsterStates)`

### `class RandomBranchState : MonsterState`

**Properties**

- `public string Id` { get }
- `public bool ShouldAppearInLogs` { get }
- `public string StateId` { get }
- `public List<StateWeight> States` { get; set }

**Methods**

- `public void AddBranch(MonsterState state, int cooldown, MoveRepeatType repeatType, Func<float> weight)`
- `public void AddBranch(MonsterState state, int cooldown, int maxRepeats, Func<float> weight)`
- `public void AddBranch(MonsterState state, int maxRepeats, Func<float> weight)`
- `public void AddBranch(MonsterState state, int cooldown, MoveRepeatType repeatType, float weight)`
- `public void AddBranch(MonsterState state, MoveRepeatType repeatType, float weight)`
- `public void AddBranch(MonsterState state, MoveRepeatType repeatType, Func<float> weight)`
- `public void AddBranch(MonsterState state, int maxRepeats, float weight)`
- `public void AddBranch(MonsterState state, int cooldown, MoveRepeatType repeatType)`
- `public void AddBranch(MonsterState state, int maxRepeats)`
- `public void AddBranch(MonsterState state, MoveRepeatType repeatType)`
- `public virtual string GetNextState(Creature owner, Rng rng)`
- `public virtual void RegisterStates(Dictionary<string, MonsterState> monsterStates)`

### `sealed class StateWeight : ValueType`

**Fields**

- `public int cooldown`
- `public int maxTimes`
- `public MoveRepeatType repeatType`
- `public string stateId`
- `public Func<float> weightLambda`

**Methods**

- `public float GetWeight()`

## MegaCrit.Sts2.Core.Multiplayer.Game

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c__DisplayClass16_0`

**Fields**

- `public Player player`

### `sealed class <>c__DisplayClass16_0`1`

**Fields**

- `public MessageHandlerDelegate<T> handler`

### `sealed class <>c__DisplayClass17_0`

**Fields**

- `public Player owner`
- `public Rng rng`

### `sealed class <>c__DisplayClass26_0`

**Fields**

- `public List<RelicPickingResult> results`

### `sealed class <>c__DisplayClass26_1`

**Fields**

- `public <>c__DisplayClass26_0 CS$<>8__locals1`
- `public RelicPickingFightMove[] possibleMoves`

### `sealed class <>c__DisplayClass26_2`

**Fields**

- `public Player p`

### `sealed class <>c__DisplayClass27_0`

**Fields**

- `public NetChecksumData remoteChecksumData`

### `sealed class <>c__DisplayClass28_0`

**Fields**

- `public NetChecksumData remoteChecksumData`

### `sealed class <>c__DisplayClass32_0`

**Fields**

- `public int checksumIndex`
- `public TrackedChecksum localChecksum`
- `public string localState`
- `public StateDivergenceMessage message`
- `public UInt64 remoteId`
- `public string remoteState`
- `public string role`

### `sealed class <>c__DisplayClass34_0`

**Fields**

- `public NetChecksumData localData`

### `sealed class <AttemptJoin>d__15 : ValueType, IAsyncStateMachine`

**Fields**

- `public NetClientGameService gameService`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <AttemptLoadJoin>d__16 : ValueType, IAsyncStateMachine`

**Fields**

- `public NetClientGameService gameService`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <AttemptRejoin>d__17 : ValueType, IAsyncStateMachine`

**Fields**

- `public NetClientGameService gameService`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <Begin>d__13 : ValueType, IAsyncStateMachine`

**Fields**

- `public IClientConnectionInitializer initializer`
- `public SceneTree sceneTree`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <ChooseOption>d__28 : ValueType, IAsyncStateMachine`

**Fields**

- `public int optionIndex`
- `public Player player`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <DoLocalCardRemoval>d__21 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <DoLocalCrystalSphereRewards>d__15 : ValueType, IAsyncStateMachine`

**Fields**

- `public Player owner`
- `public List<CrystalSphereItem> revealed`
- `public Rng rng`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <DoMerchantCardRemoval>d__10 : ValueType, IAsyncStateMachine`

**Fields**

- `public bool cancelable`
- `public int goldCost`
- `public Player player`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <DoTreasureRoomRewards>d__13 : ValueType, IAsyncStateMachine`

**Fields**

- `public Player player`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <DoUnsyncedCardRemoval>d__26 : ValueType, IAsyncStateMachine`

**Fields**

- `public Player player`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <NetServiceUpdateLoop>d__14 : ValueType, IAsyncStateMachine`

**Fields**

- `public SceneTree sceneTree`
- `public CancellationTokenSource token`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <OfferCrystalSphereRewards>d__17 : ValueType, IAsyncStateMachine`

**Fields**

- `public Player owner`
- `public List<CrystalSphereItem> revealed`
- `public Rng rng`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <QueueHoverMessage>d__36 : ValueType, IAsyncStateMachine`

**Fields**

- `public int delayMsec`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SelectLocalReward>d__16 : ValueType, IAsyncStateMachine`

**Fields**

- `public Reward reward`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SelectRewardForPlayer>d__20 : ValueType, IAsyncStateMachine`

**Fields**

- `public Player player`
- `public int rewardIndex`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SelectRewardForPlayer>d__21 : ValueType, IAsyncStateMachine`

**Fields**

- `public Reward reward`
- `public RewardsSetState setState`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SendHoverMessageAfterSmallDelay>d__37 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <TryHandleSpoilsMap>d__14 : ValueType, IAsyncStateMachine`

**Fields**

- `public Player player`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `class ActChangeSynchronizer`

**Fields**

- `private Logger _logger`
- `private List<bool> _readyPlayers`
- `private RunState _runState`

**Methods**

- `public bool IsWaitingForOtherPlayers()`
- `internal void MoveToNextAct()`
- `public void OnPlayerReady(Player player)`
- `public void SetLocalPlayerReady()`

### `sealed class AnonymizedMessageHandlerDelegate : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(INetMessage message, UInt64 senderId, AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke(INetMessage message, UInt64 senderId)`

### `sealed class BlockedMessage : ValueType`

**Fields**

- `public RunLocation location`
- `public INetMessage message`
- `public Type messageType`
- `public UInt64 senderId`

### `sealed class BufferedMessage : ValueType, IEquatable<BufferedMessage>`

**Properties**

- `public int SetId` { get }

**Fields**

- `public RewardSelectedMessage selectedMessage`
- `public UInt64 senderId`
- `public RewardSetSkippedMessage skippedMessage`

**Methods**

- `public virtual bool Equals(object obj)`
- `public bool Equals(BufferedMessage other)`
- `public virtual int GetHashCode()`
- `internal bool PrintMembers(StringBuilder builder)`
- `public virtual string ToString()`

### `sealed class BufferedMessage : ValueType`

**Fields**

- `public CardRemovedMessage cardRemovedMessage`
- `public Nullable<GoldLostMessage> goldLostMessage`
- `public Nullable<RewardObtainedMessage> rewardMessage`
- `public UInt64 senderId`

### `class ChecksumTracker : IDisposable`

**Properties**

- `public bool IsEnabled` { get; set }
- `public UInt32 NextId` { get; set }

**Fields**

- `private List<TrackedChecksum> _checksums`
- `private Logger _logger`
- `private INetGameService _netService`
- `private PacketWriter _packetWriter`
- `private List<QueuedRemoteChecksum> _queuedRemoteChecksums`
- `private List<ReplayChecksumData> _replayChecksums`
- `private IRunState _runState`
- `private Action<NetChecksumData, string, NetFullCombatState> ChecksumGenerated`
- `private Action<NetFullCombatState> StateDiverged`

**Methods**

- `internal void CheckAgainstReplayChecksum(NetChecksumData localData, string context)`
- `internal void CompareChecksums(TrackedChecksum localChecksum, NetChecksumData remoteChecksum, UInt64 remoteId)`
- `public void Dispose()`
- `public NetChecksumData GenerateChecksum(string context, GameAction action)`
- `public UInt32 GenerateChecksum(NetFullCombatState state)`
- `public void LoadReplayChecksums(List<ReplayChecksumData> replayChecksums, UInt32 nextId)`
- `internal void LogStateDivergence(TrackedChecksum localChecksum, StateDivergenceMessage message, UInt64 remoteId, int checksumIndex)`
- `internal NetChecksumData ObtainAndTrackChecksum(string context, GameAction action)`
- `internal void OnReceivedChecksumDataMessage(ChecksumDataMessage message, UInt64 senderId)`
- `internal void OnReceivedStateDivergenceMessage(StateDivergenceMessage message, UInt64 senderId)`
- `internal void ReportDivergenceToSentry(TrackedChecksum localChecksum, StateDivergenceMessage message, UInt64 remoteId, int checksumIndex)`

### `class EventSynchronizer : IDisposable`

**Properties**

- `public IReadOnlyList<EventModel> Events` { get }
- `public bool IsShared` { get }
- `internal Player LocalPlayer` { get }

**Fields**

- `private EventModel _canonicalEvent`
- `private List<EventModel> _events`
- `private UInt64 _localPlayerId`
- `private Logger _logger`
- `private RunLocationTargetedMessageBuffer _messageBuffer`
- `private Rng _multiplayerOptionSelectionRng`
- `private INetGameService _netService`
- `private UInt32 _pageIndex`
- `private IPlayerCollection _playerCollection`
- `private List<Nullable<UInt32>> _playerVotes`
- `private Action<Player> PlayerVoteChanged`

**Methods**

- `public void BeginEvent(EventModel canonicalEvent, bool isPrefinished, Action<EventModel> debugOnStart)`
- `public void ChooseLocalOption(int index)`
- `internal void ChooseOptionForEvent(Player player, int optionIndex)`
- `internal void ChooseOptionForSharedEvent(UInt32 optionIndex)`
- `internal void ChooseSharedEventOption()`
- `internal void ClearPlayerVotes()`
- `public void Dispose()`
- `public EventModel GetEventForPlayer(Player player)`
- `public EventModel GetLocalEvent()`
- `public Nullable<UInt32> GetPlayerVote(Player player)`
- `internal void HandleEventOptionChosenMessage(OptionIndexChosenMessage message, UInt64 senderId)`
- `internal void HandleSharedEventOptionChosenMessage(SharedEventOptionChosenMessage message, UInt64 senderId)`
- `internal void HandleVotedForSharedEventOptionMessage(VotedForSharedEventOptionMessage message, UInt64 senderId)`
- `internal void PlayerVotedForSharedOptionIndex(Player player, UInt32 optionIndex, UInt32 pageIndex)`
- `public void ResumeEvents(AbstractRoom exitedRoom)`
- `internal void SaveEventOptionToHistory(Player player, EventOption option)`

### `class FlavorSynchronizer : IDisposable`

**Properties**

- `internal Player LocalPlayer` { get }

**Fields**

- `private Dictionary<Player, NSpeechBubbleVfx> _endTurnPingDialogues`
- `private INetGameService _gameService`
- `private UInt64 _localPlayerId`
- `private UInt64 _nextAllowedPingTime`
- `private IPlayerCollection _playerCollection`
- `private Action<UInt64> OnEndTurnPingReceived`

**Methods**

- `internal void CreateEndTurnPingDialogueIfNecessary(Player player)`
- `internal void CreateMapPing(MapCoord coord, Player player)`
- `public void Dispose()`
- `internal void HandleEndTurnPingMessage(EndTurnPingMessage message, UInt64 senderId)`
- `internal void HandleMapPingMessage(MapPingMessage message, UInt64 senderId)`
- `public void SendEndTurnPing()`
- `public void SendMapPing(MapCoord coord)`

### `interface INetClientGameService : INetGameService`

**Properties**

- `public NetClient NetClient` { get }

### `interface INetGameService`

**Properties**

- `public bool IsConnected` { get }
- `public bool IsGameLoading` { get }
- `public UInt64 NetId` { get }
- `public PlatformType Platform` { get }
- `public NetGameType Type` { get }

**Methods**

- `public virtual void Disconnect(NetError reason, bool now)`
- `public virtual string GetRawLobbyIdentifier()`
- `public virtual ConnectionStats GetStatsForPeer(UInt64 peerId)`
- `public virtual void RegisterMessageHandler(MessageHandlerDelegate<T> messageHandlerDelegate)`
- `public virtual void SendMessage(T message, UInt64 playerId)`
- `public virtual void SendMessage(T message)`
- `public virtual void SetBufferMessages(bool bufferMessages)`
- `public virtual void SetGameLoading(bool isLoading)`
- `public virtual void UnregisterMessageHandler(MessageHandlerDelegate<T> messageHandlerDelegate)`
- `public virtual void Update()`

### `interface INetHostGameService : INetGameService`

**Properties**

- `public IReadOnlyList<NetClientData> ConnectedPeers` { get }
- `public NetHost NetHost` { get }

**Methods**

- `public virtual void DisconnectClient(UInt64 peerId, NetError reason, bool now)`
- `public virtual void SetPeerReadyForBroadcasting(UInt64 peerId)`

### `class JoinFlow`

**Properties**

- `public CancellationTokenSource CancelToken` { get; set }
- `public NetClientGameService NetService` { get; set }

**Fields**

- `private TaskCompletionSource<InitialGameInfoMessage> _connectCompletion`
- `private TaskCompletionSource<ClientLobbyJoinResponseMessage> _joinCompletion`
- `private TaskCompletionSource<ClientLoadJoinResponseMessage> _loadJoinCompletion`
- `private Logger _logger`
- `private TaskCompletionSource<ClientRejoinResponseMessage> _rejoinCompletion`

**Methods**

- `internal Task<ClientLobbyJoinResponseMessage> AttemptJoin(NetClientGameService gameService)`
- `internal Task<ClientLoadJoinResponseMessage> AttemptLoadJoin(NetClientGameService gameService)`
- `internal Task<ClientRejoinResponseMessage> AttemptRejoin(NetClientGameService gameService)`
- `public Task<JoinResult> Begin(IClientConnectionInitializer initializer, SceneTree sceneTree)`
- `internal void Cancel()`
- `internal void HandleInitialGameInfoMessage(InitialGameInfoMessage message, UInt64 _)`
- `internal void HandleJoinResponseMessage(ClientLobbyJoinResponseMessage message, UInt64 senderId)`
- `internal void HandleLoadJoinResponseMessage(ClientLoadJoinResponseMessage message, UInt64 senderId)`
- `internal void HandleRejoinResponseMessage(ClientRejoinResponseMessage message, UInt64 senderId)`
- `internal Task NetServiceUpdateLoop(CancellationTokenSource token, SceneTree sceneTree)`
- `internal void OnDisconnected(NetErrorInfo info)`

### `sealed class JoinResult : ValueType`

**Fields**

- `public GameMode gameMode`
- `public Nullable<ClientLobbyJoinResponseMessage> joinResponse`
- `public Nullable<ClientLoadJoinResponseMessage> loadJoinResponse`
- `public Nullable<ClientRejoinResponseMessage> rejoinResponse`
- `public Nullable<RunSessionState> sessionState`

### `class MapSelectionSynchronizer`

**Properties**

- `public int MapGenerationCount` { get; set }

**Fields**

- `private MapLocation _acceptingVotesFromSource`
- `private ActionQueueSynchronizer _actionQueueSynchronizer`
- `private Logger _logger`
- `private Rng _multiplayerMapPointSelection`
- `private INetGameService _netService`
- `private RunState _runState`
- `private List<Nullable<MapVote>> _votes`
- `private Action<Player> PlayerVoteCancelled`
- `private Action<Player, Nullable<MapVote>, Nullable<MapVote>> PlayerVoteChanged`
- `private Action PlayerVotesCleared`

**Methods**

- `public void BeforeMapGenerated()`
- `public Nullable<MapVote> GetVote(Player player)`
- `internal void MoveToMapCoord()`
- `public void OnLocationChanged(MapLocation location)`
- `public void PlayerVotedForMapCoord(Player player, MapLocation source, Nullable<MapVote> destination)`

### `sealed class MapVote : ValueType, IPacketSerializable`

**Fields**

- `public MapCoord coord`
- `public int mapGenerationCount`

**Methods**

- `public void Deserialize(PacketReader reader)`
- `public void Serialize(PacketWriter writer)`
- `public virtual string ToString()`

### `sealed class MessageHandler : ValueType`

**Fields**

- `public AnonymizedMessageHandlerDelegate anonymizedHandler`
- `public object originalHandler`

### `sealed class MessageHandlerDelegate`1 : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(T message, UInt64 senderId, AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke(T message, UInt64 senderId)`

### `enum NetGameType`

Values: `None`, `Singleplayer`, `Host`, `Client`, `Replay`

### `sealed class NetGameTypeExtensions`

### `class NetLoadingHandle : IDisposable`

**Fields**

- `private INetGameService _netService`

**Methods**

- `public void Dispose()`

### `class OneOffSynchronizer : IDisposable`

**Properties**

- `internal Player LocalPlayer` { get }

**Fields**

- `private INetGameService _gameService`
- `private UInt64 _localPlayerId`
- `private RunLocationTargetedMessageBuffer _messageBuffer`
- `private IPlayerCollection _playerCollection`

**Methods**

- `public void Dispose()`
- `public Task DoLocalCrystalSphereRewards(Player owner, Rng rng, List<CrystalSphereItem> revealed)`
- `public Task<bool> DoLocalMerchantCardRemoval(int goldCost, bool cancelable)`
- `public Task<int> DoLocalTreasureRoomRewards()`
- `internal Task<bool> DoMerchantCardRemoval(Player player, int goldCost, bool cancelable)`
- `internal Task<int> DoTreasureRoomRewards(Player player)`
- `internal void HandleCrystalSphereRewardsMessage(CrystalSphereRewardsMessage message, UInt64 senderId)`
- `internal void HandleMerchantCardRemoval(MerchantCardRemovalMessage message, UInt64 senderId)`
- `internal void HandleTreasureChestOpenedMessage(TreasureChestOpenedMessage message, UInt64 senderId)`
- `internal Task OfferCrystalSphereRewards(Player owner, List<CrystalSphereItem> revealed, Rng rng)`
- `internal Task<int> TryHandleSpoilsMap(Player player)`

### `class PlayerRestSite`

**Fields**

- `public Nullable<UInt32> hoveredOptionIndex`
- `public Nullable<UInt32> lastChosenOptionIndex`
- `public List<RestSiteOption> options`

### `class PlayerRewardState`

**Fields**

- `public List<BufferedMessage> bufferedMessages`
- `public Dictionary<int, RewardSetCompleteState> completedRewards`
- `public int nextId`
- `public List<RewardsSetState> rewardsStack`

### `class PlayerVote`

**Fields**

- `public Nullable<int> index`
- `public bool voteReceived`

### `sealed class QueuedRemoteChecksum : ValueType`

**Fields**

- `public NetChecksumData data`
- `public UInt64 senderId`

### `class ReactionSynchronizer : IDisposable`

**Properties**

- `public INetGameService NetService` { get }

**Fields**

- `private NReactionContainer _container`

**Methods**

- `public void Dispose()`
- `internal void HandleReactionMessage(ReactionMessage message, UInt64 senderId)`
- `public void SendLocalReaction(ReactionType type, Vector2 mouseScreenPos)`

### `class RestSiteSynchronizer : IDisposable`

**Properties**

- `internal Player LocalPlayer` { get }

**Fields**

- `private Nullable<RestSiteOptionHoveredMessage> _hoveredMessage`
- `private Task _hoverMessageTask`
- `private UInt64 _lastHoverMessageMsec`
- `private UInt64 _localPlayerId`
- `private Logger _logger`
- `private RunLocationTargetedMessageBuffer _messageBuffer`
- `private INetGameService _netService`
- `private IPlayerCollection _playerCollection`
- `private List<PlayerRestSite> _restSites`
- `private Action<RestSiteOption, bool, UInt64> AfterPlayerOptionChosen`
- `private Action<RestSiteOption, UInt64> BeforePlayerOptionChosen`
- `private Action<UInt64> PlayerHoverChanged`

**Methods**

- `public void BeginRestSite()`
- `public Task<bool> ChooseLocalOption(int index)`
- `internal Task<bool> ChooseOption(Player player, int optionIndex)`
- `public void Dispose()`
- `public Nullable<int> GetChosenOptionIndex(UInt64 playerId)`
- `public Nullable<int> GetHoveredOptionIndex(UInt64 playerId)`
- `public IReadOnlyList<RestSiteOption> GetLocalOptions()`
- `public IReadOnlyList<RestSiteOption> GetOptionsForPlayer(UInt64 playerId)`
- `public IReadOnlyList<RestSiteOption> GetOptionsForPlayer(Player player)`
- `internal void HandleRestSiteOptionChosenMessage(OptionIndexChosenMessage message, UInt64 senderId)`
- `internal void HandleRestSiteOptionHoveredMessage(RestSiteOptionHoveredMessage message, UInt64 senderId)`
- `public void LocalOptionHovered(RestSiteOption option)`
- `internal Task QueueHoverMessage(int delayMsec)`
- `internal void SendHoverMessage()`
- `internal Task SendHoverMessageAfterSmallDelay()`
- `internal void TrySendHoverMessage()`

### `enum RewardSetCompleteState`

Values: `None`, `Completed`, `Skipped`

### `class RewardsSetState`

**Fields**

- `public TaskCompletionSource completionSource`
- `public RewardsSet set`

### `class RewardsSetSynchronizer : IDisposable`

**Properties**

- `internal Player LocalPlayer` { get }

**Fields**

- `private UInt64 _localPlayerId`
- `private Logger _logger`
- `private RunLocationTargetedMessageBuffer _messageBuffer`
- `private INetGameService _netService`
- `private IPlayerCollection _playerCollection`
- `private List<PlayerRewardState> _rewardStates`

**Methods**

- `public void BeforeLeavingRoom()`
- `public Task BeginRewardsSet(RewardsSet set)`
- `internal void CompleteRewardsSet(RewardsSetState setState, RewardSetCompleteState completeState)`
- `internal void CompleteRewardsSetIfNecessary(RewardsSetState setState)`
- `public void Dispose()`
- `internal PlayerRewardState GetRewardStateForPlayer(Player player)`
- `public void HandleRewardSelectedMessage(RewardSelectedMessage message, UInt64 senderId)`
- `public void HandleRewardSetSkippedMessage(RewardSetSkippedMessage message, UInt64 senderId)`
- `public bool IsRewardsSetCompleted(RewardsSet set)`
- `public bool IsRewardsSetCompleted(Player player, int id)`
- `public Task<bool> SelectLocalReward(Reward reward)`
- `internal Task SelectRewardForPlayer(Player player, int rewardIndex)`
- `internal Task<bool> SelectRewardForPlayer(RewardsSetState setState, Reward reward)`
- `public void SkipLocalRewardsSet()`
- `internal void SkipRewardsSet(RewardsSetState setState)`
- `internal RewardsSetState SkipRewardsSetOnStackTopForPlayer(Player player)`

### `class RewardSynchronizer : IDisposable`

**Properties**

- `internal Player LocalPlayer` { get }

**Fields**

- `private List<BufferedMessage> _bufferedMessages`
- `private INetGameService _gameService`
- `private UInt64 _localPlayerId`
- `private RunLocationTargetedMessageBuffer _messageBuffer`
- `private IPlayerCollection _playerCollection`

**Methods**

- `public void Dispose()`
- `public Task<bool> DoLocalCardRemoval()`
- `public Task<bool> DoUnsyncedCardRemoval(Player player)`
- `internal void HandleCardRemovedMessage(CardRemovedMessage message, UInt64 senderId)`
- `internal void HandleGoldLostMessage(GoldLostMessage message, UInt64 senderId)`
- `internal void HandleRewardObtainedMessage(RewardObtainedMessage message, UInt64 senderId)`
- `internal void OnCombatEnded(CombatRoom _)`
- `internal void SyncLocalCardEvent(CardModel card, bool skipped)`
- `public void SyncLocalGoldLost(int goldLost)`
- `public void SyncLocalObtainedCard(CardModel card)`
- `public void SyncLocalObtainedGold(int goldAmount)`
- `public void SyncLocalObtainedPotion(PotionModel potion)`
- `public void SyncLocalObtainedRelic(RelicModel relic)`
- `internal void SyncLocalPotionEvent(PotionModel potion, bool skipped)`
- `internal void SyncLocalRelicEvent(RelicModel relic, bool skipped)`
- `public void SyncLocalSkippedCard(CardModel card)`
- `public void SyncLocalSkippedPotion(PotionModel potion)`
- `public void SyncLocalSkippedRelic(RelicModel relic)`

### `class RunLocationTargetedMessageBuffer`

**Properties**

- `public RunLocation CurrentLocation` { get; set }

**Fields**

- `private INetGameService _gameService`
- `private Logger _logger`
- `private List<TypeAndMessageHandlers> _messageHandlers`
- `private List<BlockedMessage> _messagesWaitingOnLocationChange`
- `private HashSet<RunLocation> _visitedLocations`

**Methods**

- `internal void CallHandlersOfType(Type type, INetMessage message, UInt64 senderId)`
- `internal void HandleMessage(T message, UInt64 senderId)`
- `public void OnLocationChanged(RunLocation location)`
- `public void RegisterMessageHandler(MessageHandlerDelegate<T> handler)`
- `public void UnregisterMessageHandler(MessageHandlerDelegate<T> handler)`

### `class StateDivergenceException : Exception`

### `sealed class TrackedChecksum : ValueType`

**Fields**

- `public string context`
- `public NetChecksumData data`
- `public string fingerprintContext`
- `public NetFullCombatState fullState`

### `class TreasureRoomRelicSynchronizer`

**Properties**

- `public IReadOnlyList<RelicModel> CurrentRelics` { get }
- `internal Player LocalPlayer` { get }

**Fields**

- `private ActionQueueSynchronizer _actionQueueSynchronizer`
- `private List<RelicModel> _currentRelics`
- `private UInt64 _localPlayerId`
- `private Logger _logger`
- `private IPlayerCollection _playerCollection`
- `private PlayerVote _predictedVote`
- `private Rng _rng`
- `private RelicGrabBag _sharedGrabBag`
- `private bool _singlePlayerSkipped`
- `private List<PlayerVote> _votes`
- `private Action<List<RelicPickingResult>> RelicsAwarded`
- `private Action VotesChanged`

**Methods**

- `internal void AwardRelics()`
- `public void BeginRelicPicking()`
- `public void CompleteWithNoRelics()`
- `internal void EndRelicVoting()`
- `public PlayerVote GetPlayerVote(Player player)`
- `public void OnPicked(Player player, Nullable<int> index)`
- `public void OnRoomExited()`
- `public void PickRelicLocally(Nullable<int> index)`
- `public void SkipRelicLocally()`
- `internal RelicModel TryGetRelicForTutorial(UnlockState unlockState)`

### `sealed class TypeAndMessageHandlers : ValueType`

**Fields**

- `public List<MessageHandler> handlers`
- `public Type messageType`
- `public object netServiceHandler`

## MegaCrit.Sts2.Core.Nodes.Combat

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c__DisplayClass10_0`

**Fields**

- `public PlayCardAction action`

### `sealed class <>c__DisplayClass10_0`

**Fields**

- `public CardModel card`

### `sealed class <>c__DisplayClass11_0`

**Fields**

- `public NCard card`

### `sealed class <>c__DisplayClass13_0`

**Fields**

- `public PlayCardAction playCardAction`

### `sealed class <>c__DisplayClass14_0`

**Fields**

- `public CardModel card`

### `sealed class <>c__DisplayClass16_0`

**Fields**

- `public NCard card`

### `sealed class <>c__DisplayClass19_0`

**Fields**

- `public CardModel card`

### `sealed class <>c__DisplayClass41_0`

**Fields**

- `public Vector2 size`

### `sealed class <>c__DisplayClass64_0`

**Fields**

- `public CardModel card`

### `sealed class <>c__DisplayClass8_0`

**Fields**

- `public Creature owner`

### `sealed class <>c__DisplayClass8_0`

**Fields**

- `public PowerModel power`

### `sealed class <>c__DisplayClass89_0`

**Fields**

- `public NHandCardHolder holder`

### `sealed class <>c__DisplayClass9_0`

**Fields**

- `public GameAction action`

### `sealed class <>c__DisplayClass90_0`

**Fields**

- `public NPowerAppliedVfx vfx`

### `sealed class <>c__DisplayClass90_1`

**Fields**

- `public NPowerAppliedBuffVfx buffVfx`

### `sealed class <>c__DisplayClass90_2`

**Fields**

- `public NPowerAppliedDebuffVfx debuffVfx`

### `sealed class <>c__DisplayClass91_0`

**Fields**

- `public NPowerRemovedVfx vfx`

### `sealed class <>c__DisplayClass92_0`

**Fields**

- `public CardModel model`

### `sealed class <>c__DisplayClass92_0`

**Fields**

- `public NPowerFlashVfx vfx`

### `sealed class <>c__DisplayClass93_0`

**Fields**

- `public Func<CardModel, bool> filter`

### `sealed class <AnimateVfx>d__7 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <AnimDie>d__104 : ValueType, IAsyncStateMachine`

**Fields**

- `public CancellationToken cancelToken`
- `public bool shouldRemove`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <AnimInAfterDelay>d__25 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <ApplyLiquidOverlayInternal>d__56 : ValueType, IAsyncStateMachine`

**Fields**

- `public Color tint`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <Display>d__6 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <Display>d__8 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <LerpToMouse>d__29 : ValueType, IAsyncStateMachine`

**Fields**

- `public NHandCardHolder cardHolder`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <MultiCreatureTargeting>d__27 : ValueType, IAsyncStateMachine`

**Fields**

- `public TargetMode targetMode`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <PerformIntent>d__77 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <PlayAnim>d__14 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <ProceedWithoutRewards>d__95 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <RefreshIntents>d__78 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SelectCards>d__82 : ValueType, IAsyncStateMachine`

**Fields**

- `public Func<CardModel, bool> filter`
- `public Mode mode`
- `public CardSelectorPrefs prefs`
- `public AbstractModel source`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SelectionFinished>d__32 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <ShowRewards>d__96 : ValueType, IAsyncStateMachine`

**Fields**

- `public CombatRoom room`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SingleCreatureTargeting>d__25 : ValueType, IAsyncStateMachine`

**Fields**

- `public TargetMode targetMode`
- `public TargetType targetType`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SingleCreatureTargeting>d__8 : ValueType, IAsyncStateMachine`

**Fields**

- `public TargetType targetType`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <StartAsync>d__22 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <StartCardDrag>d__23 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <TargetSelection>d__24 : ValueType, IAsyncStateMachine`

**Fields**

- `public TargetMode targetMode`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class CanceledEventHandler : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke()`

### `sealed class ConfirmedEventHandler : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke()`

### `sealed class CreatureHoveredEventHandler : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(NCreature creature, AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke(NCreature creature)`

### `sealed class CreatureUnhoveredEventHandler : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(NCreature creature, AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke(NCreature creature)`

### `sealed class FinishedEventHandler : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(bool success, AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke(bool success)`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `class MethodName : MethodName`

### `enum Mode`

Values: `None`, `Play`, `SimpleSelect`, `UpgradeSelect`

### `sealed class ModeChangedEventHandler : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke()`

### `abstract class NCardPlay : Node`

**Properties**

- `internal CardModel Card` { get }
- `internal NCard CardNode` { get }
- `internal NCreature CardOwnerNode` { get }
- `public NHandCardHolder Holder` { get; set }
- `public Player Player` { get; set }

**Fields**

- `private bool _isTryingToPlayCard`
- `protected Viewport _viewport`
- `private FinishedEventHandler backing_Finished`

**Methods**

- `public virtual void _Ready()`
- `internal void AutoDisableCannotPlayCardFtueCheck()`
- `public void CancelPlayCard()`
- `protected void CannotPlayThisCardFtueCheck(CardModel card)`
- `protected void CenterCard()`
- `protected virtual void Cleanup(bool isFinished)`
- `internal void ClearTarget()`
- `protected void EmitSignalFinished(bool success)`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool HasGodotClassSignal(godot_string_name& signal)`
- `internal void HideEvokingOrbs()`
- `protected void HideTargetingVisuals()`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `protected virtual void OnCancelPlayCard()`
- `protected void OnCreatureHover(NCreature creature)`
- `protected void OnCreatureUnhover(NCreature _)`
- `protected virtual void RaiseGodotClassSignalCallbacks(godot_string_name& signal, NativeVariantPtrArgs args)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected void ShowMultiCreatureTargetingVisuals()`
- `public virtual void Start()`
- `protected void TryPlayCard(Creature target)`
- `protected void TryShowEvokingOrbs()`

### `class NCardPlayQueue : Control`

**Fields**

- `private List<QueueItem> _playQueue`

**Methods**

- `public virtual void _ExitTree()`
- `public virtual void _Ready()`
- `public void AnimOut()`
- `internal void BeforeRemoteCardPlayResumedAfterPlayerChoice(GameAction action)`
- `public NCard GetCardNode(CardModel card)`
- `internal Vector2 GetPositionForQueueIndex(NCard card, int index)`
- `internal Vector2 GetScaleForQueueIndex(int index)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnActionEnqueued(GameAction action)`
- `public void OnLocalCardPlayed(PlayCardAction action, NCardHolder holder, CardModel card)`
- `public void ReAddCardAfterPlayerChoice(NCard card, GameAction action)`
- `internal void RemoveCardFromQueue(NCard card)`
- `internal void RemoveCardFromQueue(int index)`
- `public void RemoveCardFromQueueForCancellation(PlayCardAction action)`
- `public void RemoveCardFromQueueForCancellation(NCard card, bool forceReturnToHand)`
- `internal void RemoveCardFromQueueForCancellation(int index, bool forceReturnToHand)`
- `public void RemoveCardFromQueueForExecution(CardModel card)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `internal void TweenAllToQueuePosition()`
- `internal void TweenCardForCancellation(QueueItem item)`
- `internal void TweenCardToQueuePosition(QueueItem item, int queueIndex)`
- `public void UpdateCardBeforeExecution(PlayCardAction playCardAction)`
- `internal void UpdateCardVisuals(QueueItem item)`

### `abstract class NCombatCardPile : NButton`

**Properties**

- `internal PileType Pile` { get }

**Fields**

- `private Tween _bumpTween`
- `private MegaLabel _countLabel`
- `private int _currentCount`
- `protected LocString _emptyPileMessage`
- `protected Vector2 _hidePosition`
- `private HoverTip _hoverTip`
- `private Control _icon`
- `private Player _localPlayer`
- `private CardPile _pile`
- `private Tween _positionTween`
- `protected Vector2 _showPosition`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Ready()`
- `protected virtual void AddCard()`
- `public virtual void AnimIn()`
- `public void AnimOut()`
- `protected virtual void ConnectSignals()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `public virtual void Initialize(Player player)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `protected virtual void OnFocus()`
- `protected virtual void OnPress()`
- `protected virtual void OnRelease()`
- `protected virtual void OnUnfocus()`
- `internal void RemoveCard()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SetAnimInOutPositions()`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`

### `class NCombatPilesContainer : Control`

**Properties**

- `public NDiscardPileButton DiscardPile` { get }
- `public NDrawPileButton DrawPile` { get }
- `public NExhaustPileButton ExhaustPile` { get }

**Fields**

- `private NDiscardPileButton _discardPile`
- `private NDrawPileButton _drawPile`
- `private NExhaustPileButton _exhaustPile`

**Methods**

- `public virtual void _Ready()`
- `public void AnimIn()`
- `public void AnimOut()`
- `public void Disable()`
- `public void Enable()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `public void Initialize(Player player)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`

### `class NCombatSceneContainer : Control`

**Fields**

- `private Control _bgContainer`
- `private Window _window`

**Methods**

- `public virtual void _Ready()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnWindowChange()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`

### `class NCombatStartBanner : Control`

**Fields**

- `private ColorRect _colorRect`
- `private MegaLabel _label`

**Methods**

- `public virtual void _Ready()`
- `internal Task AnimateVfx()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`

### `class NCombatUi : Control`

**Properties**

- `public Control CardPreviewContainer` { get; set }
- `public NDiscardPileButton DiscardPile` { get }
- `public NDrawPileButton DrawPile` { get }
- `public NEndTurnButton EndTurnButton` { get; set }
- `public Control EnergyCounterContainer` { get; set }
- `public NExhaustPileButton ExhaustPile` { get }
- `public NPlayerHand Hand` { get; set }
- `public NMessyCardPreviewContainer MessyCardPreviewContainer` { get; set }
- `internal NPingButton PingButton` { get; set }
- `public Control PlayContainer` { get; set }
- `internal IEnumerable<NCard> PlayContainerCards` { get }
- `public NCardPlayQueue PlayQueue` { get; set }

**Fields**

- `private NCombatPilesContainer _combatPilesContainer`
- `private CancellationTokenSource _cts`
- `private NEnergyCounter _energyCounter`
- `private int _originalHandChildIndex`
- `private Dictionary<NCard, Vector2> _originalPlayContainerCardPositions`
- `private Dictionary<NCard, Vector2> _originalPlayContainerCardScales`
- `private Tween _playContainerPeekModeTween`
- `private NStarCounter _starCounter`
- `private CombatState _state`
- `private Action DebugToggleHpBar`
- `private Action DebugToggleIntent`

**Methods**

- `public virtual void _ExitTree()`
- `public virtual void _Input(InputEvent inputEvent)`
- `public virtual void _Ready()`
- `public void Activate(CombatState state)`
- `public void AddToPlayContainer(NCard card)`
- `internal void AnimIn()`
- `public void AnimOut()`
- `public void Deactivate()`
- `internal void DebugHideCombatUi()`
- `public void Disable()`
- `internal void DisconnectSignals()`
- `public void Enable()`
- `public NCard GetCardFromPlayContainer(CardModel model)`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnCombatEnded(CombatRoom combatRoom)`
- `internal void OnCombatWon(CombatRoom room)`
- `public void OnHandSelectModeEntered()`
- `public void OnHandSelectModeExited()`
- `public void OnPeekButtonReady(NPeekButton peekButton)`
- `internal void OnPeekButtonToggled(NPeekButton peekButton)`
- `internal void PostCombatCleanUp()`
- `internal Task ProceedWithoutRewards()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `internal Task ShowRewards(CombatRoom room)`

### `class NControllerCardPlay : NCardPlay`

**Fields**

- `private Callable _onCreatureHoverCallable`
- `private Callable _onCreatureUnhoverCallable`
- `private bool _signalsConnected`
- `private CanceledEventHandler backing_Canceled`
- `private ConfirmedEventHandler backing_Confirmed`

**Methods**

- `public virtual void _ExitTree()`
- `public virtual void _Input(InputEvent inputEvent)`
- `protected virtual void Cleanup(bool isFinished)`
- `internal void DisconnectTargetingSignals()`
- `protected void EmitSignalCanceled()`
- `protected void EmitSignalConfirmed()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool HasGodotClassSignal(godot_string_name& signal)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void MultiCreatureTargeting()`
- `protected virtual void OnCancelPlayCard()`
- `protected virtual void RaiseGodotClassSignalCallbacks(godot_string_name& signal, NativeVariantPtrArgs args)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `internal Task SingleCreatureTargeting(TargetType targetType)`
- `public virtual void Start()`

### `class NCreature : Control`

**Properties**

- `public Node2D Body` { get }
- `public Task DeathAnimationTask` { get; set }
- `public CancellationTokenSource DeathAnimCancelToken` { get }
- `public Creature Entity` { get; set }
- `public bool HasSpineAnimation` { get }
- `public Control Hitbox` { get; set }
- `public Control IntentContainer` { get; set }
- `public bool IsFocused` { get; set }
- `public bool IsInteractable` { get; set }
- `public bool IsPlayingDeathAnimation` { get }
- `public NOrbManager OrbManager` { get; set }
- `public NMultiplayerPlayerIntentHandler PlayerIntentHandler` { get; set }
- `public Vector2 PowerAppliedVfxSpawnPosition` { get }
- `public SpineAnimationAccess SpineAnimation` { get }
- `public Vector2 VfxSpawnPosition` { get }
- `public NCreatureVisuals Visuals` { get; set }

**Fields**

- `private Tween _intentFadeTween`
- `private bool _isInBestiary`
- `private bool _isInMultiselect`
- `private bool _isRemotePlayerOrPet`
- `private Tween _scaleTween`
- `private NSelectionReticle _selectionReticle`
- `private Dictionary<string, ValueTuple<string, float>> _sfxLoops`
- `private Tween _shakeTween`
- `private CreatureAnimator _spineAnimator`
- `private NCreatureStateDisplay _stateDisplay`
- `private float _tempScale`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Ready()`
- `internal Task AnimDie(bool shouldRemove, CancellationToken cancelToken)`
- `public Tween AnimDisableUi()`
- `public Tween AnimEnableUi()`
- `public void AnimHideIntent(double delay)`
- `public void AnimShake()`
- `internal void AnimTempRevive()`
- `internal void ConnectSpineAnimatorSignals()`
- `internal void DoScaleTween(Vector2 scale)`
- `public Vector2 GetBottomOfHitbox()`
- `public float GetCurrentAnimationLength()`
- `public float GetCurrentAnimationTimeRemaining()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `public T GetSpecialNode(string name)`
- `public Vector2 GetTopOfHitbox()`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `public void HideHoverTips()`
- `public void HideMultiselectReticle()`
- `public void HideSingleSelectReticle()`
- `internal void ImmediatelySetIdle()`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnCombatEnded(CombatRoom _)`
- `internal void OnFocus()`
- `internal void OnPowerApplied(PowerModel power)`
- `internal void OnPowerFlashed(PowerModel power)`
- `internal void OnPowerIncreased(PowerModel power, int amount, bool silent)`
- `internal void OnPowerRemoved(PowerModel power)`
- `public void OnTargetingStarted()`
- `internal void OnUnfocus()`
- `public void OstyScaleToSize(float ostyHealth, double duration)`
- `public Task PerformIntent()`
- `public Task RefreshIntents()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `internal Task RevealIntents()`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `public void ScaleTo(float size, double duration)`
- `public void SetAnimationTrigger(string trigger)`
- `public void SetDefaultScaleTo(float size, float duration)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `internal void SetOrbManagerPosition()`
- `public void SetRemotePlayerFocused(bool remotePlayerFocused)`
- `public void SetScaleAndHue(float scale, float hue)`
- `public void SetupForBestiary()`
- `internal void ShowCreatureHoverTips(CombatState _)`
- `public void ShowHoverTips(IEnumerable<IHoverTip> hoverTips)`
- `public void ShowMultiselectReticle()`
- `public void ShowSingleSelectReticle()`
- `public float StartDeathAnim(bool shouldRemove)`
- `public void StartReviveAnim()`
- `public void StartSfxLoop(string sfxName)`
- `public void StartSfxLoop(string sfxName, string loopParam, float loopStopValue)`
- `public void StopAllSfxLoops()`
- `public void StopSfxLoop(string sfxName)`
- `internal void SubscribeToPower(PowerModel power)`
- `public void ToggleIsInteractable(bool on)`
- `public void TrackBlockStatus(Creature creature)`
- `internal void UnsubscribeFromPower(PowerModel power)`
- `internal void UpdateBounds(string boundsNodeName)`
- `internal void UpdateBounds(Node boundsContainer)`
- `public Task UpdateIntent(IEnumerable<Creature> targets)`
- `public void UpdateNavigation()`

### `class NCreatureStateDisplay : Control`

**Fields**

- `private Creature _blockTrackingCreature`
- `private Creature _creature`
- `private Vector2 _creatureSize`
- `private NHealthBar _healthBar`
- `private Tween _hoverTween`
- `private Control _hpBarHitbox`
- `private Control _nameplateContainer`
- `private MegaLabel _nameplateLabel`
- `private Vector2 _originalPosition`
- `private NPowerContainer _powerContainer`
- `private Tween _showHideTween`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Ready()`
- `public void AnimateIn(HealthBarAnimMode mode)`
- `internal void AnimateInBlock(int oldBlock, int blockGain)`
- `public void AnimateOut()`
- `internal void DebugToggleVisibility()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `public void HideImmediately()`
- `public void HideNameplate()`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnBlockTrackingCreatureBlockChanged(int oldBlock, int blockGain)`
- `internal void OnCombatStateChanged(CombatState _)`
- `internal void OnCreatureDied(Creature _)`
- `internal void OnCreatureRevived(Creature _)`
- `internal void OnHovered()`
- `internal void OnUnhovered()`
- `internal void RefreshValues()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `public void SetCreature(Creature creature)`
- `public void SetCreatureBounds(Control bounds)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `public void ShowNameplate()`
- `internal void SubscribeToCreatureEvents()`
- `public void TrackBlockStatus(Creature creature)`

### `class NCreatureVisuals : Node2D`

**Properties**

- `public Control Bounds` { get; set }
- `public float DefaultScale` { get; set }
- `public bool HasSpineAnimation` { get }
- `public Marker2D IntentPosition` { get; set }
- `internal bool IsSpineNode` { get }
- `public bool IsUsingPhobiaModeBody` { get }
- `public Marker2D OrbPosition` { get; set }
- `public SpineAnimationAccess SpineAnimation` { get }
- `public MegaSprite SpineBody` { get; set }
- `public Marker2D TalkPosition` { get; set }
- `public Marker2D VfxSpawnPosition` { get; set }

**Fields**

- `private Node2D _body`
- `private CancellationTokenSource _cts`
- `private ShaderMaterial _currentLiquidOverlayMaterial`
- `private float _hue`
- `private double _liquidOverlayTimer`
- `private Node2D _phobiaModeBody`
- `private Material _savedNormalMaterial`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Ready()`
- `internal Task ApplyLiquidOverlayInternal(Color tint)`
- `public Node2D GetCurrentBody()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `public bool IsPlayingHurtAnimation()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `public void SetScaleAndHue(float scale, float hue)`
- `public void SetUpSkin(MonsterModel model)`
- `public void TryApplyLiquidOverlay(Color tint)`
- `internal void UpdatePhobiaMode()`

### `class NDiscardPileButton : NCombatCardPile`

**Properties**

- `internal String[] Hotkeys` { get }
- `internal PileType Pile` { get }

**Methods**

- `public virtual void _Ready()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SetAnimInOutPositions()`

### `class NDrawPileButton : NCombatCardPile`

**Properties**

- `internal String[] Hotkeys` { get }
- `internal PileType Pile` { get }

**Methods**

- `public virtual void _Ready()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SetAnimInOutPositions()`

### `class NEndTurnButton : NButton`

**Properties**

- `internal bool CanTurnBeEnded` { get }
- `internal Vector2 HidePos` { get }
- `internal String[] Hotkeys` { get }
- `internal Vector2 ShowPos` { get }

**Fields**

- `private CombatState _combatState`
- `private NCombatUi _combatUi`
- `private int _endTurnWithNoPlayableCardsCount`
- `private Control _glow`
- `private Tween _glowEnableTween`
- `private Texture2D _glowTexture`
- `private Control _glowVfx`
- `private Tween _glowVfxTween`
- `private HoverTip _hoverTip`
- `private Tween _hoverTween`
- `private ShaderMaterial _hsv`
- `private TextureRect _image`
- `private bool _isShiny`
- `private MegaLabel _label`
- `private NEndTurnLongPressBar _longPressBar`
- `private Texture2D _normalTexture`
- `private CardPile _playerHand`
- `private NMultiplayerVoteContainer _playerIconContainer`
- `private Tween _positionTween`
- `private float _pulseTimer`
- `private State _state`
- `private Viewport _viewport`
- `private Control _visuals`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Ready()`
- `internal void AfterPlayerEndedTurn(Player player, bool canBackOut)`
- `internal void AfterPlayerUnendedTurn(Player player)`
- `internal void AnimIn()`
- `internal void AnimOut()`
- `public void CallReleaseLogic()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `internal void GlowPulse()`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `internal bool HasPlayableCard()`
- `public void Initialize(CombatState state)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnAboutToSwitchToEnemyTurn(CombatState _)`
- `public void OnCombatEnded()`
- `internal void OnCombatStateChanged(CombatState combatState)`
- `protected virtual void OnDisable()`
- `protected virtual void OnEnable()`
- `protected virtual void OnFocus()`
- `protected virtual void OnPress()`
- `protected virtual void OnRelease()`
- `internal void OnTurnStarted(CombatState state)`
- `protected virtual void OnUnfocus()`
- `internal bool PlayerCanTakeAction(Player player)`
- `public void RefreshEnabled()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `public void SecretEndTurnLogicViaFtue()`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `internal void SetState(State newState)`
- `internal bool ShouldDisplayPlayerIcon(Player player)`
- `internal bool ShouldShowPlayableCardsFtue()`
- `internal void StartOrStopPulseVfx()`
- `internal void UpdateShaderV(float value)`

### `class NEndTurnLongPressBar : ColorRect`

**Fields**

- `private bool _enabled`
- `private NEndTurnButton _endTurnButton`
- `private bool _isPressed`
- `private Control _outline`
- `private double _pressTimer`
- `private Tween _tween`

**Methods**

- `public virtual void _Process(double delta)`
- `public virtual void _Ready()`
- `public void CancelPress()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `public void Init(NEndTurnButton endTurnButton)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal Task PlayAnim()`
- `internal void RecalculateBar()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `public void StartPress()`

### `class NEnemyTurnBanner : Control`

**Fields**

- `private MegaLabel _label`

**Methods**

- `public virtual void _Ready()`
- `internal Task Display()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`

### `class NEnergyCounter : Control`

**Properties**

- `internal Color OutlineColor` { get }

**Fields**

- `private Tween _animInTween`
- `private Tween _animOutTween`
- `private NParticlesContainer _backVfx`
- `private NParticlesContainer _frontVfx`
- `private HoverTip _hoverTip`
- `private MegaLabel _label`
- `private Control _layers`
- `private Player _player`
- `private Control _rotationLayers`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Process(double delta)`
- `public virtual void _Ready()`
- `public void AnimIn()`
- `public void AnimOut()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnCombatStateChanged(CombatState combatState)`
- `internal void OnEnergyChanged(int oldEnergy, int newEnergy)`
- `internal void OnHovered()`
- `internal void OnUnhovered()`
- `internal void RefreshLabel()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`

### `class NExhaustPileButton : NCombatCardPile`

**Properties**

- `internal String[] Hotkeys` { get }
- `internal PileType Pile` { get }

**Fields**

- `private Vector2 _posOffset`
- `private Viewport _viewport`

**Methods**

- `public virtual void _Ready()`
- `protected virtual void AddCard()`
- `public virtual void AnimIn()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `public virtual void Initialize(Player player)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SetAnimInOutPositions()`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`

### `class NHealthBar : Control`

**Properties**

- `public Control HpBarContainer` { get; set }
- `internal float MaxFgWidth` { get }

**Fields**

- `private Control _blockContainer`
- `private MegaLabel _blockLabel`
- `private Control _blockOutline`
- `private Creature _blockTrackingCreature`
- `private Tween _blockTween`
- `private Creature _creature`
- `private int _currentHpOnLastRefresh`
- `private Control _doomForeground`
- `private float _expectedMaxFgWidth`
- `private LocString _healthBarDead`
- `private Control _hpForeground`
- `private Control _hpForegroundContainer`
- `private MegaLabel _hpLabel`
- `private Tween _hpLabelFadeTween`
- `private Control _hpMiddleground`
- `private TextureRect _infinityTex`
- `private int _maxHpOnLastRefresh`
- `private Tween _middlegroundTween`
- `private Vector2 _originalBlockPosition`
- `private Control _poisonForeground`

**Methods**

- `public virtual void _Ready()`
- `public void AnimateInBlock(int oldBlock, int blockGain)`
- `internal void DebugToggleVisibility()`
- `public void FadeInHpLabel(float duration)`
- `public void FadeOutHpLabel(float duration, float finalAlpha)`
- `internal float GetFgWidth(int amount)`
- `internal float GetFgWidth(int amount, float maxFgWidth)`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal bool IsDoomLethal(int doomAmount, int poisonDamage)`
- `internal bool IsPoisonLethal(int poisonDamage)`
- `internal void RefreshBlockUi()`
- `internal void RefreshForeground()`
- `internal void RefreshMiddleground()`
- `internal void RefreshText()`
- `public void RefreshValues()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `public void SetCreature(Creature creature)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `internal void SetHpBarContainerSizeWithOffsets(Vector2 size)`
- `internal void SetHpBarContainerSizeWithOffsetsImmediately(Vector2 size)`
- `public void TrackBlockStatus(Creature creature)`
- `public void UpdateLayoutForCreatureBounds(Control bounds)`
- `public void UpdateWidthRelativeToReferenceValue(float refMaxHp, float refWidth)`

### `class NIntent : Control`

**Fields**

- `private Nullable<int> _animationFrame`
- `private List<Texture2D> _animationFrames`
- `private string _animationName`
- `private NCombatRoom _combatRoom`
- `private AbstractIntent _intent`
- `private Control _intentHolder`
- `private CpuParticles2D _intentParticle`
- `private Sprite2D _intentSprite`
- `private bool _isFrozen`
- `private Creature _owner`
- `private IEnumerable<Creature> _targets`
- `private float _timeAccumulator`
- `private float _timeOffset`
- `private MegaRichTextLabel _valueLabel`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Process(double delta)`
- `public virtual void _Ready()`
- `internal void DebugToggleVisibility()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnCombatStateChanged(CombatState _)`
- `internal void OnHovered()`
- `internal void OnUnhovered()`
- `public void PlayPerform()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `public void SetFrozen(bool isFrozen)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `public void UpdateIntent(AbstractIntent intent, IEnumerable<Creature> targets, Creature owner)`
- `internal void UpdateVisuals()`

### `class NMouseCardPlay : NCardPlay`

**Properties**

- `internal float CancelZoneThreshold` { get }
- `internal float PlayZoneThreshold` { get }

**Fields**

- `private CancellationTokenSource _cancellationTokenSource`
- `private StringName _cancelShortcut`
- `private float _dragStartYPosition`
- `private bool _isLeftMouseDown`
- `private Callable _onCreatureHoverCallable`
- `private Callable _onCreatureUnhoverCallable`
- `private bool _signalsConnected`
- `private bool _skipStartCardDrag`
- `private Creature _target`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Input(InputEvent inputEvent)`
- `internal void DisconnectTargetingSignals()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal bool IsCardInCancelZone()`
- `internal bool IsCardInPlayZone()`
- `internal Task LerpToMouse(NHandCardHolder cardHolder)`
- `internal Task MultiCreatureTargeting(TargetMode targetMode)`
- `protected virtual void OnCancelPlayCard()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `internal Task SingleCreatureTargeting(TargetMode targetMode, TargetType targetType)`
- `public virtual void Start()`
- `internal Task StartAsync()`
- `internal Task StartCardDrag()`
- `internal Task TargetSelection(TargetMode targetMode)`

### `sealed class NodeHoveredEventHandler : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(Node node, AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke(Node node)`

### `sealed class NodeUnhoveredEventHandler : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(Node node, AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke(Node node)`

### `class NPeekButton : NButton`

**Properties**

- `public Marker2D CurrentCardMarker` { get; set }
- `internal String[] Hotkeys` { get }
- `public bool IsPeeking` { get; set }

**Fields**

- `private TextureRect _flash`
- `private List<Control> _hiddenTargets`
- `private Tween _hoverTween`
- `private IOverlayScreen _overlayScreenParent`
- `private List<Control> _targets`
- `private Control _visuals`
- `private Tween _wiggleTween`
- `private ToggledEventHandler backing_Toggled`

**Methods**

- `public virtual void _Ready()`
- `public void AddTargets(Control[] targets)`
- `protected void EmitSignalToggled(NPeekButton peekButton)`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool HasGodotClassSignal(godot_string_name& signal)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnCombatRoomReady()`
- `protected virtual void OnDisable()`
- `protected virtual void OnEnable()`
- `protected virtual void OnFocus()`
- `internal void OnOverlayStackChanged()`
- `protected virtual void OnPress()`
- `protected virtual void OnRelease()`
- `protected virtual void OnUnfocus()`
- `protected virtual void RaiseGodotClassSignalCallbacks(godot_string_name& signal, NativeVariantPtrArgs args)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `public void SetPeeking(bool isPeeking)`
- `public void Wiggle()`

### `class NPingButton : NButton`

**Properties**

- `internal Vector2 HidePos` { get }
- `internal String[] Hotkeys` { get }
- `internal Vector2 ShowPos` { get }

**Fields**

- `private Tween _hoverTween`
- `private ShaderMaterial _hsv`
- `private TextureRect _image`
- `private MegaLabel _label`
- `private Tween _positionTween`
- `private CancellationTokenSource _showCancelTokenSource`
- `private State _state`
- `private Viewport _viewport`
- `private Control _visuals`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Ready()`
- `internal void AfterPlayerEndedTurn(Player player, bool _)`
- `internal void AfterPlayerUnendedTurn(Player player)`
- `internal void AnimIn()`
- `internal Task AnimInAfterDelay()`
- `internal void AnimOut()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnAboutToSwitchToEnemyTurn(CombatState _)`
- `public void OnCombatEnded()`
- `protected virtual void OnDisable()`
- `protected virtual void OnEnable()`
- `protected virtual void OnFocus()`
- `protected virtual void OnPress()`
- `protected virtual void OnRelease()`
- `protected virtual void OnUnfocus()`
- `public void RefreshEnabled()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `internal void SetState(State newState)`
- `internal void UpdateShaderV(float value)`

### `class NPlayerHand : Control`

**Properties**

- `public IReadOnlyList<NHandCardHolder> ActiveHolders` { get }
- `public Control CardHolderContainer` { get; set }
- `public Mode CurrentMode` { get; set }
- `public Control DefaultFocusedControl` { get }
- `public NHandCardHolder FocusedHolder` { get; set }
- `internal bool HasDraggedHolder` { get }
- `internal IReadOnlyList<NHandCardHolder> Holders` { get }
- `public bool InCardPlay` { get }
- `public bool IsInCardSelection` { get }
- `public NPeekButton PeekButton` { get; set }
- `public Func<CardModel, bool> SelectModeGoldGlowOverride` { get }

**Fields**

- `private Tween _animEnableTween`
- `private Tween _animInTween`
- `private Tween _animOutTween`
- `private CombatState _combatState`
- `private NCardPlay _currentCardPlay`
- `private Mode _currentMode`
- `private Func<CardModel, bool> _currentSelectionFilter`
- `private int _draggedHolderIndex`
- `private Dictionary<NHandCardHolder, int> _holdersAwaitingQueue`
- `private bool _isDisabled`
- `private int _lastFocusedHolderIdx`
- `private CardSelectorPrefs _prefs`
- `private StringName[] _selectCardShortcuts`
- `private List<CardModel> _selectedCards`
- `private Tween _selectedCardScaleTween`
- `private NSelectedHandCardContainer _selectedHandCardContainer`
- `private TaskCompletionSource<IEnumerable<CardModel>> _selectionCompletionSource`
- `private MegaRichTextLabel _selectionHeader`
- `private Control _selectModeBackstop`
- `private NConfirmButton _selectModeConfirmButton`
- `private NUpgradePreview _upgradePreview`
- `private Control _upgradePreviewContainer`
- `private ModeChangedEventHandler backing_ModeChanged`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Ready()`
- `public virtual void _UnhandledInput(InputEvent input)`
- `public NHandCardHolder Add(NCard card, int index)`
- `internal void AddCardHolder(NHandCardHolder holder, int index)`
- `internal void AfterCardsSelected(AbstractModel source)`
- `internal void AnimDisable()`
- `internal void AnimEnable()`
- `public void AnimIn()`
- `public void AnimOut()`
- `internal bool AreCardActionsAllowed()`
- `public void CancelAllCardPlay()`
- `internal void CancelHandSelectionIfNecessary()`
- `internal bool CanPlayCards()`
- `internal void CheckIfSelectionComplete()`
- `public void DeselectCard(NCard card)`
- `public void DisableControllerNavigation()`
- `protected void EmitSignalModeChanged()`
- `public void EnableControllerNavigation()`
- `public void FlashPlayableHolders()`
- `public void ForceRefreshCardIndices()`
- `public NCard GetCard(CardModel card)`
- `public NCardHolder GetCardHolder(CardModel card)`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool HasGodotClassSignal(godot_string_name& signal)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `public bool IsAwaitingPlay(NHandCardHolder holder)`
- `internal void OnCardDeselected(Node _)`
- `internal void OnCardSelected(Node _)`
- `internal void OnCombatEnded(CombatRoom _)`
- `internal void OnCombatStateChanged(CombatState state)`
- `internal void OnHolderFocused(NHandCardHolder holder)`
- `internal void OnHolderPressed(NCardHolder holder)`
- `internal void OnHolderUnfocused(NHandCardHolder holder)`
- `internal void OnPeekButtonToggled(NPeekButton button)`
- `internal void OnPlayerActionsDisabledChanged(CombatState state)`
- `internal void OnPlayerUnendedTurn(Player player)`
- `internal void OnSelectModeConfirmButtonPressed(NButton _)`
- `internal void OnSelectModeSourceFinished(AbstractModel source)`
- `protected virtual void RaiseGodotClassSignalCallbacks(godot_string_name& signal, NativeVariantPtrArgs args)`
- `internal void RefreshLayout()`
- `internal void RefreshSelectModeConfirmButton()`
- `public void Remove(CardModel card)`
- `public void RemoveCardHolder(NCardHolder holder)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `internal void ReturnHolderToHand(NHandCardHolder holder)`
- `internal void RevalidateSelectionAfterStateChange()`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `internal void SelectCardInSimpleMode(NHandCardHolder holder)`
- `internal void SelectCardInUpgradeMode(NHandCardHolder holder)`
- `public Task<IEnumerable<CardModel>> SelectCards(CardSelectorPrefs prefs, Func<CardModel, bool> filter, AbstractModel source, Mode mode)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `internal void StartCardPlay(NHandCardHolder holder, bool startedViaShortcut)`
- `public void TryCancelCardPlay(CardModel card)`
- `internal void UpdateHandDisabledState(ICombatState state)`
- `internal void UpdateSelectedCardContainer(int count)`
- `internal void UpdateSelectModeCardVisibility()`

### `class NPlayerTurnBanner : Control`

**Fields**

- `private MegaLabel _label`
- `private int _roundNumber`
- `private MegaLabel _turnLabel`

**Methods**

- `public virtual void _Ready()`
- `internal Task Display()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`

### `class NPower : Control`

**Properties**

- `public NPowerContainer Container` { get; set }
- `public PowerModel Model` { get; set }

**Fields**

- `private MegaLabel _amountLabel`
- `private Tween _animInTween`
- `private TextureRect _icon`
- `private PowerModel _model`
- `private CpuParticles2D _powerFlash`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Ready()`
- `internal void FlashPower()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnDisplayAmountChanged()`
- `internal void OnHovered()`
- `internal void OnOwnerDied(Creature _)`
- `internal void OnOwnerRevived(Creature _)`
- `internal void OnPowerFlashed(PowerModel _)`
- `internal void OnPowerRemoved()`
- `internal void OnPulsingStarted()`
- `internal void OnPulsingStopped()`
- `internal void OnUnhovered()`
- `internal void RefreshAmount()`
- `internal void Reload()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `internal void ShowPowerHoverTips(CombatState _)`
- `internal void SubscribeToModelEvents()`
- `internal void UnsubscribeFromModelEvents()`

### `class NPowerContainer : Control`

**Fields**

- `private Creature _creature`
- `private Nullable<Vector2> _originalPosition`
- `private List<NPower> _powerNodes`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `internal void Add(PowerModel power)`
- `internal void ConnectCreatureSignals()`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnPowerApplied(PowerModel power)`
- `internal void OnPowerRemoved(PowerModel power)`
- `internal void Remove(PowerModel power)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `public void SetCreature(Creature creature)`
- `public void SetCreatureBounds(Control bounds)`
- `internal void UpdatePositions()`

### `class NRemoteTargetingIndicator : Node2D`

**Fields**

- `private Vector2 _fromPosition`
- `private bool _isTargetingCreature`
- `private Line2D _line`
- `private Line2D _lineBack`
- `private Player _player`
- `private Vector2 _toPosition`
- `private Tween _tween`

**Methods**

- `public virtual void _Process(double delta)`
- `public virtual void _Ready()`
- `internal void DoTargetingCreatureTween(bool isTargetingCreature)`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `public void Initialize(Player player)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `public void StartDrawingFrom(Vector2 from)`
- `public void StopDrawing()`
- `public void UpdateDrawingTo(Vector2 position)`

### `class NSelectedHandCardContainer : Control`

**Properties**

- `public NPlayerHand Hand` { get; set }
- `public List<NSelectedHandCardHolder> Holders` { get }

**Methods**

- `public virtual void _Ready()`
- `public NSelectedHandCardHolder Add(NHandCardHolder originalHolder)`
- `public void DeselectCard(CardModel card)`
- `internal void DeselectHolder(NCardHolder holder)`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnFocus()`
- `internal void RefreshHolderPositions()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`

### `class NSelectionReticle : Control`

**Properties**

- `public bool IsSelected` { get; set }

**Fields**

- `private CancellationTokenSource _cancelToken`
- `private Tween _currentTween`

**Methods**

- `public virtual void _ExitTree()`
- `public virtual void _Ready()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `public void OnDeselect()`
- `public void OnSelect()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`

### `class NStarCounter : Control`

**Fields**

- `private int _displayedStarCount`
- `private HoverTip _hoverTip`
- `private ShaderMaterial _hsv`
- `private Tween _hsvTween`
- `private Control _icon`
- `private bool _isListeningToCombatState`
- `private MegaRichTextLabel _label`
- `private float _lerpingStarCount`
- `private Player _player`
- `private Control _rotationLayers`
- `private float _velocity`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Process(double delta)`
- `public virtual void _Ready()`
- `internal void ConnectStarsChangedSignal()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `public void Initialize(Player player)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnHovered()`
- `internal void OnStarsChanged(int oldStars, int newStars)`
- `internal void OnUnhovered()`
- `internal void RefreshVisibility()`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `internal void SetStarCountText(int stars)`
- `internal void UpdateShaderV(float value)`
- `internal void UpdateStarCount(int oldCount, int newCount)`

### `class NTargetingArrow : Node2D`

**Properties**

- `internal Vector2 From` { get }

**Fields**

- `private Sprite2D _arrowHead`
- `private Tween _arrowHeadTween`
- `private Nullable<Vector2> _currentArrowPos`
- `private bool _followMouse`
- `private Control _fromControl`
- `private Vector2 _fromPos`
- `private bool _initialized`
- `private Sprite2D[] _segments`
- `private Vector2 _toPosition`

**Methods**

- `public virtual void _Process(double delta)`
- `public virtual void _Ready()`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `public void SetHighlightingOff()`
- `public void SetHighlightingOn(bool isEnemy)`
- `public void StartDrawingFrom(Vector2 from, bool usingController)`
- `public void StartDrawingFrom(Control control, bool usingController)`
- `public void StopDrawing()`
- `internal void UpdateArrowPosition(Vector2 targetPos)`
- `public void UpdateDrawingTo(Vector2 position)`
- `internal void UpdateSegments(Vector2 initialPos, Vector2 finalPos, Vector2 controlPoint)`

### `class NTargetManager : Node2D`

**Properties**

- `internal Node HoveredNode` { get; set }
- `public bool IsInSelection` { get }
- `public long LastTargetingFinishedFrame` { get; set }

**Fields**

- `private TaskCompletionSource<Node> _completionSource`
- `private Func<bool> _exitEarlyCondition`
- `private Func<Node, bool> _nodeFilter`
- `private NTargetingArrow _targetingArrow`
- `private TargetMode _targetMode`
- `private TargetType _validTargetsType`
- `private CreatureHoveredEventHandler backing_CreatureHovered`
- `private CreatureUnhoveredEventHandler backing_CreatureUnhovered`
- `private NodeHoveredEventHandler backing_NodeHovered`
- `private NodeUnhoveredEventHandler backing_NodeUnhovered`
- `private TargetingBeganEventHandler backing_TargetingBegan`
- `private TargetingEndedEventHandler backing_TargetingEnded`

**Methods**

- `public virtual void _EnterTree()`
- `public virtual void _ExitTree()`
- `public virtual void _Input(InputEvent inputEvent)`
- `public virtual void _Process(double delta)`
- `public virtual void _Ready()`
- `internal bool AllowedToTargetCreature(Creature creature)`
- `public bool AllowedToTargetNode(Node node)`
- `public void CancelTargeting()`
- `protected void EmitSignalCreatureHovered(NCreature creature)`
- `protected void EmitSignalCreatureUnhovered(NCreature creature)`
- `protected void EmitSignalNodeHovered(Node node)`
- `protected void EmitSignalNodeUnhovered(Node node)`
- `protected void EmitSignalTargetingBegan()`
- `protected void EmitSignalTargetingEnded()`
- `internal void FinishTargeting(bool cancel)`
- `protected virtual bool GetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `protected virtual bool HasGodotClassMethod(godot_string_name& method)`
- `protected virtual bool HasGodotClassSignal(godot_string_name& signal)`
- `protected virtual bool InvokeGodotClassMethod(godot_string_name& method, NativeVariantPtrArgs args, godot_variant& ret)`
- `internal void OnCombatEnded(CombatRoom _)`
- `internal void OnCreatureHovered(NCreature creature)`
- `internal void OnCreatureUnhovered(NCreature creature)`
- `public void OnNodeHovered(Node node)`
- `public void OnNodeUnhovered(Node node)`
- `protected virtual void RaiseGodotClassSignalCallbacks(godot_string_name& signal, NativeVariantPtrArgs args)`
- `protected virtual void RestoreGodotObjectData(GodotSerializationInfo info)`
- `protected virtual void SaveGodotObjectData(GodotSerializationInfo info)`
- `public Task<Node> SelectionFinished()`
- `protected virtual bool SetGodotClassPropertyValue(godot_string_name& name, godot_variant& value)`
- `public void StartTargeting(TargetType validTargetsType, Vector2 startPosition, TargetMode startingMode, Func<bool> exitEarlyCondition, Func<Node, bool> nodeFilter)`
- `public void StartTargeting(TargetType validTargetsType, Control control, TargetMode startingMode, Func<bool> exitEarlyCondition, Func<Node, bool> nodeFilter)`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class PropertyName : PropertyName`

### `class QueueItem`

**Fields**

- `public GameAction action`
- `public NCard card`
- `public Tween currentTween`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `class SignalName : SignalName`

### `enum State`

Values: `Enabled`, `Disabled`, `Hidden`

### `enum State`

Values: `Enabled`, `Disabled`, `Hidden`

### `sealed class TargetingBeganEventHandler : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke()`

### `sealed class TargetingEndedEventHandler : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke()`

### `enum TargetMode`

Values: `None`, `ReleaseMouseToTarget`, `ClickMouseToTarget`, `Controller`

### `sealed class ToggledEventHandler : MulticastDelegate`

**Methods**

- `public virtual IAsyncResult BeginInvoke(NPeekButton peekButton, AsyncCallback callback, object object)`
- `public virtual void EndInvoke(IAsyncResult result)`
- `public virtual void Invoke(NPeekButton peekButton)`

## MegaCrit.Sts2.Core.Runs

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c`

### `sealed class <>c__DisplayClass102_0`

**Fields**

- `public UInt64 netId`

### `sealed class <>c__DisplayClass103_0`

**Fields**

- `public UInt64 netId`

### `sealed class <>c__DisplayClass121_0`

**Fields**

- `public ActMap map`

### `sealed class <>c__DisplayClass16_0`

**Fields**

- `public RelicModel relic`

### `sealed class <>c__DisplayClass170_0`

**Fields**

- `public UInt64 localPlayerId`

### `sealed class <>c__DisplayClass175_0`

**Fields**

- `public ActModel act`

### `sealed class <>c__DisplayClass35_0`

**Fields**

- `public Player player`

### `sealed class <>c__DisplayClass40_0`

**Fields**

- `public CardModel first`

### `sealed class <>c__DisplayClass41_0`

**Fields**

- `public CardModel first`

### `sealed class <>c__DisplayClass41_1`

**Fields**

- `public CardModel first`

### `sealed class <>c__DisplayClass5_0`

**Fields**

- `public IRunState runState`

### `sealed class <>O`

### `sealed class <AbandonInternal>d__204 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EnterAct>d__196 : ValueType, IAsyncStateMachine`

**Fields**

- `public int currentActIndex`
- `public bool doTransition`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EnterMapCoordDebug>d__187 : ValueType, IAsyncStateMachine`

**Fields**

- `public MapCoord coord`
- `public AbstractModel model`
- `public MapPointType pointType`
- `public RoomType roomType`
- `public bool showTransition`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EnterMapPointInternal>d__181 : ValueType, IAsyncStateMachine`

**Fields**

- `public int actFloor`
- `public MapPointType pointType`
- `public AbstractRoom preFinishedRoom`
- `public bool saveGame`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EnterNextAct>d__194 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EnterRoom>d__192 : ValueType, IAsyncStateMachine`

**Fields**

- `public AbstractRoom room`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EnterRoomDebug>d__188 : ValueType, IAsyncStateMachine`

**Fields**

- `public AbstractModel model`
- `public MapPointType pointType`
- `public RoomType roomType`
- `public bool showTransition`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EnterRoomInternal>d__191 : ValueType, IAsyncStateMachine`

**Fields**

- `public bool isRestoringRoomStackBase`
- `public AbstractRoom room`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <EnterRoomWithoutExitingCurrentRoom>d__193 : ValueType, IAsyncStateMachine`

**Fields**

- `public bool fadeToBlack`
- `public AbstractRoom room`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <ExitCurrentRoom>d__190 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <ExitCurrentRooms>d__189 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <FadeIn>d__185 : ValueType, IAsyncStateMachine`

**Fields**

- `public bool showTransition`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <FinalizeStartingRelics>d__174 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <GenerateMap>d__177 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <GuaranteeKillAllPlayers>d__205 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <IterateHookListeners>d__118 : IEnumerable<AbstractModel>, IEnumerable, IEnumerator<AbstractModel>, IEnumerator, IDisposable`

**Properties**

- `internal AbstractModel System.Collections.Generic.IEnumerator<MegaCrit.Sts2.Core.Models.AbstractModel>.Current` { get }
- `internal object System.Collections.IEnumerator.Current` { get }

**Fields**

- `private ICombatState childCombatState`

**Methods**

- `internal bool MoveNext()`
- `internal IEnumerator<AbstractModel> System.Collections.Generic.IEnumerable<MegaCrit.Sts2.Core.Models.AbstractModel>.GetEnumerator()`
- `internal IEnumerator System.Collections.IEnumerable.GetEnumerator()`
- `internal void System.Collections.IEnumerator.Reset()`
- `internal void System.IDisposable.Dispose()`

### `sealed class <LoadIntoLatestMapCoord>d__179 : ValueType, IAsyncStateMachine`

**Fields**

- `public AbstractRoom preFinishedRoom`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <ProceedFromTerminalRewardsScreen>d__199 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <ResumePreviousRoom>d__200 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <ReturnToMainMenuWithError>d__217 : ValueType, IAsyncStateMachine`

**Fields**

- `public NetErrorInfo info`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SetActInternal>d__197 : ValueType, IAsyncStateMachine`

**Fields**

- `public int actIndex`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SetUpSavedMultiPlayer>d__161 : ValueType, IAsyncStateMachine`

**Fields**

- `public LoadRunLobby lobby`
- `public RunState state`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <SetUpSavedSinglePlayer>d__160 : ValueType, IAsyncStateMachine`

**Fields**

- `public SerializableRun save`
- `public RunState state`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `sealed class <WinRun>d__195 : ValueType, IAsyncStateMachine`

**Methods**

- `internal void MoveNext()`
- `internal void SetStateMachine(IAsyncStateMachine stateMachine)`

### `enum CardCreationFlags`

Values: `NoRarityModification`, `NoUpgradeRoll`, `NoHookUpgrades`, `NoModifyHooks`, `NoCardPoolModifications`, `NoCardModelModifications`, `ForceRarityOddsChange`, `IsCardReward`, `NoUpgrades`, `NoModifications`

### `class CardCreationOptions : IEquatable<CardCreationOptions>`

**Properties**

- `public Func<CardModel, bool> CardPoolFilter` { get; set }
- `public IReadOnlyCollection<CardPoolModel> CardPools` { get }
- `public IEnumerable<CardModel> CustomCardPool` { get; set }
- `internal Type EqualityContract` { get }
- `public CardCreationFlags Flags` { get; set }
- `public CardRarityOddsType RarityOdds` { get; set }
- `public Rng RngOverride` { get; set }
- `public CardCreationSource Source` { get; set }

**Fields**

- `private List<CardPoolModel> _cardPools`

**Methods**

- `internal void AssertUniformOddsIfSingleRarityPool()`
- `public virtual bool Equals(object obj)`
- `public virtual bool Equals(CardCreationOptions other)`
- `public virtual int GetHashCode()`
- `public IEnumerable<CardModel> GetPossibleCards(Player player)`
- `protected virtual bool PrintMembers(StringBuilder builder)`
- `public virtual string ToString()`
- `public Nullable<CardRarity> TryGetSingleRarityInPool()`
- `public CardCreationOptions WithCardPools(IEnumerable<CardPoolModel> pools, Func<CardModel, bool> cardPoolFilter)`
- `public CardCreationOptions WithCustomPool(IEnumerable<CardModel> customPool, Nullable<CardRarityOddsType> rarityOdds)`
- `public CardCreationOptions WithFlags(CardCreationFlags flag)`
- `public CardCreationOptions WithRngOverride(Rng rng)`

### `enum CardCreationSource`

Values: `None`, `Encounter`, `Shop`, `Other`

### `enum CardRarityOddsType`

Values: `None`, `RegularEncounter`, `EliteEncounter`, `BossEncounter`, `Shop`, `Uniform`

### `class ExtraRunFields`

**Properties**

- `public bool FreedRepy` { get; set }
- `public bool StartedWithNeow` { get; set }
- `public int TestSubjectKills` { get; set }

**Methods**

- `public SerializableExtraRunFields ToSerializable()`

### `enum GameMode`

Values: `None`, `Standard`, `Daily`, `Custom`

### `sealed class GameModeExtension`

### `interface ICardScope`

**Methods**

- `public virtual void AddCard(CardModel mutableCard, Player owner)`
- `public virtual CardModel CloneCard(CardModel mutableCard)`
- `public virtual T CreateCard(Player owner)`
- `public virtual CardModel CreateCard(CardModel canonicalCard, Player owner)`
- `public virtual void RemoveCard(CardModel card)`

### `interface IPlayerCollection`

**Properties**

- `public IReadOnlyList<Player> Players` { get }

**Methods**

- `public virtual Player GetPlayer(UInt64 netId)`
- `public virtual int GetPlayerSlotIndex(Player player)`

### `interface IRunState : ICardScope, IPlayerCollection`

**Properties**

- `public ActModel Act` { get }
- `public int ActFloor` { get; set }
- `public IReadOnlyList<ActModel> Acts` { get }
- `public int AscensionLevel` { get }
- `public IReadOnlyList<BadgeModel> BadgeModels` { get }
- `public AbstractRoom BaseRoom` { get }
- `public CardMultiplayerConstraint CardMultiplayerConstraint` { get }
- `public int CurrentActIndex` { get; set }
- `public Nullable<MapCoord> CurrentMapCoord` { get }
- `public MapPoint CurrentMapPoint` { get }
- `public MapPointHistoryEntry CurrentMapPointHistoryEntry` { get }
- `public AbstractRoom CurrentRoom` { get }
- `public int CurrentRoomCount` { get }
- `public ExtraRunFields ExtraFields` { get }
- `public GameMode GameMode` { get }
- `public bool IsGameOver` { get }
- `public ActMap Map` { get; set }
- `public MapLocation MapLocation` { get }
- `public IReadOnlyList<IReadOnlyList<MapPointHistoryEntry>> MapPointHistory` { get }
- `public IReadOnlyList<ModifierModel> Modifiers` { get }
- `public MultiplayerScalingModel MultiplayerScalingModel` { get }
- `public RunOddsSet Odds` { get }
- `public RunRngSet Rng` { get }
- `public RunLocation RunLocation` { get }
- `public RelicGrabBag SharedRelicGrabBag` { get }
- `public int TotalFloor` { get }
- `public UnlockState UnlockState` { get }

**Methods**

- `public virtual void AppendToMapPointHistory(MapPointType mapPointType, RoomType initialRoomType, ModelId modelId)`
- `public virtual bool ContainsCard(CardModel card)`
- `public virtual int GetAndIncrementNextRoomId()`
- `public virtual MapPointHistoryEntry GetHistoryEntryFor(MapLocation location)`
- `public virtual IEnumerable<AbstractModel> IterateHookListeners(ICombatState childCombatState)`
- `public virtual CardModel LoadCard(SerializableCard serializableCard, Player owner)`

### `sealed class MapLocation : ValueType, IEquatable<MapLocation>, IComparable<MapLocation>, IPacketSerializable`

**Fields**

- `public int actIndex`
- `public Nullable<MapCoord> coord`

**Methods**

- `public int CompareTo(MapLocation other)`
- `public void Deserialize(PacketReader reader)`
- `public bool Equals(MapLocation other)`
- `public virtual bool Equals(object obj)`
- `public virtual int GetHashCode()`
- `public void Serialize(PacketWriter writer)`
- `public virtual string ToString()`

### `class NullRunState : IRunState, ICardScope, IPlayerCollection`

**Properties**

- `public ActModel Act` { get }
- `public int ActFloor` { get; set }
- `public IReadOnlyList<ActModel> Acts` { get }
- `public int AscensionLevel` { get }
- `public IReadOnlyList<BadgeModel> BadgeModels` { get }
- `public AbstractRoom BaseRoom` { get }
- `public int CurrentActIndex` { get; set }
- `public Nullable<MapCoord> CurrentMapCoord` { get }
- `public MapPoint CurrentMapPoint` { get }
- `public MapPointHistoryEntry CurrentMapPointHistoryEntry` { get }
- `public AbstractRoom CurrentRoom` { get }
- `public int CurrentRoomCount` { get }
- `public ExtraRunFields ExtraFields` { get }
- `public GameMode GameMode` { get }
- `public bool IsGameOver` { get }
- `public ActMap Map` { get; set }
- `public MapLocation MapLocation` { get }
- `public IReadOnlyList<IReadOnlyList<MapPointHistoryEntry>> MapPointHistory` { get }
- `public IReadOnlyList<ModifierModel> Modifiers` { get }
- `public MultiplayerScalingModel MultiplayerScalingModel` { get }
- `public RunOddsSet Odds` { get }
- `public IReadOnlyList<Player> Players` { get }
- `public RunRngSet Rng` { get }
- `public RunLocation RunLocation` { get }
- `public RelicGrabBag SharedRelicGrabBag` { get }
- `public int TotalFloor` { get }
- `public UnlockState UnlockState` { get }

**Methods**

- `public void AddCard(CardModel mutableCard, Player owner)`
- `public void AppendToMapPointHistory(MapPointType mapPointType, RoomType initialRoomType, ModelId modelId)`
- `public CardModel CloneCard(CardModel mutableCard)`
- `public bool ContainsCard(CardModel card)`
- `public T CreateCard(Player owner)`
- `public CardModel CreateCard(CardModel canonicalCard, Player owner)`
- `public int GetAndIncrementNextRoomId()`
- `public MapPointHistoryEntry GetHistoryEntryFor(MapLocation location)`
- `public Player GetPlayer(UInt64 netId)`
- `public int GetPlayerSlotIndex(Player player)`
- `public IEnumerable<AbstractModel> IterateHookListeners(ICombatState childCombatState)`
- `public CardModel LoadCard(SerializableCard serializableCard, Player owner)`
- `public void RemoveCard(CardModel card)`

### `class PlayerMapPointHistoryEntry : IPacketSerializable`

**Properties**

- `public List<AncientChoiceHistoryEntry> AncientChoices` { get; set }
- `public List<ModelId> BoughtColorless` { get; set }
- `public List<ModelId> BoughtPotions` { get; set }
- `public List<ModelId> BoughtRelics` { get; set }
- `public List<CardChoiceHistoryEntry> CardChoices` { get; set }
- `public List<CardEnchantmentHistoryEntry> CardsEnchanted` { get; set }
- `public List<SerializableCard> CardsGained` { get; set }
- `public List<SerializableCard> CardsRemoved` { get; set }
- `public List<CardTransformationHistoryEntry> CardsTransformed` { get; set }
- `public List<ModelId> CompletedQuests` { get; set }
- `public int CurrentGold` { get; set }
- `public int CurrentHp` { get; set }
- `public int DamageTaken` { get; set }
- `public List<ModelId> DowngradedCards` { get; set }
- `public List<EventOptionHistoryEntry> EventChoices` { get; set }
- `public int GoldGained` { get; set }
- `public int GoldLost` { get; set }
- `public int GoldSpent` { get; set }
- `public int GoldStolen` { get; set }
- `public int HpHealed` { get; set }
- `public int MaxHp` { get; set }
- `public int MaxHpGained` { get; set }
- `public int MaxHpLost` { get; set }
- `public UInt64 PlayerId` { get; set }
- `public List<ModelChoiceHistoryEntry> PotionChoices` { get; set }
- `public List<ModelId> PotionDiscarded` { get; set }
- `public List<ModelId> PotionUsed` { get; set }
- `public List<ModelChoiceHistoryEntry> RelicChoices` { get; set }
- `public List<ModelId> RelicsRemoved` { get; set }
- `public List<string> RestSiteChoices` { get; set }
- `public List<ModelId> UpgradedCards` { get; set }

**Methods**

- `public PlayerMapPointHistoryEntry Anonymized()`
- `public void Deserialize(PacketReader reader)`
- `public LocString GetAncientPickedChoiceLoc()`
- `public List<LocString> GetAncientSkippedChoiceLoc()`
- `public void Serialize(PacketWriter writer)`

### `class RelicGrabBag`

**Properties**

- `public bool IsPopulated` { get }

**Fields**

- `private Dictionary<RelicRarity, List<RelicModel>> _deques`
- `private List<RelicModel> _mpFallbackDequeue`
- `private List<RelicModel> _originalRelics`
- `private bool _refreshAllowed`

**Methods**

- `public bool Contains(RelicModel relic)`
- `internal bool DequeHasAnyRelics(List<RelicModel> deque, Func<RelicModel, bool> filter)`
- `internal List<RelicModel> GetAvailableDeque(RelicRarity rarity, IRunState runState, Func<RelicModel, bool> filter)`
- `internal List<RelicModel> GetDeque(RelicRarity rarity)`
- `public bool HasAvailableRelics(IRunState runState)`
- `public void LoadFromSerializable(SerializableRelicGrabBag save)`
- `public void MoveToFallback(RelicModel toRemove)`
- `public void Populate(Player player, Rng rng)`
- `public void Populate(IEnumerable<RelicModel> relics, Rng rng)`
- `public RelicModel PullFromBack(RelicRarity rarity, Func<RelicModel, bool> filter, IRunState runState)`
- `public RelicModel PullFromFront(RelicRarity rarity, IRunState runState)`
- `public RelicModel PullFromFront(RelicRarity rarity, Func<RelicModel, bool> filter, IRunState runState)`
- `internal void RefreshRarity(RelicRarity rarity)`
- `public void Remove()`
- `public void Remove(RelicModel relic)`
- `internal void RemoveDisallowedRelicsFromDeques(IRunState runState)`
- `public SerializableRelicGrabBag ToSerializable()`

### `class RunHistory : ISaveSchema`

**Properties**

- `public List<ModelId> Acts` { get; set }
- `public int Ascension` { get; set }
- `public string BuildId` { get; set }
- `public GameMode GameMode` { get; set }
- `public ModelId KilledByEncounter` { get; set }
- `public ModelId KilledByEvent` { get; set }
- `public List<List<MapPointHistoryEntry>> MapPointHistory` { get; set }
- `public List<SerializableModifier> Modifiers` { get; set }
- `public PlatformType PlatformType` { get; set }
- `public List<RunHistoryPlayer> Players` { get; set }
- `public float RunTime` { get; set }
- `public int SchemaVersion` { get; set }
- `public string Seed` { get; set }
- `public long StartTime` { get; set }
- `public bool WasAbandoned` { get; set }
- `public bool Win` { get; set }

### `class RunHistoryPlayer`

**Properties**

- `public IEnumerable<SerializableBadge> Badges` { get; set }
- `public ModelId Character` { get; set }
- `public IEnumerable<SerializableCard> Deck` { get; set }
- `public UInt64 Id` { get; set }
- `public int MaxPotionSlotCount` { get; set }
- `public IEnumerable<SerializablePotion> Potions` { get; set }
- `public IEnumerable<SerializableRelic> Relics` { get; set }

### `sealed class RunHistoryUtilities`

### `sealed class RunLocation : ValueType, IEquatable<RunLocation>, IComparable<RunLocation>, IPacketSerializable`

**Fields**

- `public MapLocation mapLocation`
- `public Nullable<int> roomId`

**Methods**

- `public int CompareTo(RunLocation other)`
- `public void Deserialize(PacketReader reader)`
- `public bool Equals(RunLocation other)`
- `public virtual bool Equals(object obj)`
- `public virtual int GetHashCode()`
- `public void Serialize(PacketWriter writer)`
- `public virtual string ToString()`

### `class RunManager : IRunLobbyListener`

**Properties**

- `public ActChangeSynchronizer ActChangeSynchronizer` { get; set }
- `public ActionExecutor ActionExecutor` { get; set }
- `public ActionQueueSet ActionQueueSet` { get; set }
- `public ActionQueueSynchronizer ActionQueueSynchronizer` { get; set }
- `public AscensionManager AscensionManager` { get; set }
- `public ChecksumTracker ChecksumTracker` { get; set }
- `public CombatReplayWriter CombatReplayWriter` { get; set }
- `public CombatStateSynchronizer CombatStateSynchronizer` { get; set }
- `public Nullable<DateTimeOffset> DailyTime` { get; set }
- `public EventSynchronizer EventSynchronizer` { get; set }
- `public FlavorSynchronizer FlavorSynchronizer` { get; set }
- `public bool ForceDiscoveryOrderModifications` { get; set }
- `public RunHistory History` { get; set }
- `public HoveredModelTracker HoveredModelTracker` { get; set }
- `public PeerInputSynchronizer InputSynchronizer` { get; set }
- `public bool IsAbandoned` { get; set }
- `public bool IsCleaningUp` { get; set }
- `public bool IsGameOver` { get }
- `public bool IsInProgress` { get }
- `public bool IsSinglePlayerOrFakeMultiplayer` { get }
- `public SerializableMapDrawings MapDrawingsToLoad` { get; set }
- `public MapSelectionSynchronizer MapSelectionSynchronizer` { get; set }
- `public INetGameService NetService` { get; set }
- `public OneOffSynchronizer OneOffSynchronizer` { get; set }
- `public PlayerChoiceSynchronizer PlayerChoiceSynchronizer` { get; set }
- `public RestSiteSynchronizer RestSiteSynchronizer` { get; set }
- `public RewardsSetSynchronizer RewardsSetSynchronizer` { get; set }
- `public RewardSynchronizer RewardSynchronizer` { get; set }
- `public RunLobby RunLobby` { get; set }
- `public RunLocationTargetedMessageBuffer RunLocationTargetedBuffer` { get; set }
- `public long RunTime` { get }
- `public Dictionary<int, SerializableActMap> SavedMapsToLoad` { get; set }
- `public bool ShouldSave` { get; set }
- `internal RunState State` { get; set }
- `public TreasureRoomRelicSynchronizer TreasureRoomRelicSynchronizer` { get; set }
- `public long WinTime` { get; set }

**Fields**

- `private int _numReloads`
- `private long _prevRunTime`
- `private bool _runHistoryWasUploaded`
- `private long _sessionStartTime`
- `private long _startTime`
- `private Action ActEntered`
- `public Action debugAfterCombatRewardsOverride`
- `private Action RoomEntered`
- `private Action RoomExited`
- `private Action<RunState> RunStarted`

**Methods**

- `public void Abandon()`
- `internal Task AbandonInternal()`
- `internal void AfterMapLocationChanged()`
- `public void ApplyAscensionEffects(Player player)`
- `public void CleanUp(bool graceful)`
- `internal void ClearScreens()`
- `internal AbstractRoom CreateRoom(RoomType roomType, MapPointType mapPointType, AbstractModel model)`
- `public RunState DebugOnlyGetState()`
- `public Task EnterAct(int currentActIndex, bool doTransition)`
- `public Task EnterMapCoord(MapCoord coord)`
- `public Task EnterMapCoordDebug(MapCoord coord, RoomType roomType, MapPointType pointType, AbstractModel model, bool showTransition)`
- `internal Task EnterMapCoordInternal(MapCoord coord, AbstractRoom preFinishedRoom, bool saveGame)`
- `public Task EnterMapPointInternal(int actFloor, MapPointType pointType, AbstractRoom preFinishedRoom, bool saveGame)`
- `public Task EnterNextAct()`
- `public Task EnterRoom(AbstractRoom room)`
- `public Task<AbstractRoom> EnterRoomDebug(RoomType roomType, MapPointType pointType, AbstractModel model, bool showTransition)`
- `internal Task EnterRoomInternal(AbstractRoom room, bool isRestoringRoomStackBase)`
- `public Task EnterRoomWithoutExitingCurrentRoom(AbstractRoom room, bool fadeToBlack)`
- `internal Task<AbstractRoom> ExitCurrentRoom()`
- `internal Task ExitCurrentRooms()`
- `internal Task FadeIn(bool showTransition)`
- `public Task FinalizeStartingRelics()`
- `public Task GenerateMap()`
- `public void GenerateRooms()`
- `public string GetLocalCharacterEnergyIconPrefix()`
- `public ClientRejoinResponseMessage GetRejoinMessage()`
- `internal Task GuaranteeKillAllPlayers()`
- `public bool HasAscension(AscensionLevel level)`
- `internal void InitializeNewRun()`
- `internal void InitializeRunLobby(INetGameService netService, RunState state)`
- `internal void InitializeSavedRun(SerializableRun save)`
- `internal void InitializeShared(INetGameService netService, PeerInputSynchronizer inputSynchronizer, bool shouldSave, Nullable<DateTimeOffset> dailyTime, long startTime, long runTime, long winTime, int numReloads)`
- `public RunState Launch()`
- `public Task LoadIntoLatestMapCoord(AbstractRoom preFinishedRoom)`
- `public void LocalPlayerDisconnected(NetErrorInfo info)`
- `internal void MegaCrit.Sts2.Core.Multiplayer.Game.Lobby.IRunLobbyListener.RunAbandoned()`
- `public SerializableRun OnEnded(bool isVictory)`
- `public Task ProceedFromTerminalRewardsScreen()`
- `internal void RemotePlayerDisconnected(UInt64 playerId)`
- `internal Task ResumePreviousRoom()`
- `internal Task ReturnToMainMenuWithError(NetErrorInfo info)`
- `internal RoomType RollRoomTypeFor(MapPointType pointType, IEnumerable<RoomType> blacklist)`
- `internal void SendPostActionChecksum(GameAction action)`
- `public Task SetActInternal(int actIndex)`
- `internal void SetStartedWithNeowFlag()`
- `public void SetUpNewMultiPlayer(RunState state, StartRunLobby lobby, bool shouldSave, Nullable<DateTimeOffset> dailyTime)`
- `public void SetUpNewSinglePlayer(RunState state, bool shouldSave, Nullable<DateTimeOffset> dailyTime)`
- `public void SetUpReplay(RunState state, CombatReplay replay)`
- `public Task SetUpSavedMultiPlayer(RunState state, LoadRunLobby lobby)`
- `public Task SetUpSavedSinglePlayer(RunState state, SerializableRun save)`
- `public void SetUpTest(RunState state, INetGameService gameService, bool disableCombatStateSync, bool shouldSave)`
- `public bool ShouldApplyTutorialModifications()`
- `internal void StateDiverged(NetFullCombatState state)`
- `public SerializableRun ToSave(AbstractRoom preFinishedRoom)`
- `internal bool TryGetRoomTypeForTutorial(MapPointType pointType, RoomType& roomType)`
- `internal void UpdatePlayerStatsInMapPointHistory()`
- `internal void UpdateRichPresence()`
- `internal Task WinRun()`
- `public void WriteReplay(bool stopRecording)`

### `class RunRngSet`

**Properties**

- `public Rng CombatCardGeneration` { get }
- `public Rng CombatCardSelection` { get }
- `public Rng CombatEnergyCosts` { get }
- `public Rng CombatOrbGeneration` { get }
- `public Rng CombatPotionGeneration` { get }
- `public Rng CombatTargets` { get }
- `public Rng MonsterAi` { get }
- `public Rng Niche` { get }
- `public UInt32 Seed` { get }
- `public Rng Shuffle` { get }
- `public string StringSeed` { get }
- `public Rng TreasureRoomRelics` { get }
- `public Rng UnknownMapPoint` { get }
- `public Rng UpFront` { get }

**Fields**

- `private Dictionary<RunRngType, Rng> _rngs`

**Methods**

- `internal Rng CreateRng(RunRngType rngType)`
- `internal Rng GetRng(RunRngType rngType)`
- `public void LoadFromSerializable(SerializableRunRngSet save)`
- `public void MockRng(RunRngType rngType, UInt32 seed)`
- `public SerializableRunRngSet ToSerializable()`

### `class RunState : IRunState, ICardScope, IPlayerCollection`

**Properties**

- `public ActModel Act` { get }
- `public int ActFloor` { get; set }
- `public IReadOnlyList<ActModel> Acts` { get; set }
- `public int AscensionLevel` { get; set }
- `public IReadOnlyList<BadgeModel> BadgeModels` { get; set }
- `public AbstractRoom BaseRoom` { get }
- `public int CurrentActIndex` { get; set }
- `public Nullable<MapCoord> CurrentMapCoord` { get }
- `public MapPoint CurrentMapPoint` { get }
- `public MapPointHistoryEntry CurrentMapPointHistoryEntry` { get }
- `public AbstractRoom CurrentRoom` { get }
- `public int CurrentRoomCount` { get }
- `public ExtraRunFields ExtraFields` { get; set }
- `public GameMode GameMode` { get; set }
- `public bool IsGameOver` { get }
- `public ActMap Map` { get; set }
- `public MapLocation MapLocation` { get }
- `public IReadOnlyList<IReadOnlyList<MapPointHistoryEntry>> MapPointHistory` { get }
- `public IReadOnlyList<ModifierModel> Modifiers` { get; set }
- `public MultiplayerScalingModel MultiplayerScalingModel` { get; set }
- `public int NextRoomId` { get; set }
- `public RunOddsSet Odds` { get; set }
- `public IReadOnlyList<Player> Players` { get }
- `public RunRngSet Rng` { get; set }
- `public RunLocation RunLocation` { get }
- `public RelicGrabBag SharedRelicGrabBag` { get; set }
- `public int TotalFloor` { get }
- `public UnlockState UnlockState` { get; set }
- `public IReadOnlySet<ModelId> VisitedEventIds` { get }
- `public IReadOnlyList<MapCoord> VisitedMapCoords` { get }

**Fields**

- `private List<CardModel> _allCards`
- `private int _currentActIndex`
- `private List<AbstractRoom> _currentRooms`
- `private List<List<MapPointHistoryEntry>> _mapPointHistory`
- `private List<Player> _players`
- `private HashSet<ModelId> _visitedEventIds`
- `private List<MapCoord> _visitedMapCoords`

**Methods**

- `public void AddCard(CardModel card, Player owner)`
- `internal void AddCard(CardModel card)`
- `public void AddModifierDebug(ModifierModel modifier)`
- `public void AddPlayerDebug(Player player, int index)`
- `public void AddVisitedEvent(EventModel eventModel)`
- `public bool AddVisitedMapCoord(MapCoord coord)`
- `public void AppendToMapPointHistory(MapPointType mapPointType, RoomType initialRoomType, ModelId roomModelId)`
- `public void ClearVisitedMapCoordsDebug()`
- `public CardModel CloneCard(CardModel mutableCard)`
- `public bool ContainsCard(CardModel card)`
- `public T CreateCard(Player owner)`
- `public CardModel CreateCard(CardModel canonicalCard, Player owner)`
- `public int GetAndIncrementNextRoomId()`
- `public MapPointHistoryEntry GetHistoryEntryFor(MapLocation location)`
- `public Player GetPlayer(UInt64 netId)`
- `public int GetPlayerSlotIndex(Player player)`
- `public int GetPlayerSlotIndex(UInt64 netId)`
- `public IEnumerable<AbstractModel> IterateHookListeners(ICombatState childCombatState)`
- `public CardModel LoadCard(SerializableCard serializableCard, Player owner)`
- `public AbstractRoom PopCurrentRoom()`
- `public void PushRoom(AbstractRoom room)`
- `public void RemoveCard(CardModel card)`
- `public void RemoveStaleVisitedMapCoords(ActMap map)`
- `public void SetActDebug(ActModel act)`

### `sealed class ScoreUtility`
