using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.addons.mega_text;
using STS2Plus.Features;
using STS2Plus.Localization;

namespace STS2Plus.Ui;

[ScriptPath("res://src/Ui/BuildCreatorOverlay.cs")]
internal sealed class BuildCreatorOverlay : Control
{
	private enum TabMode
	{
		Cards,
		Relics
	}

	private const int CardsPerRow = 4;

	private static readonly Color BackdropColor = new Color(0f, 0f, 0f, 0.74f);

	private static readonly Color PanelColor = new Color(0.09f, 0.07f, 0.06f, 0.98f);

	private static readonly Color PanelBorderColor = new Color(0.83f, 0.59f, 0.24f, 1f);

	private static readonly Color SelectedRowColor = new Color(0.22f, 0.16f, 0.1f, 1f);

	private static readonly Color RowColor = new Color(0.13f, 0.1f, 0.09f, 0.96f);

	private static readonly Vector2 CardPreviewScale = Vector2.One * 0.62f;

	private static readonly Vector2 CardPreviewVisualOffset = new Vector2(26f, 18f);

	private static readonly Vector2 CardPreviewVisualSize = NCard.defaultSize * CardPreviewScale + CardPreviewVisualOffset;

	private static readonly Vector2 CardPreviewHostOffset = new Vector2(90f, 120f);

	private readonly TaskCompletionSource<bool> completionSource = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

	private readonly Dictionary<string, BuildCreatorCardSelection> selectedCardSelections = new Dictionary<string, BuildCreatorCardSelection>(StringComparer.Ordinal);

	private readonly HashSet<string> selectedRelicEntries = new HashSet<string>(StringComparer.Ordinal);

	private readonly Dictionary<string, CardModel> cardCatalog = new Dictionary<string, CardModel>(StringComparer.Ordinal);

	private readonly Dictionary<string, RelicModel> relicCatalog = new Dictionary<string, RelicModel>(StringComparer.Ordinal);

	private Player? player;

	private TabMode currentTab;

	private LineEdit? searchInput;

	private VBoxContainer? itemListContainer;

	private VBoxContainer? selectedCardsContainer;

	private VBoxContainer? selectedRelicsContainer;

	private Label? statusLabel;

	private Label? headerLabel;

	private Button? cardsTabButton;

	private Button? relicsTabButton;

	private Button? applyButton;

	public static Task OpenAsync(EventModel eventModel)
	{
		if (eventModel.Owner == null || !BuildCreatorRuntime.IsSupportedRun(eventModel.Owner))
		{
			return Task.CompletedTask;
		}
		NRun instance = NRun.Instance;
		if (instance == null)
		{
			return Task.CompletedTask;
		}
		BuildCreatorOverlay buildCreatorOverlay = new BuildCreatorOverlay();
		((Node)instance).AddChild((Node)(object)buildCreatorOverlay, false, (Node.InternalMode)0);
		((Node)instance).MoveChild((Node)(object)buildCreatorOverlay, -1);
		buildCreatorOverlay.Initialize(eventModel.Owner);
		return buildCreatorOverlay.completionSource.Task;
	}

	public override void _Ready()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Expected O, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Expected O, but got Unknown
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Expected O, but got Unknown
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Expected O, but got Unknown
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Expected O, but got Unknown
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Expected O, but got Unknown
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Expected O, but got Unknown
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Expected O, but got Unknown
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Expected O, but got Unknown
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Expected O, but got Unknown
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Expected O, but got Unknown
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0507: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Expected O, but got Unknown
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0579: Unknown result type (might be due to invalid IL or missing references)
		//IL_0584: Expected O, but got Unknown
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Expected O, but got Unknown
		//IL_05f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_060e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0619: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Expected O, but got Unknown
		//IL_064f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0654: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Expected O, but got Unknown
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a2: Expected O, but got Unknown
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Expected O, but got Unknown
		//IL_0734: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Expected O, but got Unknown
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0776: Expected O, but got Unknown
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07af: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Expected O, but got Unknown
		//IL_07e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_0814: Unknown result type (might be due to invalid IL or missing references)
		//IL_081f: Unknown result type (might be due to invalid IL or missing references)
		//IL_082f: Expected O, but got Unknown
		((Control)this).MouseFilter = (Control.MouseFilterEnum)0;
		((Control)this).FocusMode = (Control.FocusModeEnum)2;
		((CanvasItem)this).ZIndex = 500;
		((Control)this).SetAnchorsAndOffsetsPreset((LayoutPreset)15, (LayoutPresetMode)0, 0);
		ColorRect val = new ColorRect
		{
			Color = BackdropColor,
			MouseFilter = (Control.MouseFilterEnum)0
		};
		((Control)val).SetAnchorsAndOffsetsPreset((LayoutPreset)15, (LayoutPresetMode)0, 0);
		((Node)this).AddChild((Node)(object)val, false, (Node.InternalMode)0);
		MarginContainer val2 = new MarginContainer();
		((Control)val2).SetAnchorsAndOffsetsPreset((LayoutPreset)15, (LayoutPresetMode)0, 0);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_left", 72);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_top", 48);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_right", 72);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_bottom", 48);
		((Node)this).AddChild((Node)(object)val2, false, (Node.InternalMode)0);
		PanelContainer val3 = new PanelContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			SizeFlagsVertical = (Control.SizeFlags)3
		};
		((Control)val3).AddThemeStyleboxOverride((StringName)"panel", (StyleBox)(object)CreatePanelStyle(PanelColor, PanelBorderColor, 14));
		((Node)val2).AddChild((Node)(object)val3, false, (Node.InternalMode)0);
		MarginContainer val4 = new MarginContainer();
		((Control)val4).AddThemeConstantOverride((StringName)"margin_left", 20);
		((Control)val4).AddThemeConstantOverride((StringName)"margin_top", 20);
		((Control)val4).AddThemeConstantOverride((StringName)"margin_right", 20);
		((Control)val4).AddThemeConstantOverride((StringName)"margin_bottom", 18);
		((Node)val3).AddChild((Node)(object)val4, false, (Node.InternalMode)0);
		VBoxContainer val5 = new VBoxContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			SizeFlagsVertical = (Control.SizeFlags)3
		};
		((Control)val5).AddThemeConstantOverride((StringName)"separation", 14);
		((Node)val4).AddChild((Node)(object)val5, false, (Node.InternalMode)0);
		headerLabel = new Label
		{
			Text = PlusLoc.Text("BUILD_CREATOR_TITLE"),
			HorizontalAlignment = (HorizontalAlignment)0
		};
		((Control)headerLabel).AddThemeFontSizeOverride((StringName)"font_size", 28);
		((Control)headerLabel).AddThemeColorOverride((StringName)"font_color", new Color(0.97f, 0.88f, 0.68f, 1f));
		((Node)val5).AddChild((Node)(object)headerLabel, false, (Node.InternalMode)0);
		Label val6 = new Label
		{
			Text = PlusLoc.Text("BUILD_CREATOR_SUBTITLE"),
			HorizontalAlignment = (HorizontalAlignment)0,
			AutowrapMode = (TextServer.AutowrapMode)3
		};
		((Control)val6).AddThemeFontSizeOverride((StringName)"font_size", 16);
		((Control)val6).AddThemeColorOverride((StringName)"font_color", new Color(0.84f, 0.8f, 0.74f, 1f));
		((Node)val5).AddChild((Node)(object)val6, false, (Node.InternalMode)0);
		HBoxContainer val7 = new HBoxContainer();
		((Control)val7).AddThemeConstantOverride((StringName)"separation", 10);
		((Node)val5).AddChild((Node)(object)val7, false, (Node.InternalMode)0);
		cardsTabButton = CreateTabButton(PlusLoc.Text("BUILD_CREATOR_TAB_CARDS"));
		((BaseButton)cardsTabButton).Pressed += delegate
		{
			SwitchTab(TabMode.Cards);
		};
		((Node)val7).AddChild((Node)(object)cardsTabButton, false, (Node.InternalMode)0);
		relicsTabButton = CreateTabButton(PlusLoc.Text("BUILD_CREATOR_TAB_RELICS"));
		((BaseButton)relicsTabButton).Pressed += delegate
		{
			SwitchTab(TabMode.Relics);
		};
		((Node)val7).AddChild((Node)(object)relicsTabButton, false, (Node.InternalMode)0);
		searchInput = new LineEdit
		{
			PlaceholderText = PlusLoc.Text("BUILD_CREATOR_SEARCH_CARDS"),
			ClearButtonEnabled = true,
			SizeFlagsHorizontal = (Control.SizeFlags)3
		};
		searchInput.TextChanged += (LineEdit.TextChangedEventHandler)delegate
		{
			RefreshVisibleContent();
		};
		((Node)val5).AddChild((Node)(object)searchInput, false, (Node.InternalMode)0);
		HBoxContainer val8 = new HBoxContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			SizeFlagsVertical = (Control.SizeFlags)3
		};
		((Control)val8).AddThemeConstantOverride((StringName)"separation", 16);
		((Node)val5).AddChild((Node)(object)val8, false, (Node.InternalMode)0);
		PanelContainer val9 = new PanelContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			SizeFlagsVertical = (Control.SizeFlags)3
		};
		((Control)val9).AddThemeStyleboxOverride((StringName)"panel", (StyleBox)(object)CreatePanelStyle(RowColor, new Color(0.27f, 0.18f, 0.1f, 1f), 10));
		((Node)val8).AddChild((Node)(object)val9, false, (Node.InternalMode)0);
		ScrollContainer val10 = new ScrollContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			SizeFlagsVertical = (Control.SizeFlags)3,
			HorizontalScrollMode = (ScrollContainer.ScrollMode)0
		};
		((Node)val9).AddChild((Node)(object)val10, false, (Node.InternalMode)0);
		itemListContainer = new VBoxContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			SizeFlagsVertical = (Control.SizeFlags)3
		};
		((Control)itemListContainer).AddThemeConstantOverride((StringName)"separation", 18);
		((Node)val10).AddChild((Node)(object)itemListContainer, false, (Node.InternalMode)0);
		PanelContainer val11 = new PanelContainer
		{
			CustomMinimumSize = new Vector2(260f, 0f),
			SizeFlagsVertical = (Control.SizeFlags)3
		};
		((Control)val11).AddThemeStyleboxOverride((StringName)"panel", (StyleBox)(object)CreatePanelStyle(RowColor, new Color(0.27f, 0.18f, 0.1f, 1f), 10));
		((Node)val8).AddChild((Node)(object)val11, false, (Node.InternalMode)0);
		MarginContainer val12 = new MarginContainer();
		((Control)val12).AddThemeConstantOverride((StringName)"margin_left", 14);
		((Control)val12).AddThemeConstantOverride((StringName)"margin_top", 14);
		((Control)val12).AddThemeConstantOverride((StringName)"margin_right", 14);
		((Control)val12).AddThemeConstantOverride((StringName)"margin_bottom", 14);
		((Node)val11).AddChild((Node)(object)val12, false, (Node.InternalMode)0);
		VBoxContainer val13 = new VBoxContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			SizeFlagsVertical = (Control.SizeFlags)3
		};
		((Control)val13).AddThemeConstantOverride((StringName)"separation", 10);
		((Node)val12).AddChild((Node)(object)val13, false, (Node.InternalMode)0);
		Label val14 = new Label
		{
			Text = PlusLoc.Text("BUILD_CREATOR_SELECTED_CARDS")
		};
		((Control)val14).AddThemeFontSizeOverride((StringName)"font_size", 18);
		((Control)val14).AddThemeColorOverride((StringName)"font_color", new Color(0.95f, 0.87f, 0.67f, 1f));
		((Node)val13).AddChild((Node)(object)val14, false, (Node.InternalMode)0);
		ScrollContainer val15 = new ScrollContainer
		{
			CustomMinimumSize = new Vector2(0f, 220f),
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			SizeFlagsVertical = (Control.SizeFlags)3,
			HorizontalScrollMode = (ScrollContainer.ScrollMode)0
		};
		((Node)val13).AddChild((Node)(object)val15, false, (Node.InternalMode)0);
		selectedCardsContainer = new VBoxContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3
		};
		((Control)selectedCardsContainer).AddThemeConstantOverride((StringName)"separation", 6);
		((Node)val15).AddChild((Node)(object)selectedCardsContainer, false, (Node.InternalMode)0);
		Label val16 = new Label
		{
			Text = PlusLoc.Text("BUILD_CREATOR_SELECTED_RELICS")
		};
		((Control)val16).AddThemeFontSizeOverride((StringName)"font_size", 18);
		((Control)val16).AddThemeColorOverride((StringName)"font_color", new Color(0.95f, 0.87f, 0.67f, 1f));
		((Node)val13).AddChild((Node)(object)val16, false, (Node.InternalMode)0);
		ScrollContainer val17 = new ScrollContainer
		{
			CustomMinimumSize = new Vector2(0f, 180f),
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			SizeFlagsVertical = (Control.SizeFlags)3,
			HorizontalScrollMode = (ScrollContainer.ScrollMode)0
		};
		((Node)val13).AddChild((Node)(object)val17, false, (Node.InternalMode)0);
		selectedRelicsContainer = new VBoxContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3
		};
		((Control)selectedRelicsContainer).AddThemeConstantOverride((StringName)"separation", 6);
		((Node)val17).AddChild((Node)(object)selectedRelicsContainer, false, (Node.InternalMode)0);
		HBoxContainer val18 = new HBoxContainer();
		((Control)val18).AddThemeConstantOverride((StringName)"separation", 12);
		((Node)val5).AddChild((Node)(object)val18, false, (Node.InternalMode)0);
		statusLabel = new Label
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			VerticalAlignment = (VerticalAlignment)1,
			AutowrapMode = (TextServer.AutowrapMode)3
		};
		((Control)statusLabel).AddThemeColorOverride((StringName)"font_color", new Color(0.89f, 0.8f, 0.67f, 1f));
		((Node)val18).AddChild((Node)(object)statusLabel, false, (Node.InternalMode)0);
		applyButton = new Button
		{
			Text = PlusLoc.Text("BUILD_CREATOR_APPLY"),
			CustomMinimumSize = new Vector2(180f, 46f)
		};
		((BaseButton)applyButton).Pressed += async delegate
		{
			await ApplyAndCloseAsync();
		};
		((Node)val18).AddChild((Node)(object)applyButton, false, (Node.InternalMode)0);
		RefreshAll();
		LineEdit? obj = searchInput;
		if (obj != null)
		{
			((Control)obj).GrabFocus();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed((StringName)"ui_cancel", false, false))
		{
			((Node)this).GetViewport().SetInputAsHandled();
		}
	}

	private void Initialize(Player owner)
	{
		player = owner;
		cardCatalog.Clear();
		relicCatalog.Clear();
		selectedCardSelections.Clear();
		selectedRelicEntries.Clear();
		foreach (CardModel selectableCard in BuildCreatorRuntime.GetSelectableCards(owner))
		{
			cardCatalog[((AbstractModel)selectableCard).Id.Entry] = selectableCard;
		}
		foreach (RelicModel selectableRelic in BuildCreatorRuntime.GetSelectableRelics())
		{
			relicCatalog[((AbstractModel)selectableRelic).Id.Entry] = selectableRelic;
		}
		foreach (CardModel card in owner.Deck.Cards)
		{
			if (cardCatalog.ContainsKey(((AbstractModel)card).Id.Entry))
			{
				selectedCardSelections.TryGetValue(((AbstractModel)card).Id.Entry, out var value);
				value = (card.IsUpgraded ? new BuildCreatorCardSelection(value.BaseCount, value.UpgradedCount + 1) : new BuildCreatorCardSelection(value.BaseCount + 1, value.UpgradedCount));
				selectedCardSelections[((AbstractModel)card).Id.Entry] = value;
			}
		}
		foreach (RelicModel relic in owner.Relics)
		{
			if (relicCatalog.ContainsKey(((AbstractModel)relic).Id.Entry))
			{
				selectedRelicEntries.Add(((AbstractModel)relic).Id.Entry);
			}
		}
		if (((Node)this).IsNodeReady())
		{
			RefreshAll();
		}
	}

	private void RefreshAll()
	{
		RefreshHeader();
		RefreshTabButtons();
		RefreshVisibleContent();
		RefreshSummary();
		RefreshApplyState();
	}

	private void RefreshHeader()
	{
		if (headerLabel != null)
		{
			int value = selectedCardSelections.Values.Sum((BuildCreatorCardSelection selection) => selection.TotalCount);
			headerLabel.Text = $"{PlusLoc.Text("BUILD_CREATOR_TITLE")} ({value} {PlusLoc.Text("BUILD_CREATOR_CARD_COUNT_SUFFIX")}, {selectedRelicEntries.Count} {PlusLoc.Text("BUILD_CREATOR_RELIC_COUNT_SUFFIX")})";
		}
	}

	private void RefreshTabButtons()
	{
		ApplyTabStyle(cardsTabButton, currentTab == TabMode.Cards);
		ApplyTabStyle(relicsTabButton, currentTab == TabMode.Relics);
		if (searchInput != null)
		{
			searchInput.PlaceholderText = ((currentTab == TabMode.Cards) ? PlusLoc.Text("BUILD_CREATOR_SEARCH_CARDS") : PlusLoc.Text("BUILD_CREATOR_SEARCH_RELICS"));
		}
	}

	private void RefreshVisibleContent()
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Expected O, but got Unknown
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		if (itemListContainer == null)
		{
			return;
		}
		ClearChildren((Node)(object)itemListContainer);
		LineEdit? obj = searchInput;
		string search = ((obj == null) ? null : obj.Text?.Trim()) ?? string.Empty;
		int num = 0;
		if (currentTab == TabMode.Cards)
		{
			List<CardModel> list = FilterCards(search).ToList();
			for (int i = 0; i < list.Count; i += 4)
			{
				HBoxContainer val = new HBoxContainer
				{
					SizeFlagsHorizontal = (Control.SizeFlags)3
				};
				((Control)val).AddThemeConstantOverride((StringName)"separation", 18);
				((Node)itemListContainer).AddChild((Node)(object)val, false, (Node.InternalMode)0);
				List<CardModel> list2 = list.Skip(i).Take(4).ToList();
				foreach (CardModel item in list2)
				{
					((Node)val).AddChild((Node)(object)CreateCardRow(item), false, (Node.InternalMode)0);
					num++;
				}
				for (int j = list2.Count; j < 4; j++)
				{
					((Node)val).AddChild((Node)(object)CreateCardSpacer(), false, (Node.InternalMode)0);
				}
			}
		}
		else
		{
			foreach (RelicModel item2 in FilterRelics(search))
			{
				((Node)itemListContainer).AddChild((Node)(object)CreateRelicRow(item2), false, (Node.InternalMode)0);
				num++;
			}
		}
		if (num <= 0)
		{
			Label val2 = new Label
			{
				Text = PlusLoc.Text("BUILD_CREATOR_NO_RESULTS"),
				HorizontalAlignment = (HorizontalAlignment)1
			};
			((Control)val2).AddThemeColorOverride((StringName)"font_color", new Color(0.82f, 0.77f, 0.69f, 0.84f));
			((Node)itemListContainer).AddChild((Node)(object)val2, false, (Node.InternalMode)0);
		}
	}

	private void RefreshSummary()
	{
		if (selectedCardsContainer != null)
		{
			ClearChildren((Node)(object)selectedCardsContainer);
			foreach (KeyValuePair<string, BuildCreatorCardSelection> item in (from entry in selectedCardSelections
				where entry.Value.HasAny
				orderby entry.Value.TotalCount descending
				select entry).ThenBy<KeyValuePair<string, BuildCreatorCardSelection>, string>((KeyValuePair<string, BuildCreatorCardSelection> entry) => cardCatalog[entry.Key].Title, StringComparer.CurrentCultureIgnoreCase))
			{
				if (item.Value.BaseCount > 0)
				{
					((Node)selectedCardsContainer).AddChild((Node)(object)CreateSummaryLabel($"{item.Value.BaseCount}x {cardCatalog[item.Key].Title}"), false, (Node.InternalMode)0);
				}
				if (item.Value.UpgradedCount > 0)
				{
					((Node)selectedCardsContainer).AddChild((Node)(object)CreateSummaryLabel($"{item.Value.UpgradedCount}x {FormatUpgradedCardTitle(cardCatalog[item.Key])}"), false, (Node.InternalMode)0);
				}
			}
			if (((Node)selectedCardsContainer).GetChildCount(false) == 0)
			{
				((Node)selectedCardsContainer).AddChild((Node)(object)CreateSummaryLabel(PlusLoc.Text("BUILD_CREATOR_EMPTY_CARDS")), false, (Node.InternalMode)0);
			}
		}
		if (selectedRelicsContainer == null)
		{
			return;
		}
		ClearChildren((Node)(object)selectedRelicsContainer);
		foreach (string item2 in selectedRelicEntries.OrderBy<string, string>((string entry) => relicCatalog[entry].Title.GetFormattedText(), StringComparer.CurrentCultureIgnoreCase))
		{
			((Node)selectedRelicsContainer).AddChild((Node)(object)CreateSummaryLabel(relicCatalog[item2].Title.GetFormattedText()), false, (Node.InternalMode)0);
		}
		if (((Node)selectedRelicsContainer).GetChildCount(false) == 0)
		{
			((Node)selectedRelicsContainer).AddChild((Node)(object)CreateSummaryLabel(PlusLoc.Text("BUILD_CREATOR_EMPTY_RELICS")), false, (Node.InternalMode)0);
		}
	}

	private void RefreshApplyState()
	{
		if (applyButton != null && statusLabel != null)
		{
			bool flag = selectedCardSelections.Values.Sum((BuildCreatorCardSelection selection) => selection.TotalCount) > 0;
			((BaseButton)applyButton).Disabled = !flag;
			statusLabel.Text = (flag ? PlusLoc.Text("BUILD_CREATOR_READY") : PlusLoc.Text("BUILD_CREATOR_NEED_CARD"));
		}
	}

	private IEnumerable<CardModel> FilterCards(string search)
	{
		string search2 = search;
		Dictionary<string, CardModel>.ValueCollection values = cardCatalog.Values;
		if (string.IsNullOrWhiteSpace(search2))
		{
			return values;
		}
		return values.Where((CardModel card) => card.Title.Contains(search2, StringComparison.CurrentCultureIgnoreCase) || ((AbstractModel)card).Id.Entry.Contains(search2, StringComparison.OrdinalIgnoreCase));
	}

	private IEnumerable<RelicModel> FilterRelics(string search)
	{
		string search2 = search;
		Dictionary<string, RelicModel>.ValueCollection values = relicCatalog.Values;
		if (string.IsNullOrWhiteSpace(search2))
		{
			return values;
		}
		return values.Where((RelicModel relic) => relic.Title.GetFormattedText().Contains(search2, StringComparison.CurrentCultureIgnoreCase) || ((AbstractModel)relic).Id.Entry.Contains(search2, StringComparison.OrdinalIgnoreCase));
	}

	private Control CreateCardRow(CardModel card)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Expected O, but got Unknown
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Expected O, but got Unknown
		CardModel card2 = card;
		selectedCardSelections.TryGetValue(((AbstractModel)card2).Id.Entry, out var value);
		int totalCount = value.TotalCount;
		PanelContainer val = new PanelContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			CustomMinimumSize = new Vector2(0f, 420f)
		};
		((Control)val).ClipContents = false;
		((Control)val).AddThemeStyleboxOverride((StringName)"panel", (StyleBox)(object)CreatePanelStyle((totalCount > 0) ? SelectedRowColor : RowColor, (Color)((totalCount > 0) ? PanelBorderColor : new Color(0.22f, 0.16f, 0.12f, 1f)), 8));
		MarginContainer val2 = new MarginContainer();
		((Control)val2).AddThemeConstantOverride((StringName)"margin_left", 10);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_top", 8);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_right", 10);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_bottom", 8);
		((Node)val).AddChild((Node)(object)val2, false, (Node.InternalMode)0);
		VBoxContainer val3 = new VBoxContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			SizeFlagsVertical = (Control.SizeFlags)3
		};
		((Control)val3).AddThemeConstantOverride((StringName)"separation", 12);
		((Node)val2).AddChild((Node)(object)val3, false, (Node.InternalMode)0);
		PanelContainer val4 = new PanelContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			CustomMinimumSize = new Vector2(0f, CardPreviewVisualSize.Y + 12f)
		};
		((Control)val4).ClipContents = false;
		((Control)val4).AddThemeStyleboxOverride((StringName)"panel", (StyleBox)(object)CreatePanelStyle(new Color(0.12f, 0.09f, 0.08f, 0.96f), new Color(0.32f, 0.24f, 0.16f, 1f), 4));
		((Node)val3).AddChild((Node)(object)val4, false, (Node.InternalMode)0);
		Control previewHost = new Control
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			SizeFlagsVertical = (Control.SizeFlags)3,
			MouseFilter = (Control.MouseFilterEnum)2
		};
		previewHost.ClipContents = false;
		((Node)val4).AddChild((Node)(object)previewHost, false, (Node.InternalMode)0);
		Control preview = CreateCardPreview(card2);
		((Node)previewHost).AddChild((Node)(object)preview, false, (Node.InternalMode)0);
		((Node)previewHost).Ready += delegate
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			Callable val7 = Callable.From((Action)CenterPreview);
			val7.CallDeferred(Array.Empty<Variant>());
		};
		previewHost.Resized += CenterPreview;
		Callable val5 = Callable.From((Action)CenterPreview);
		val5.CallDeferred(Array.Empty<Variant>());
		VBoxContainer val6 = new VBoxContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3
		};
		((Control)val6).AddThemeConstantOverride((StringName)"separation", 8);
		((Node)val3).AddChild((Node)(object)val6, false, (Node.InternalMode)0);
		((Node)val6).AddChild((Node)(object)CreateCardCountSelector(PlusLoc.Text("BUILD_CREATOR_CARD_BASE"), value.BaseCount, delegate
		{
			AdjustCardSelection(((AbstractModel)card2).Id.Entry, -1, 0);
		}, delegate
		{
			AdjustCardSelection(((AbstractModel)card2).Id.Entry, 1, 0);
		}), false, (Node.InternalMode)0);
		if (card2.IsUpgradable)
		{
			((Node)val6).AddChild((Node)(object)CreateCardCountSelector(PlusLoc.Text("BUILD_CREATOR_CARD_UPGRADED"), value.UpgradedCount, delegate
			{
				AdjustCardSelection(((AbstractModel)card2).Id.Entry, 0, -1);
			}, delegate
			{
				AdjustCardSelection(((AbstractModel)card2).Id.Entry, 0, 1);
			}), false, (Node.InternalMode)0);
		}
		return (Control)(object)val;
		void CenterPreview()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			preview.Position = new Vector2(Mathf.Max(0f, (previewHost.Size.X - CardPreviewVisualSize.X) * 0.5f + CardPreviewHostOffset.X), Mathf.Max(0f, (previewHost.Size.Y - CardPreviewVisualSize.Y) * 0.5f + CardPreviewHostOffset.Y));
		}
	}

	private Control CreateRelicRow(RelicModel relic)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Expected O, but got Unknown
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Expected O, but got Unknown
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Expected O, but got Unknown
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Expected O, but got Unknown
		RelicModel relic2 = relic;
		bool flag = selectedRelicEntries.Contains(((AbstractModel)relic2).Id.Entry);
		PanelContainer val = new PanelContainer();
		((Control)val).AddThemeStyleboxOverride((StringName)"panel", (StyleBox)(object)CreatePanelStyle(flag ? SelectedRowColor : RowColor, (Color)(flag ? PanelBorderColor : new Color(0.22f, 0.16f, 0.12f, 1f)), 8));
		MarginContainer val2 = new MarginContainer();
		((Control)val2).AddThemeConstantOverride((StringName)"margin_left", 10);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_top", 8);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_right", 10);
		((Control)val2).AddThemeConstantOverride((StringName)"margin_bottom", 8);
		((Node)val).AddChild((Node)(object)val2, false, (Node.InternalMode)0);
		HBoxContainer val3 = new HBoxContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3
		};
		((Control)val3).AddThemeConstantOverride((StringName)"separation", 10);
		((Node)val2).AddChild((Node)(object)val3, false, (Node.InternalMode)0);
		TextureRect val4 = new TextureRect
		{
			Texture = relic2.BigIcon,
			CustomMinimumSize = new Vector2(44f, 44f),
			StretchMode = (TextureRect.StretchModeEnum)5,
			MouseFilter = (Control.MouseFilterEnum)2
		};
		((Node)val3).AddChild((Node)(object)val4, false, (Node.InternalMode)0);
		VBoxContainer val5 = new VBoxContainer
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3
		};
		((Control)val5).AddThemeConstantOverride((StringName)"separation", 2);
		((Node)val3).AddChild((Node)(object)val5, false, (Node.InternalMode)0);
		Label val6 = new Label
		{
			Text = relic2.Title.GetFormattedText(),
			SizeFlagsHorizontal = (Control.SizeFlags)3
		};
		((Control)val6).AddThemeFontSizeOverride((StringName)"font_size", 17);
		((Node)val5).AddChild((Node)(object)val6, false, (Node.InternalMode)0);
		Label val7 = new Label();
		RelicRarity rarity = relic2.Rarity;
		val7.Text = rarity.ToString();
		((Control)val7).SizeFlagsHorizontal = (Control.SizeFlags)3;
		Label val8 = val7;
		((Control)val8).AddThemeColorOverride((StringName)"font_color", new Color(0.83f, 0.78f, 0.7f, 0.84f));
		((Node)val5).AddChild((Node)(object)val8, false, (Node.InternalMode)0);
		MegaRichTextLabel val9 = new MegaRichTextLabel
		{
			BbcodeEnabled = true,
			AutoSizeEnabled = false,
			Text = relic2.DynamicDescription.GetFormattedText(),
			FitContent = true,
			AutowrapMode = (TextServer.AutowrapMode)3,
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			MouseFilter = (Control.MouseFilterEnum)2
		};
		((Control)val9).AddThemeColorOverride((StringName)"default_color", new Color(0.9f, 0.87f, 0.82f, 0.92f));
		((Node)val5).AddChild((Node)(object)val9, false, (Node.InternalMode)0);
		Button val10 = new Button
		{
			Text = (flag ? PlusLoc.Text("BUILD_CREATOR_REMOVE") : PlusLoc.Text("BUILD_CREATOR_ADD")),
			CustomMinimumSize = new Vector2(100f, 34f)
		};
		((BaseButton)val10).Pressed += delegate
		{
			if (!selectedRelicEntries.Add(((AbstractModel)relic2).Id.Entry))
			{
				selectedRelicEntries.Remove(((AbstractModel)relic2).Id.Entry);
			}
			RefreshAll();
		};
		((Node)val3).AddChild((Node)(object)val10, false, (Node.InternalMode)0);
		return (Control)(object)val;
	}

	private async Task ApplyAndCloseAsync()
	{
		if (player == null)
		{
			completionSource.TrySetResult(result: true);
			((Node)this).QueueFree();
		}
		else if (!(await BuildCreatorRuntime.ApplyBuildAsync(player, selectedCardSelections, selectedRelicEntries)))
		{
			if (statusLabel != null)
			{
				statusLabel.Text = PlusLoc.Text("BUILD_CREATOR_APPLY_FAILED");
			}
		}
		else
		{
			completionSource.TrySetResult(result: true);
			((Node)this).QueueFree();
		}
	}

	private void SwitchTab(TabMode mode)
	{
		if (currentTab != mode)
		{
			currentTab = mode;
			RefreshTabButtons();
			RefreshVisibleContent();
		}
	}

	private static Button CreateTabButton(string text)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		return new Button
		{
			Text = text,
			CustomMinimumSize = new Vector2(150f, 38f)
		};
	}

	private static void ApplyTabStyle(Button? button, bool selected)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (button != null)
		{
			((CanvasItem)button).Modulate = (selected ? new Color(1f, 0.95f, 0.82f, 1f) : new Color(0.84f, 0.8f, 0.74f, 1f));
		}
	}

	private static Label CreateSummaryLabel(string text)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Expected O, but got Unknown
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Label val = new Label
		{
			Text = text,
			AutowrapMode = (TextServer.AutowrapMode)3,
			SizeFlagsHorizontal = (Control.SizeFlags)3
		};
		((Control)val).AddThemeColorOverride((StringName)"font_color", new Color(0.9f, 0.85f, 0.78f, 1f));
		return val;
	}

	private static StyleBoxFlat CreatePanelStyle(Color background, Color border, int cornerRadius)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		return new StyleBoxFlat
		{
			BgColor = background,
			BorderColor = border,
			BorderWidthBottom = 2,
			BorderWidthLeft = 2,
			BorderWidthRight = 2,
			BorderWidthTop = 2,
			CornerRadiusBottomLeft = cornerRadius,
			CornerRadiusBottomRight = cornerRadius,
			CornerRadiusTopLeft = cornerRadius,
			CornerRadiusTopRight = cornerRadius
		};
	}

	private static string FormatCardCost(CardModel card)
	{
		return card.EnergyCost.CostsX ? "X" : card.EnergyCost.Canonical.ToString();
	}

	private static string FormatUpgradedCardTitle(CardModel card)
	{
		return (card.MaxUpgradeLevel > 1) ? (card.Title + "+1") : (card.Title + "+");
	}

	private Control CreateCardPreview(CardModel card)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		Control val = new Control
		{
			CustomMinimumSize = CardPreviewVisualSize,
			Size = CardPreviewVisualSize,
			MouseFilter = (Control.MouseFilterEnum)2
		};
		val.SetAnchorsAndOffsetsPreset((LayoutPreset)0, (LayoutPresetMode)0, 0);
		val.ClipContents = false;
		NCard previewCard = NCard.Create(card, (ModelVisibility)1);
		if (previewCard == null)
		{
			return val;
		}
		((Control)previewCard).MouseFilter = (Control.MouseFilterEnum)2;
		((Control)previewCard).SetAnchorsAndOffsetsPreset((LayoutPreset)0, (LayoutPresetMode)0, 0);
		((Control)previewCard).Size = NCard.defaultSize;
		((Control)previewCard).Scale = CardPreviewScale;
		((Control)previewCard).Position = CardPreviewVisualOffset;
		((GodotObject)previewCard).Connect(SignalName.Ready, Callable.From((Action)delegate
		{
			previewCard.UpdateVisuals((PileType)0, (CardPreviewMode)1);
		}), 0u);
		((Node)val).AddChild((Node)(object)previewCard, false, (Node.InternalMode)0);
		return val;
	}

	private Control CreateCardCountSelector(string labelText, int count, Action onRemove, Action onAdd)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Expected O, but got Unknown
		HBoxContainer val = new HBoxContainer();
		((Control)val).AddThemeConstantOverride((StringName)"separation", 6);
		((Control)val).SizeFlagsHorizontal = (Control.SizeFlags)3;
		Label val2 = new Label
		{
			Text = labelText,
			SizeFlagsHorizontal = (Control.SizeFlags)3,
			VerticalAlignment = (VerticalAlignment)1
		};
		((Control)val2).AddThemeFontSizeOverride((StringName)"font_size", 12);
		((Control)val2).AddThemeColorOverride((StringName)"font_color", new Color(0.89f, 0.82f, 0.71f, 1f));
		((Node)val).AddChild((Node)(object)val2, false, (Node.InternalMode)0);
		Button val3 = new Button
		{
			Text = "-",
			Disabled = (count <= 0),
			CustomMinimumSize = new Vector2(30f, 30f)
		};
		((BaseButton)val3).Pressed += onRemove;
		((Node)val).AddChild((Node)(object)val3, false, (Node.InternalMode)0);
		Label val4 = new Label
		{
			Text = count.ToString(),
			CustomMinimumSize = new Vector2(24f, 0f),
			HorizontalAlignment = (HorizontalAlignment)1,
			VerticalAlignment = (VerticalAlignment)1
		};
		((Control)val4).AddThemeFontSizeOverride((StringName)"font_size", 16);
		((Control)val4).AddThemeColorOverride((StringName)"font_color", new Color(0.98f, 0.9f, 0.72f, 1f));
		((Node)val).AddChild((Node)(object)val4, false, (Node.InternalMode)0);
		Button val5 = new Button
		{
			Text = "+",
			CustomMinimumSize = new Vector2(30f, 30f)
		};
		((BaseButton)val5).Pressed += onAdd;
		((Node)val).AddChild((Node)(object)val5, false, (Node.InternalMode)0);
		return (Control)(object)val;
	}

	private static Control CreateCardSpacer()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		return new Control
		{
			SizeFlagsHorizontal = (Control.SizeFlags)3
		};
	}

	private void AdjustCardSelection(string entry, int baseDelta, int upgradedDelta)
	{
		selectedCardSelections.TryGetValue(entry, out var value);
		int baseCount = Math.Max(0, value.BaseCount + baseDelta);
		int upgradedCount = Math.Max(0, value.UpgradedCount + upgradedDelta);
		BuildCreatorCardSelection value2 = new BuildCreatorCardSelection(baseCount, upgradedCount);
		if (value2.HasAny)
		{
			selectedCardSelections[entry] = value2;
		}
		else
		{
			selectedCardSelections.Remove(entry);
		}
		RefreshAll();
	}

	private static void ClearChildren(Node parent)
	{
		Node[] array = ((IEnumerable)parent.GetChildren(false)).OfType<Node>().ToArray();
		foreach (Node val in array)
		{
			parent.RemoveChild(val);
			val.QueueFree();
		}
	}

}
