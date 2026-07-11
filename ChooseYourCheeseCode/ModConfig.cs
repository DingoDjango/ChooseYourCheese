using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.addons.mega_text;
using BaseLib.Config;

namespace ChooseYourCheese.ChooseYourCheeseCode
{
    [ConfigHoverTipsByDefault]
    public class ModConfig : SimpleModConfig
    {
        private const float CardAspectRatio = 16f / 9f;
        private const float GridPadding = 8f;
        private const int ActChipWidth = 50;
        private const int ActChipHeight = 24;

        // Minimum card dimensions so VBoxContainer doesn't collapse before layout
        private const float MinCardWidth = 300f;
        private const float MinCardHeight = MinCardWidth / CardAspectRatio;
        // Reasonable initial width — optionContainer.Size.X is 0 at setup time
        private const float InitialCardWidth = 800f;

        private const int TitleFontSize = 28;
        private const int StatusFontSize = 20;
        private const int ActChipFontSize = 12;

        [ConfigIgnore]
        public static Dictionary<string, HashSet<int>> EventAllowedActs { get; set; } = new();

        public static bool EnableBrainLeech { get; set; } = false;
        public static bool EnableCrystalSphere { get; set; } = false;
        public static bool EnableDollRoom { get; set; } = false;
        public static bool EnableFakeMerchant { get; set; } = false;
        public static bool EnablePotionCourier { get; set; } = false;
        public static bool EnableRanwidTheElder { get; set; } = false;
        public static bool EnableRelicTrader { get; set; } = false;
        public static bool EnableRoomFullOfCheese { get; set; } = false;
        public static bool EnableSelfHelpBook { get; set; } = false;
        public static bool EnableSlipperyBridge { get; set; } = false;
        public static bool EnableStoneOfAllTime { get; set; } = false;
        public static bool EnableSymbiote { get; set; } = false;
        public static bool EnableTeaMaster { get; set; } = false;
        public static bool EnableTheFutureOfPotions { get; set; } = false;
        public static bool EnableTheLegendsWereTrue { get; set; } = false;
        public static bool EnableThisOrThat { get; set; } = false;
        public static bool EnableWelcomeToWongos { get; set; } = false;

        private static readonly Dictionary<string, bool[]> _cardActStates = new();
        private static readonly Dictionary<string, bool> _cardEnabledStates = new();
        private static Texture2D? _sharedGenericFallback;
        private Dictionary<string, Texture2D> _textureCache = new();
        private readonly Dictionary<string, EventModel?> _eventModelCache = new();

        private VBoxContainer? _eventGrid;
        private Control? _optionContainerRef;
        private float _lastCardWidth;
        private bool _isResizing;

        private static readonly CheeseEvent[] Events =
        {
            new("BrainLeech", "EnableBrainLeech", ActMask.Act1 | ActMask.Act2),
            new("CrystalSphere", "EnableCrystalSphere", ActMask.Act2 | ActMask.Act3),
            new("DollRoom", "EnableDollRoom", ActMask.Act1),
            new("FakeMerchant", "EnableFakeMerchant", ActMask.Act2 | ActMask.Act3),
            new("PotionCourier", "EnablePotionCourier", ActMask.Act2 | ActMask.Act3),
            new("RanwidTheElder", "EnableRanwidTheElder", ActMask.Act1 | ActMask.Act2),
            new("RelicTrader", "EnableRelicTrader", ActMask.Act1 | ActMask.Act2),
            new("RoomFullOfCheese", "EnableRoomFullOfCheese", ActMask.Act1 | ActMask.Act2),
            new("SelfHelpBook", "EnableSelfHelpBook", ActMask.Act1 | ActMask.Act2 | ActMask.Act3),
            new("SlipperyBridge", "EnableSlipperyBridge", ActMask.Act1 | ActMask.Act2 | ActMask.Act3),
            new("StoneOfAllTime", "EnableStoneOfAllTime", ActMask.Act2),
            new("Symbiote", "EnableSymbiote", ActMask.Act2 | ActMask.Act3),
            new("TeaMaster", "EnableTeaMaster", ActMask.Act1),
            new("TheFutureOfPotions", "EnableTheFutureOfPotions", ActMask.Act1 | ActMask.Act2 | ActMask.Act3),
            new("TheLegendsWereTrue", "EnableTheLegendsWereTrue", ActMask.Act1),
            new("ThisOrThat", "EnableThisOrThat", ActMask.Act1 | ActMask.Act2 | ActMask.Act3),
            new("WelcomeToWongos", "EnableWelcomeToWongos", ActMask.Act2)
        };

        private static Texture2D GetGenericFallbackTexture()
        {
            if (_sharedGenericFallback != null)
                return _sharedGenericFallback;

            try
            {
                var image = Image.CreateEmpty(128, 128, false, Image.Format.Rgba8);
                image.Fill(new Color(0.1f, 0.1f, 0.1f, 1f));
                var tex = ImageTexture.CreateFromImage(image);
                _sharedGenericFallback = tex;
#if EXPORTDEBUG
                MainFile.Logger.Info($"[DEBUG] Created shared generic fallback texture {tex.GetRid().Id}");
#endif
                return tex;
            }
            catch (Exception ex)
            {
                MainFile.Logger.Error($"[DEBUG] Failed to create generic fallback texture: {ex.Message}");
                throw new InvalidOperationException("Failed to create generic fallback texture", ex);
            }
        }

        public override void SetupConfigUI(Control optionContainer)
        {
            MainFile.Logger.Info("Setting up ChooseYourCheese config UI");

#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] optionContainer={optionContainer.GetType().Name}");
            MainFile.Logger.Info($"[DEBUG] optionContainer.Size={optionContainer.Size}");
            MainFile.Logger.Info($"[DEBUG] optionContainer.IsInsideTree={optionContainer.IsInsideTree()}");
            MainFile.Logger.Info($"[DEBUG] optionContainer.Visible={optionContainer.Visible}");
#endif

            _optionContainerRef = optionContainer;

            ResetInternalStates();
#if EXPORTDEBUG
            MainFile.Logger.Info("[DEBUG] ResetInternalStates complete");
#endif

            PreloadEventTextures();
#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] PreloadEventTextures complete, cache count={_textureCache.Count}");
#endif

            SyncPropertiesFromUI();
#if EXPORTDEBUG
            MainFile.Logger.Info("[DEBUG] SyncPropertiesFromUI complete");
#endif

            var headerLabel = CreateRawLabelControl("[b]Choose Your Cheese Events[/b]", 36);
            headerLabel.HorizontalAlignment = HorizontalAlignment.Center;
            optionContainer.AddChild(headerLabel);
            optionContainer.AddChild(CreateDividerControl());
#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] Header and divider added. optionContainer child count={optionContainer.GetChildCount()}");
#endif

            _eventGrid = new VBoxContainer
            {
                CustomMinimumSize = new Vector2(MinCardWidth, 0),
            };
            _eventGrid.AddThemeConstantOverride("separation", 20);
            _eventGrid.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

            // optionContainer.Size is (0, 16) at setup, but the parent (_contentPanel)
            // already has the correct layout size from NModConfigSubmenu.RefreshSize.
            var parent = optionContainer.GetParent();
            if (parent is Control parentCtrl && parentCtrl.Size.X > 0)
            {
                var usableWidth = parentCtrl.Size.X - (GridPadding * 2);
                _lastCardWidth = Math.Max(usableWidth, MinCardWidth);
#if EXPORTDEBUG
                MainFile.Logger.Info($"[DEBUG] Got initial card width from parent ({parent.GetType().Name}): parent={parentCtrl.Size.X}, usable={usableWidth}, final={_lastCardWidth}");
#endif
            }
            else
            {
                _lastCardWidth = InitialCardWidth;
#if EXPORTDEBUG
                MainFile.Logger.Info($"[DEBUG] Parent not available for width, using fallback: {_lastCardWidth}");
#endif
            }

#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] VBoxContainer created. _eventGrid.IsInsideTree={_eventGrid.IsInsideTree()}");
#endif

            var cardCount = 0;
            foreach (var eventDef in Events)
            {
                var card = CreateEventCard(eventDef);
#if EXPORTDEBUG
                MainFile.Logger.Info($"[DEBUG] Card created for {eventDef.Name}: type={card.GetType().Name}, valid={GodotObject.IsInstanceValid(card)}");
#endif
                _eventGrid.AddChild(card);
                cardCount++;
            }
#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] {cardCount} cards added to _eventGrid. _eventGrid child count={_eventGrid.GetChildCount()}");
#endif

            optionContainer.AddChild(_eventGrid);
#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] _eventGrid added to optionContainer. optionContainer child count={optionContainer.GetChildCount()}");
#endif

            AddRestoreDefaultsButton(optionContainer);
            SetupFocusNeighbors(optionContainer);

            _optionContainerRef.Resized += OnContainerResized;

#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] Final: optionContainer={optionContainer.Size}, _eventGrid={_eventGrid?.Size}, " +
                $"_lastCardWidth={_lastCardWidth}");
#endif

            MainFile.Logger.Info("ChooseYourCheese config UI setup complete");
        }

        private void ResetInternalStates()
        {
            _textureCache.Clear();
            _eventModelCache.Clear();
            _cardActStates.Clear();
            _cardEnabledStates.Clear();

            foreach (var eventDef in Events)
            {
                _cardActStates[eventDef.Name] = new bool[3];
                _cardEnabledStates[eventDef.Name] = false;
            }

            if (_eventGrid != null && GodotObject.IsInstanceValid(_eventGrid))
            {
                var childCount = _eventGrid.GetChildCount();
#if EXPORTDEBUG
                MainFile.Logger.Info($"[DEBUG] ResetInternalStates: freeing {childCount} children from old _eventGrid");
#endif
                foreach (var child in _eventGrid.GetChildren())
                {
                    if (GodotObject.IsInstanceValid(child))
                        child.QueueFree();
                }
                _eventGrid = null;
            }
            else
            {
#if EXPORTDEBUG
                MainFile.Logger.Info($"[DEBUG] ResetInternalStates: _eventGrid is null or invalid (was null={_eventGrid == null})");
#endif
            }
        }

        private void SyncPropertiesFromUI()
        {
            _cardEnabledStates["BrainLeech"] = EnableBrainLeech;
            _cardEnabledStates["CrystalSphere"] = EnableCrystalSphere;
            _cardEnabledStates["DollRoom"] = EnableDollRoom;
            _cardEnabledStates["FakeMerchant"] = EnableFakeMerchant;
            _cardEnabledStates["PotionCourier"] = EnablePotionCourier;
            _cardEnabledStates["RanwidTheElder"] = EnableRanwidTheElder;
            _cardEnabledStates["RelicTrader"] = EnableRelicTrader;
            _cardEnabledStates["RoomFullOfCheese"] = EnableRoomFullOfCheese;
            _cardEnabledStates["SelfHelpBook"] = EnableSelfHelpBook;
            _cardEnabledStates["SlipperyBridge"] = EnableSlipperyBridge;
            _cardEnabledStates["StoneOfAllTime"] = EnableStoneOfAllTime;
            _cardEnabledStates["Symbiote"] = EnableSymbiote;
            _cardEnabledStates["TeaMaster"] = EnableTeaMaster;
            _cardEnabledStates["TheFutureOfPotions"] = EnableTheFutureOfPotions;
            _cardEnabledStates["TheLegendsWereTrue"] = EnableTheLegendsWereTrue;
            _cardEnabledStates["ThisOrThat"] = EnableThisOrThat;
            _cardEnabledStates["WelcomeToWongos"] = EnableWelcomeToWongos;

            foreach (var eventDef in Events)
            {
                var acts = MaskToActIndices(eventDef.VanillaDefault);
                _cardActStates[eventDef.Name] = new bool[3];
                foreach (var actIdx in acts)
                    _cardActStates[eventDef.Name][actIdx] = true;
            }
        }

        private void OnContainerResized()
        {
            if (_optionContainerRef == null || _eventGrid == null || _isResizing) return;
            var containerWidth = _optionContainerRef.Size.X;
            if (containerWidth <= GridPadding * 2) return;
            var cardWidth = containerWidth - (GridPadding * 2);
            if (Math.Abs(cardWidth - _lastCardWidth) < 2f) return;

            _isResizing = true;
            try
            {
                _lastCardWidth = cardWidth;
#if EXPORTDEBUG
                MainFile.Logger.Info($"[DEBUG] OnContainerResized: rebuilding grid with cardWidth={cardWidth}");
#endif
                RefreshEventGrid();
            }
            finally
            {
                _isResizing = false;
            }
        }

        private void RefreshEventGrid()
        {
            if (_eventGrid == null || !GodotObject.IsInstanceValid(_eventGrid))
            {
#if EXPORTDEBUG
                MainFile.Logger.Warn("[DEBUG] RefreshEventGrid: _eventGrid null or invalid, skipping");
#endif
                return;
            }

            var oldChildCount = _eventGrid.GetChildCount();
#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] RefreshEventGrid: freeing {oldChildCount} old children");
#endif

            foreach (var child in _eventGrid.GetChildren())
            {
                if (GodotObject.IsInstanceValid(child))
                    child.QueueFree();
            }

            foreach (var eventDef in Events)
                _eventGrid.AddChild(CreateEventCard(eventDef));

#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] RefreshEventGrid: rebuilt with {_eventGrid.GetChildCount()} cards");
#endif
        }

        private Control CreateEventCard(CheeseEvent eventDef)
        {
            var enabled = _cardEnabledStates.GetValueOrDefault(eventDef.Name, false);
            var styleBox = CreateCardStyle(enabled);
            var cardWidth = _lastCardWidth;
            var cardHeight = cardWidth / CardAspectRatio;

#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] Creating card for {eventDef.Name}: enabled={enabled}, size={cardWidth}x{cardHeight}");
#endif

            var cardPanel = new Panel
            {
                CustomMinimumSize = new Vector2(cardWidth, cardHeight),
                SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter
            };
            cardPanel.AddThemeStyleboxOverride("panel", styleBox);

            var textureRect = new TextureRect
            {
                CustomMinimumSize = new Vector2(cardWidth, cardHeight),
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
                ZIndex = 0
            };

            var eventTexture = _textureCache.GetValueOrDefault(eventDef.Name, GetGenericFallbackTexture());
            textureRect.Texture = eventTexture;
            cardPanel.AddChild(textureRect);

            var eventModel = GetEventModel(eventDef.Name);
            var eventTitle = eventModel != null
                ? eventModel.Title.GetFormattedText()
                : eventDef.Name;

            var titleLabel = CreateRawLabelControl($"[b]{eventTitle}[/b]", TitleFontSize);
            titleLabel.HorizontalAlignment = HorizontalAlignment.Center;
            titleLabel.VerticalAlignment = VerticalAlignment.Center;
            titleLabel.CustomMinimumSize = new Vector2(cardWidth, cardHeight * 0.15f);
            titleLabel.Position = new Vector2(0, cardHeight * 0.05f);
            titleLabel.ZIndex = 2;
            cardPanel.AddChild(titleLabel);

            var chipsContainer = new HBoxContainer
            {
                CustomMinimumSize = new Vector2(0, ActChipHeight + 4),
                SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter
            };

            chipsContainer.AddChild(CreateActChip(eventDef.Name, 1));
            chipsContainer.AddChild(new HSeparator { CustomMinimumSize = new Vector2(4, ActChipHeight) });
            chipsContainer.AddChild(CreateActChip(eventDef.Name, 2));
            chipsContainer.AddChild(new HSeparator { CustomMinimumSize = new Vector2(4, ActChipHeight) });
            chipsContainer.AddChild(CreateActChip(eventDef.Name, 3));

            var chipsWrapper = new CenterContainer
            {
                CustomMinimumSize = new Vector2(cardWidth, ActChipHeight + 8),
                Position = new Vector2(0, cardHeight - ActChipHeight - 20)
            };
            chipsWrapper.AddChild(chipsContainer);
            cardPanel.AddChild(chipsWrapper);

            //var statusLabel = CreateRawLabelControl($"[b]{GetEventStatus(eventDef)}[/b]", StatusFontSize);
            //statusLabel.HorizontalAlignment = HorizontalAlignment.Center;
            //statusLabel.CustomMinimumSize = new Vector2(cardWidth, cardHeight * 0.1f);
            //statusLabel.Position = new Vector2(0, cardHeight - cardHeight * 0.1f);
            //statusLabel.ZIndex = 2;
            //cardPanel.AddChild(statusLabel);

            var button = new Button
            {
                Text = "",
                CustomMinimumSize = new Vector2(cardWidth, cardHeight),
                Modulate = new Color(1, 1, 1, 0),
                FocusMode = Control.FocusModeEnum.All,
                ZIndex = 1
};
            button.MouseFilter = Control.MouseFilterEnum.Ignore;
            
button.Pressed += () => ShowActSelectionPopup(eventDef);
            button.FocusMode = Control.FocusModeEnum.None;

            cardPanel.AddChild(button);

#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] {eventDef.Name} card complete: Panel children={cardPanel.GetChildCount()}");
#endif

            return cardPanel;
        }

        private void PreloadEventTextures()
        {
            foreach (var eventDef in Events)
            {
                try
                {
                    var eventModel = GetEventModel(eventDef.Name);

                    if (eventModel == null)
                    {
#if EXPORTDEBUG
                        MainFile.Logger.Warn($"[DEBUG] {eventDef.Name}: eventModel not found in ModelDb.AllEvents, using generic fallback");
#endif
                        _textureCache[eventDef.Name] = GetGenericFallbackTexture();
                        continue;
                    }

#if EXPORTDEBUG
                    MainFile.Logger.Info($"[DEBUG] {eventDef.Name}: eventModel found (type={eventModel.GetType().Name}), Title='{eventModel.Title.GetFormattedText()}'");
#endif

                    if (eventModel.GetType().Name == "FakeMerchant")
                    {
#if EXPORTDEBUG
                        MainFile.Logger.Info($"[DEBUG] {eventDef.Name}: FakeMerchant detected, using generic fallback (no portrait available)");
#endif
                        _textureCache[eventDef.Name] = GetGenericFallbackTexture();
                        continue;
                    }

                    var texture = eventModel.CreateInitialPortrait();
                    if (texture == null)
                    {
#if EXPORTDEBUG
                        MainFile.Logger.Warn($"[DEBUG] {eventDef.Name}: CreateInitialPortrait returned null, using generic fallback");
#endif
                        _textureCache[eventDef.Name] = GetGenericFallbackTexture();
                    }
                    else
                    {
#if EXPORTDEBUG
                        MainFile.Logger.Info($"[DEBUG] {eventDef.Name}: CreateInitialPortrait returned texture (type={texture.GetType().Name}, size={texture.GetSize()})");
#endif
                        // CompressedTexture2D from game assets may not render in dynamic TextureRect.
                        // Convert to ImageTexture for reliable runtime rendering.
                        if (texture is CompressedTexture2D compressedTex)
                        {
                            try
                            {
                                var image = compressedTex.GetImage();
                                var imageTex = ImageTexture.CreateFromImage(image);
                                _textureCache[eventDef.Name] = imageTex;
#if EXPORTDEBUG
                                MainFile.Logger.Info($"[DEBUG] {eventDef.Name}: Converted CompressedTexture2D to ImageTexture (size={imageTex.GetSize()})");
#endif
                            }
                            catch (Exception ex)
                            {
                                MainFile.Logger.Error($"[DEBUG] {eventDef.Name}: Failed to convert CompressedTexture2D to ImageTexture: {ex.Message}");
                                _textureCache[eventDef.Name] = texture;
                            }
                        }
                        else
                        {
                            _textureCache[eventDef.Name] = texture;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MainFile.Logger.Error($"[DEBUG] Failed to preload texture for {eventDef.Name}: {ex.Message}\n{ex.StackTrace}");
                    _textureCache[eventDef.Name] = GetGenericFallbackTexture();
                }
            }
        }

        private EventModel? GetEventModel(string eventName)
        {
            if (_eventModelCache.TryGetValue(eventName, out var cached))
            {
#if EXPORTDEBUG
                MainFile.Logger.Info($"[DEBUG] GetEventModel({eventName}): cache hit, model null={cached == null}");
#endif
                return cached;
            }

            try
            {
                var allEvents = ModelDb.AllEvents;
#if EXPORTDEBUG
                MainFile.Logger.Info($"[DEBUG] GetEventModel({eventName}): ModelDb.AllEvents has {allEvents.Count()} events");
#endif

                var eventModel = allEvents.FirstOrDefault(e => e.GetType().Name == eventName);

                if (eventModel == null)
                {
#if EXPORTDEBUG
                    var knownNames = string.Join(", ", allEvents.Select(e => e.GetType().Name).Take(10));
                    MainFile.Logger.Warn($"[DEBUG] GetEventModel({eventName}): not found in ModelDb. Known types (first 10): {knownNames}");
#endif
                }

                _eventModelCache[eventName] = eventModel;
                return eventModel;
            }
            catch (Exception ex)
            {
                MainFile.Logger.Error($"[DEBUG] Failed to get event model for {eventName}: {ex.Message}\n{ex.StackTrace}");
                return null;
            }
        }

        private static StyleBoxFlat CreateCardStyle(bool enabled) => new()
        {
            BgColor = enabled ? new Color(0.15f, 0.3f, 0.15f, 0.4f) : new Color(0.2f, 0.2f, 0.2f, 0.4f),
            BorderWidthLeft = 3,
            BorderWidthRight = 3,
            BorderWidthTop = 3,
            BorderWidthBottom = 3,
            BorderColor = enabled ? new Color(0.3f, 0.7f, 0.3f, 1f) : new Color(0.4f, 0.4f, 0.4f, 1f),
            CornerRadiusTopLeft = 10,
            CornerRadiusTopRight = 10,
            CornerRadiusBottomLeft = 10,
            CornerRadiusBottomRight = 10,
            ShadowColor = new Color(0, 0, 0, 0.3f),
            ShadowOffset = new Vector2(2, 2),
            ShadowSize = 4
        };

        private void AnimateCardHover(Panel cardPanel, StyleBoxFlat styleBox, bool isHovered)
        {
            try
            {
                var tween = cardPanel.CreateTween();
                var targetBg = isHovered ? styleBox.BgColor.Lightened(0.15f) : styleBox.BgColor;
                var targetBorder = isHovered ? styleBox.BorderColor.Lightened(0.25f) : styleBox.BorderColor;
                tween.TweenProperty(styleBox, "bg_color", targetBg, 0.15f);
                tween.TweenProperty(styleBox, "border_color", targetBorder, 0.15f);
            }
            catch (Exception ex)
            {
                MainFile.Logger.Warn($"[DEBUG] Animation error: {ex.Message}");
            }
        }

        private void SetActEnabled(string eventName, int actNumber, bool enabled)
        {
            if (!_cardActStates.ContainsKey(eventName))
                _cardActStates[eventName] = new bool[3];

            _cardActStates[eventName][actNumber - 1] = enabled;
            Changed();
            SyncEventPropertyToPublicProperty(eventName);
        }

        private void OnActChipToggled(string eventName, int actNumber, bool pressed)
        {
            SetActEnabled(eventName, actNumber, pressed);
            RefreshEventGrid();
        }

        private void SyncEventPropertyToPublicProperty(string eventName)
        {
            var anyActEnabled = _cardActStates.TryGetValue(eventName, out var acts) && acts.Any(a => a);
            _cardEnabledStates[eventName] = anyActEnabled;

            switch (eventName)
            {
                case "BrainLeech": EnableBrainLeech = anyActEnabled; break;
                case "CrystalSphere": EnableCrystalSphere = anyActEnabled; break;
                case "DollRoom": EnableDollRoom = anyActEnabled; break;
                case "FakeMerchant": EnableFakeMerchant = anyActEnabled; break;
                case "PotionCourier": EnablePotionCourier = anyActEnabled; break;
                case "RanwidTheElder": EnableRanwidTheElder = anyActEnabled; break;
                case "RelicTrader": EnableRelicTrader = anyActEnabled; break;
                case "RoomFullOfCheese": EnableRoomFullOfCheese = anyActEnabled; break;
                case "SelfHelpBook": EnableSelfHelpBook = anyActEnabled; break;
                case "SlipperyBridge": EnableSlipperyBridge = anyActEnabled; break;
                case "StoneOfAllTime": EnableStoneOfAllTime = anyActEnabled; break;
                case "Symbiote": EnableSymbiote = anyActEnabled; break;
                case "TeaMaster": EnableTeaMaster = anyActEnabled; break;
                case "TheFutureOfPotions": EnableTheFutureOfPotions = anyActEnabled; break;
                case "TheLegendsWereTrue": EnableTheLegendsWereTrue = anyActEnabled; break;
                case "ThisOrThat": EnableThisOrThat = anyActEnabled; break;
                case "WelcomeToWongos": EnableWelcomeToWongos = anyActEnabled; break;
            }
        }

        private static bool GetActEnabled(string eventName, int actNumber)
        {
            if (_cardActStates.TryGetValue(eventName, out var acts) && actNumber >= 1 && actNumber <= 3)
                return acts[actNumber - 1];
            return false;
        }

        private void ShowActSelectionPopup(CheeseEvent eventDef)
        {
            MainFile.Logger.Info($"[DEBUG] Opening act selection popup for {eventDef.Name}");
        }

        //private string GetEventStatus(CheeseEvent eventDef)
        //{
        //    var acts = new List<string>();
        //    if (GetActEnabled(eventDef.Name, 1)) acts.Add("A1");
        //    if (GetActEnabled(eventDef.Name, 2)) acts.Add("A2");
        //    if (GetActEnabled(eventDef.Name, 3)) acts.Add("A3");
        //    return acts.Count > 0 ? string.Join(", ", acts) : "Disabled";
        //}

        public static void UpdateEventActs()
        {
#if EXPORTDEBUG
            MainFile.Logger.Info("[DEBUG] UpdateEventActs called");
#endif
            EventAllowedActs.Clear();

            foreach (var eventDef in Events)
            {
                var mask = GetActMaskStatic(eventDef.Name);
                if (mask == ActMask.None) continue;

                EventAllowedActs[eventDef.Name] = MaskToActIndices(mask);

#if EXPORTDEBUG
                MainFile.Logger.Info($"[DEBUG] UpdateEventActs: {eventDef.Name} -> acts [{string.Join(", ", MaskToActIndices(mask))}]");
#endif
            }

#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] UpdateEventActs complete. EventAllowedActs has {EventAllowedActs.Count} entries");
#endif
        }

        private static ActMask GetActMaskStatic(string eventName)
        {
            var mask = ActMask.None;
            if (GetActEnabled(eventName, 1)) mask |= ActMask.Act1;
            if (GetActEnabled(eventName, 2)) mask |= ActMask.Act2;
            if (GetActEnabled(eventName, 3)) mask |= ActMask.Act3;
            return mask;
        }

        private static HashSet<int> MaskToActIndices(ActMask mask)
        {
            var set = new HashSet<int>();
            if ((mask & ActMask.Act1) != 0) set.Add(0);
            if ((mask & ActMask.Act2) != 0) set.Add(1);
            if ((mask & ActMask.Act3) != 0) set.Add(2);
            return set;
        }

        private Control CreateActChip(string eventName, int actNumber)
        {
            var enabled = GetActEnabled(eventName, actNumber);

            var actLoc = new LocString("gameplay_ui", "ACT_NUMBER");
            actLoc.Add("actNumber", actNumber);
            var actLabelText = actLoc.GetFormattedText();

#if EXPORTDEBUG
            MainFile.Logger.Info($"[DEBUG] CreateActChip({eventName}, act={actNumber}): enabled={enabled}, label='{actLabelText}'");
#endif

            var checkBox = new CheckBox
            {
                Text = "",
                CustomMinimumSize = new Vector2(ActChipWidth, ActChipHeight),
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                FocusMode = Control.FocusModeEnum.None
            };

            var chipStyle = new StyleBoxFlat
            {
                BgColor = enabled ? new Color(0.1f, 0.3f, 0.1f, 0.8f) : new Color(0.2f, 0.2f, 0.2f, 0.6f),
                BorderWidthLeft = 2,
                BorderWidthRight = 2,
                BorderWidthTop = 2,
                BorderWidthBottom = 2,
                BorderColor = enabled ? new Color(0.3f, 0.7f, 0.3f, 1f) : new Color(0.4f, 0.4f, 0.4f, 1f),
                CornerRadiusTopLeft = 6,
                CornerRadiusTopRight = 6,
                CornerRadiusBottomLeft = 6,
                CornerRadiusBottomRight = 6
            };
            checkBox.AddThemeStyleboxOverride("normal", chipStyle);

            var toggleStyle = new StyleBoxFlat
            {
                BgColor = new Color(0.15f, 0.35f, 0.15f, 0.9f),
                BorderWidthLeft = 2,
                BorderWidthRight = 2,
                BorderWidthTop = 2,
                BorderWidthBottom = 2,
                BorderColor = new Color(0.3f, 0.7f, 0.3f, 1f),
                CornerRadiusTopLeft = 6,
                CornerRadiusTopRight = 6,
                CornerRadiusBottomLeft = 6,
                CornerRadiusBottomRight = 6
            };
            checkBox.AddThemeStyleboxOverride("pressed", toggleStyle);

            var checkboxLabel = CreateRawLabelControl(actLabelText, ActChipFontSize);
            checkboxLabel.CustomMinimumSize = new Vector2(ActChipWidth, ActChipHeight);
            checkboxLabel.HorizontalAlignment = HorizontalAlignment.Center;
            checkBox.AddChild(checkboxLabel);

            checkBox.ButtonPressed = enabled;
            checkBox.Toggled += (pressed) => OnActChipToggled(eventName, actNumber, pressed);

            return checkBox;
        }

        [Flags]
        public enum ActMask
        {
            None = 0,
            Act1 = 1,
            Act2 = 2,
            Act3 = 4
        }

        public class CheeseEvent
        {
            public string Name { get; }
            public string ConfigProperty { get; }
            public ActMask VanillaDefault { get; }

            public CheeseEvent(string name, string configProperty, ActMask vanillaDefault)
            {
                Name = name;
                ConfigProperty = configProperty;
                VanillaDefault = vanillaDefault;
            }
        }
    }
}
