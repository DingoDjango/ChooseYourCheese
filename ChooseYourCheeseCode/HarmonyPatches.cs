using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace ChooseYourCheese.ChooseYourCheeseCode
{
    internal static class HarmonyPatches
    {
        private static readonly Dictionary<string, int> ActIntegers = new Dictionary<string, int>()
        {
            { "OVERGROWTH", 0 },
            { "UNDERDOCKS", 0 },
            { "HIVE", 1 },
            { "GLORY", 2 }
        };

        public static Harmony harmony = new Harmony($"Dingo.Harmony.{MainFile.ModId}");

        internal static void InitializeHarmony()
        {
            try
            {
                /* Update event preferences before run starts */
                harmony.Patch(
                    original: AccessTools.Method(typeof(RunManager), nameof(RunManager.GenerateRooms)),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(RunManager_GenerateRooms_Prefix))
                );

                /* Override EnsureNextEventIsValid to apply mod act restrictions
                 * This gives us full control over event selection without needing to patch IsAllowed */
                harmony.Patch(
                    original: AccessTools.Method(typeof(RoomSet), nameof(RoomSet.EnsureNextEventIsValid)),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(RoomSet_EnsureNextEventIsValid_Prefix))
                );
            }
            catch (Exception ex)
            {
                MainFile.Logger.Error($"Error applying Harmony patches: {ex}");
            }
        }

        private static void RunManager_GenerateRooms_Prefix()
        {
#if EXPORTDEBUG
            MainFile.Logger.Info("RunManager.GenerateRooms called - updating event config");
#endif

            ModConfig.UpdateEventActs();
        }

        private static bool RoomSet_EnsureNextEventIsValid_Prefix(RoomSet __instance, RunState runState)
        {
#if EXPORTDEBUG
            MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid called, events count: {__instance.events.Count}, eventsVisited: {__instance.eventsVisited}");
#endif

            if (__instance.events.Count == 0)
            {
                return true; // Run original
            }

            // Iterate through events to find one that passes both vanilla and mod restrictions
            for (int i = 0; i < __instance.events.Count; i++)
            {
                EventModel eventModel = __instance.events[i];
                string eventName = eventModel.GetType().Name;

#if EXPORTDEBUG
                    MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid: Checking event {eventName}");
#endif

                // Check mod act restrictions first
                if (ModConfig.EventAllowedActs.TryGetValue(eventName, out var allowedActs))
                {
                    int currentActIndex = runState.CurrentActIndex;

                    if (!allowedActs.Contains(currentActIndex))
                    {
#if EXPORTDEBUG
                            MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid: Blocking {eventName} in act index {currentActIndex} due to mod config");
#endif
                        continue; // Skip this event
                    }

#if EXPORTDEBUG
                        MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid: {eventName} allowed in act index {currentActIndex} by mod config");
#endif

                    // User allows this act - check vanilla non-act restrictions while ignoring act check
                    bool vanillaRestrictionsPass = CheckVanillaNonActRestrictions(eventName, runState);

#if EXPORTDEBUG
                        MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid: {eventName} vanilla non-act restrictions: {vanillaRestrictionsPass}");
#endif

                    if (!vanillaRestrictionsPass)
                    {
                        continue; // Skip this event
                    }
                }
                else
                {
                    // No mod config for this event - use vanilla IsAllowed
                    if (!eventModel.IsAllowed(runState))
                    {
#if EXPORTDEBUG
                            MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid: {eventName} failed vanilla IsAllowed");
#endif
                        continue; // Skip this event
                    }
                }

                // Check if already visited
                if (runState.VisitedEventIds.Contains(eventModel.Id))
                {
#if EXPORTDEBUG
                        MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid: {eventName} already visited");
#endif
                    continue; // Skip this event
                }

#if EXPORTDEBUG
                    MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid: {eventName} passed all checks, setting as next event");
#endif

                // Move this event to the front of the list
                __instance.events.RemoveAt(i);
                __instance.events.Insert(0, eventModel);
                __instance.eventsVisited = 0;

                return false; // Skip original - we've already found a valid event
            }

#if EXPORTDEBUG
                MainFile.Logger.Warn("RoomSet.EnsureNextEventIsValid: All unique events exhausted, allowing repetition", 2);
#endif

            return true; // Run original to handle repetition
        }

        private static bool CheckVanillaNonActRestrictions(string eventName, IRunState runState)
        {
            return eventName switch
            {
                "CrystalSphere" => CheckCrystalSphereRestrictions(runState),
                "RelicTrader" => CheckRelicTraderRestrictions(runState),
                "SlipperyBridge" => CheckSlipperyBridgeRestrictions(runState),
                "StoneOfAllTime" => CheckStoneOfAllTimeRestrictions(runState),
                "TeaMaster" => CheckTeaMasterRestrictions(runState),
                "TheFutureOfPotions" => CheckTheFutureOfPotionsRestrictions(runState),
                "TheLegendsWereTrue" => CheckTheLegendsWereTrueRestrictions(runState),
                _ => true // Events with only act restrictions or no restrictions
            };
        }

        private static bool CheckCrystalSphereRestrictions(IRunState runState)
        {
            return runState.Players.All(p => p.Gold >= 100);
        }

        private static bool CheckRelicTraderRestrictions(IRunState runState)
        {
            return runState.Players.All(p => p.Relics.Count >= 5);
        }

        private static bool CheckSlipperyBridgeRestrictions(IRunState runState)
        {
            return runState.TotalFloor > 6 && runState.Players.All(p => p.Deck.Cards.Any(c => c.IsRemovable));
        }

        private static bool CheckStoneOfAllTimeRestrictions(IRunState runState)
        {
            return runState.Players.All(p => p.Potions.Any());
        }

        private static bool CheckTeaMasterRestrictions(IRunState runState)
        {
            return runState.Players.All(p => p.Gold >= 150);
        }

        private static bool CheckTheFutureOfPotionsRestrictions(IRunState runState)
        {
            return runState.Players.All(p => p.Potions.Count<PotionModel>() >= 2);
        }

        private static bool CheckTheLegendsWereTrueRestrictions(IRunState runState)
        {
            return runState.Players.All(p => p.Deck.Cards.Count > 0) && runState.Players.All(p => p.Creature.CurrentHp >= 10);
        }
    }
}
