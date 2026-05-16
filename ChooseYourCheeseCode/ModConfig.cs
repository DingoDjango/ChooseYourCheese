using BaseLib.Config;

namespace ChooseYourCheese.ChooseYourCheeseCode
{
    [ConfigHoverTipsByDefault]
    public class ModConfig : SimpleModConfig
    {
        [ConfigIgnore]
        public static Dictionary<string, HashSet<int>> EventAllowedActs { get; set; } = new()
        {            
            ["BrainLeech"] = new HashSet<int> { 0, 1 },
            ["CrystalSphere"] = new HashSet<int> { 1, 2 },
            ["DollRoom"] = new HashSet<int> { 1 },
            ["FakeMerchant"] = new HashSet<int> { 1, 2 },
            ["PotionCourier"] = new HashSet<int> { 1, 2 },
            ["RanwidTheElder"] = new HashSet<int> { 1, 2 },
            ["RelicTrader"] = new HashSet<int> { 1, 2 },
            ["RoomFullOfCheese"] = new HashSet<int> { 0, 1 },
            ["SelfHelpBook"] = new HashSet<int> { 0, 1, 2 },
            ["SlipperyBridge"] = new HashSet<int> { 0, 1, 2 },
            ["StoneOfAllTime"] = new HashSet<int> { 1 },
            ["Symbiote"] = new HashSet<int> { 1, 2 },
            ["TeaMaster"] = new HashSet<int> { 0, 1 },
            ["TheFutureOfPotions"] = new HashSet<int> { 0, 1, 2 },
            ["TheLegendsWereTrue"] = new HashSet<int> { 0 },
            ["ThisOrThat"] = new HashSet<int> { 0, 1, 2 },
            ["WelcomeToWongos"] = new HashSet<int> { 1 }
        };

        // Shared events == events that can appear in all acts
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

        // BrainLeech section - vanilla: Acts 0, 1
        [ConfigSection("BrainLeech")]
        [ConfigVisibleIf(nameof(EnableBrainLeech))]
        public static bool BrainLeech_Act1 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableBrainLeech))]
        public static bool BrainLeech_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableBrainLeech))]
        public static bool BrainLeech_Act3 { get; set; } = false;

        // CrystalSphere section - vanilla: Acts 1, 2
        [ConfigSection("CrystalSphere")]
        [ConfigVisibleIf(nameof(EnableCrystalSphere))]
        public static bool CrystalSphere_Act1 { get; set; } = false;

        [ConfigVisibleIf(nameof(EnableCrystalSphere))]
        public static bool CrystalSphere_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableCrystalSphere))]
        public static bool CrystalSphere_Act3 { get; set; } = true;

        // DollRoom section - vanilla: Act 1 only
        [ConfigSection("DollRoom")]
        [ConfigVisibleIf(nameof(EnableDollRoom))]
        public static bool DollRoom_Act1 { get; set; } = false;

        [ConfigVisibleIf(nameof(EnableDollRoom))]
        public static bool DollRoom_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableDollRoom))]
        public static bool DollRoom_Act3 { get; set; } = false;

        // FakeMerchant section - vanilla: Acts 1, 2
        [ConfigSection("FakeMerchant")]
        [ConfigVisibleIf(nameof(EnableFakeMerchant))]
        public static bool FakeMerchant_Act1 { get; set; } = false;

        [ConfigVisibleIf(nameof(EnableFakeMerchant))]
        public static bool FakeMerchant_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableFakeMerchant))]
        public static bool FakeMerchant_Act3 { get; set; } = true;

        // PotionCourier section - vanilla: Acts 1, 2
        [ConfigSection("PotionCourier")]
        [ConfigVisibleIf(nameof(EnablePotionCourier))]
        public static bool PotionCourier_Act1 { get; set; } = false;

        [ConfigVisibleIf(nameof(EnablePotionCourier))]
        public static bool PotionCourier_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnablePotionCourier))]
        public static bool PotionCourier_Act3 { get; set; } = true;

        // RanwidTheElder section - vanilla: Acts 1, 2
        [ConfigSection("RanwidTheElder")]
        [ConfigVisibleIf(nameof(EnableRanwidTheElder))]
        public static bool RanwidTheElder_Act1 { get; set; } = false;

        [ConfigVisibleIf(nameof(EnableRanwidTheElder))]
        public static bool RanwidTheElder_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableRanwidTheElder))]
        public static bool RanwidTheElder_Act3 { get; set; } = true;

        // RelicTrader section - vanilla: Acts 1, 2
        [ConfigSection("RelicTrader")]
        [ConfigVisibleIf(nameof(EnableRelicTrader))]
        public static bool RelicTrader_Act1 { get; set; } = false;

        [ConfigVisibleIf(nameof(EnableRelicTrader))]
        public static bool RelicTrader_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableRelicTrader))]
        public static bool RelicTrader_Act3 { get; set; } = true;

        // RoomFullOfCheese section - vanilla: Acts 0, 1
        [ConfigSection("RoomFullOfCheese")]
        [ConfigVisibleIf(nameof(EnableRoomFullOfCheese))]
        public static bool RoomFullOfCheese_Act1 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableRoomFullOfCheese))]
        public static bool RoomFullOfCheese_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableRoomFullOfCheese))]
        public static bool RoomFullOfCheese_Act3 { get; set; } = false;

        // SelfHelpBook section - vanilla: all acts
        [ConfigSection("SelfHelpBook")]
        [ConfigVisibleIf(nameof(EnableSelfHelpBook))]
        public static bool SelfHelpBook_Act1 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableSelfHelpBook))]
        public static bool SelfHelpBook_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableSelfHelpBook))]
        public static bool SelfHelpBook_Act3 { get; set; } = true;

        // SlipperyBridge section - vanilla: all acts (floor-based)
        [ConfigSection("SlipperyBridge")]
        [ConfigVisibleIf(nameof(EnableSlipperyBridge))]
        public static bool SlipperyBridge_Act1 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableSlipperyBridge))]
        public static bool SlipperyBridge_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableSlipperyBridge))]
        public static bool SlipperyBridge_Act3 { get; set; } = true;

        // StoneOfAllTime section - vanilla: Act 1 only
        [ConfigSection("StoneOfAllTime")]
        [ConfigVisibleIf(nameof(EnableStoneOfAllTime))]
        public static bool StoneOfAllTime_Act1 { get; set; } = false;

        [ConfigVisibleIf(nameof(EnableStoneOfAllTime))]
        public static bool StoneOfAllTime_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableStoneOfAllTime))]
        public static bool StoneOfAllTime_Act3 { get; set; } = false;

        // Symbiote section - vanilla: Acts 1, 2
        [ConfigSection("Symbiote")]
        [ConfigVisibleIf(nameof(EnableSymbiote))]
        public static bool Symbiote_Act1 { get; set; } = false;

        [ConfigVisibleIf(nameof(EnableSymbiote))]
        public static bool Symbiote_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableSymbiote))]
        public static bool Symbiote_Act3 { get; set; } = true;

        // TeaMaster section - vanilla: Acts 0, 1
        [ConfigSection("TeaMaster")]
        [ConfigVisibleIf(nameof(EnableTeaMaster))]
        public static bool TeaMaster_Act1 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableTeaMaster))]
        public static bool TeaMaster_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableTeaMaster))]
        public static bool TeaMaster_Act3 { get; set; } = false;

        // TheFutureOfPotions section - vanilla: all acts
        [ConfigSection("TheFutureOfPotions")]
        [ConfigVisibleIf(nameof(EnableTheFutureOfPotions))]
        public static bool TheFutureOfPotions_Act1 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableTheFutureOfPotions))]
        public static bool TheFutureOfPotions_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableTheFutureOfPotions))]
        public static bool TheFutureOfPotions_Act3 { get; set; } = true;

        // TheLegendsWereTrue section - vanilla: Act 0 only
        [ConfigSection("TheLegendsWereTrue")]
        [ConfigVisibleIf(nameof(EnableTheLegendsWereTrue))]
        public static bool TheLegendsWereTrue_Act1 { get; set; } = true;

        // ThisOrThat section - vanilla: all acts
        [ConfigSection("ThisOrThat")]
        [ConfigVisibleIf(nameof(EnableThisOrThat))]
        public static bool ThisOrThat_Act1 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableThisOrThat))]
        public static bool ThisOrThat_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableThisOrThat))]
        public static bool ThisOrThat_Act3 { get; set; } = true;

        // WelcomeToWongos section - vanilla: Act 1 only
        [ConfigSection("WelcomeToWongos")]
        [ConfigVisibleIf(nameof(EnableWelcomeToWongos))]
        public static bool WelcomeToWongos_Act1 { get; set; } = false;

        [ConfigVisibleIf(nameof(EnableWelcomeToWongos))]
        public static bool WelcomeToWongos_Act2 { get; set; } = true;

        [ConfigVisibleIf(nameof(EnableWelcomeToWongos))]
        public static bool WelcomeToWongos_Act3 { get; set; } = false;

        public static void UpdateEventActs()
        {
            UpdateEvent("BrainLeech", EnableBrainLeech, BrainLeech_Act1, BrainLeech_Act2, BrainLeech_Act3);
            UpdateEvent("CrystalSphere", EnableCrystalSphere, CrystalSphere_Act1, CrystalSphere_Act2, CrystalSphere_Act3);
            UpdateEvent("DollRoom", EnableDollRoom, DollRoom_Act1, DollRoom_Act2, DollRoom_Act3);
            UpdateEvent("FakeMerchant", EnableFakeMerchant, FakeMerchant_Act1, FakeMerchant_Act2, FakeMerchant_Act3);
            UpdateEvent("PotionCourier", EnablePotionCourier, PotionCourier_Act1, PotionCourier_Act2, PotionCourier_Act3);
            UpdateEvent("RanwidTheElder", EnableRanwidTheElder, RanwidTheElder_Act1, RanwidTheElder_Act2, RanwidTheElder_Act3);
            UpdateEvent("RelicTrader", EnableRelicTrader, RelicTrader_Act1, RelicTrader_Act2, RelicTrader_Act3);
            UpdateEvent("RoomFullOfCheese", EnableRoomFullOfCheese, RoomFullOfCheese_Act1, RoomFullOfCheese_Act2, RoomFullOfCheese_Act3);
            UpdateEvent("SelfHelpBook", EnableSelfHelpBook, SelfHelpBook_Act1, SelfHelpBook_Act2, SelfHelpBook_Act3);
            UpdateEvent("SlipperyBridge", EnableSlipperyBridge, SlipperyBridge_Act1, SlipperyBridge_Act2, SlipperyBridge_Act3);
            UpdateEvent("StoneOfAllTime", EnableStoneOfAllTime, StoneOfAllTime_Act1, StoneOfAllTime_Act2, StoneOfAllTime_Act3);
            UpdateEvent("Symbiote", EnableSymbiote, Symbiote_Act1, Symbiote_Act2, Symbiote_Act3);
            UpdateEvent("TeaMaster", EnableTeaMaster, TeaMaster_Act1, TeaMaster_Act2, TeaMaster_Act3);
            UpdateEvent("TheFutureOfPotions", EnableTheFutureOfPotions, TheFutureOfPotions_Act1, TheFutureOfPotions_Act2, TheFutureOfPotions_Act3);
            UpdateEvent("TheLegendsWereTrue", EnableTheLegendsWereTrue, TheLegendsWereTrue_Act1, false, false);
            UpdateEvent("ThisOrThat", EnableThisOrThat, ThisOrThat_Act1, ThisOrThat_Act2, ThisOrThat_Act3);
            UpdateEvent("WelcomeToWongos", EnableWelcomeToWongos, WelcomeToWongos_Act1, WelcomeToWongos_Act2, WelcomeToWongos_Act3);

#if EXPORTDEBUG
                MainFile.Logger.Info("Updated EventAllowedActs:");
                foreach (var kvp in EventAllowedActs)
                {
                    MainFile.Logger.Info($"  {kvp.Key}: Acts {string.Join(", ", kvp.Value)}");
                }
#endif
        }

        private static void UpdateEvent(string eventName, bool enabled, bool act1, bool act2, bool act3)
        {
            if (enabled)
            {
                EventAllowedActs[eventName] = new HashSet<int>();
                if (act1) EventAllowedActs[eventName].Add(0);
                if (act2) EventAllowedActs[eventName].Add(1);
                if (act3) EventAllowedActs[eventName].Add(2);
            }
            else
            {
                EventAllowedActs.Remove(eventName);
            }
        }

        /*
        // Custom UI methods - commented out for now
        private static string GetEventTitle(string eventName)
        {
            var eventModel = ModelDb.AllEvents.FirstOrDefault(e => e.GetType().Name == eventName);
            return eventModel?.Title.GetFormattedText() ?? eventName;
        }

        private static string FormatLocalizedString(string key, params object[] args)
        {
            var baseKey = $"CHOOSEYOURCHEESE-{key}";
            var dynamicKey = $"{baseKey}.dynamic.{string.Join("_", args.Select(a => a.ToString().Replace(" ", "_")))}";

            var loc = LocString.GetIfExists("settings_ui", dynamicKey);
            if (loc == null)
            {
                var template = LocString.GetIfExists("settings_ui", baseKey);
                if (template != null)
                {
                    string formattedText = template.GetRawText();
                    for (int i = 0; i < args.Length; i++)
                    {
                        formattedText = formattedText.Replace($"{{{i}}}", args[i].ToString());
                    }
                    LocManager.Instance.GetTable("settings_ui").MergeWith(new Dictionary<string, string> { { dynamicKey, formattedText } });
                    loc = new LocString("settings_ui", dynamicKey);
                }
                else
                {
                    return key;
                }
            }
            return loc.GetFormattedText();
        }

        public override void SetupConfigUI(Control optionContainer)
        {
            foreach (var eventName in EventNames)
            {
                var eventTitle = GetEventTitle(eventName);
                var section = CreateCollapsibleSection(eventTitle);
                optionContainer.AddChild(section);
                section.ContentContainer.AddChild(CreateEventToggle(eventName));

                // Only show Act toggles if the event is enabled
                var enableProp = GetType().GetProperty($"Enable{eventName}");
                var isEnabled = (bool)enableProp.GetValue(null);
                if (isEnabled)
                {
                    section.ContentContainer.AddChild(CreateDividerControl());
                    section.ContentContainer.AddChild(CreateActToggle(eventName, 1));
                    section.ContentContainer.AddChild(CreateActToggle(eventName, 2));
                    section.ContentContainer.AddChild(CreateActToggle(eventName, 3));
                }
            }

            AddRestoreDefaultsButton(optionContainer);
            SetupFocusNeighbors(optionContainer);
        }

        private NConfigOptionRow CreateEventToggle(string eventName)
        {
            var prop = GetType().GetProperty($"Enable{eventName}");
            var eventTitle = GetEventTitle(eventName);
            var labelText = FormatLocalizedString("MODIFY_EVENT", eventTitle);
            var label = CreateRawLabelControl(labelText, 28);
            var control = CreateRawTickboxControl(prop);
            var option = new NConfigOptionRow(ModPrefix, $"Enable{eventName}", label, control);
            var dynamicHoverKey = $"CHOOSEYOURCHEESE-MODIFY_EVENT.dynamic.{eventName}";
            option.AddCustomHoverTip(null, dynamicHoverKey);
            return option;
        }

        private NConfigOptionRow CreateActToggle(string eventName, int actNumber)
        {
            var prop = GetType().GetProperty($"{eventName}_Act{actNumber}");
            var labelText = $"Act {actNumber}";
            var label = CreateRawLabelControl(labelText, 28);
            var control = CreateRawTickboxControl(prop);
            var option = new NConfigOptionRow(ModPrefix, $"{eventName}_Act{actNumber}", label, control);
            option.AddCustomHoverTip(null, $"CHOOSEYOURCHEESE-ACT{actNumber}.hover.desc");
            return option;
        }
        */
    }
}
