using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace ChooseYourCheese.ChooseYourCheeseCode
{
    internal static class HarmonyPatches
    {
        public static Harmony harmony = new Harmony($"Dingo.Harmony.{MainFile.ModId}");

        internal static void InitializeHarmony()
        {
            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(RunManager), nameof(RunManager.GenerateRooms)),
                    prefix: new HarmonyMethod(typeof(HarmonyPatches), nameof(RunManager_GenerateRooms_Prefix))
                );

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
                return true;

            var eventsCopy = __instance.events.ToList();
            for (int i = 0; i < eventsCopy.Count; i++)
            {
                EventModel eventModel = eventsCopy[i];
                string eventName = eventModel.GetType().Name;

#if EXPORTDEBUG
                MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid: Checking event {eventName}");
#endif

                // Skip already-visited events
                if (runState.VisitedEventIds.Contains(eventModel.Id))
                {
#if EXPORTDEBUG
                    MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid: {eventName} already visited");
#endif
                    continue;
                }

                bool passesModConfigCheck = CheckModConfigRestrictions(eventName, runState);
                if (!passesModConfigCheck)
                {
#if EXPORTDEBUG
                    MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid: Skipping {eventName} due to mod config restrictions");
#endif
                    continue;
                }

                bool passesVanillaRestrictions = CheckVanillaNonActRestrictions(eventName, runState);
                if (!passesVanillaRestrictions)
                {
#if EXPORTDEBUG
                    MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid: Skipping {eventName} due to vanilla restrictions");
#endif
                    continue;
                }

#if EXPORTDEBUG
                MainFile.Logger.Info($"RoomSet.EnsureNextEventIsValid: {eventName} passed all checks, setting as next event");
#endif

                // Move selected event to front of the list
                __instance.events.RemoveAt(i);
                __instance.events.Insert(0, eventModel);
                __instance.eventsVisited = 0;

                return false; // prevent original method from running
            }

#if EXPORTDEBUG
            MainFile.Logger.Warn("RoomSet.EnsureNextEventIsValid: All unique events exhausted, allowing repetition", 2);
#endif

            return true; // let original method handle fallback
        }

        private static bool CheckModConfigRestrictions(string eventName, RunState runState)
        {
            if (!ModConfig.EventAllowedActs.TryGetValue(eventName, out var allowedActs))
            {
                // Not a mod-controlled event — let vanilla IsAllowed decide
                var eventModel = ModelDb.AllEvents.FirstOrDefault(e => e.GetType().Name == eventName);
                return eventModel?.IsAllowed(runState) ?? false;
            }

            int currentActIndex = runState.CurrentActIndex;

            if (!allowedActs.Contains(currentActIndex))
            {
#if EXPORTDEBUG
                MainFile.Logger.Info($"RoomSet: Blocking {eventName} in act index {currentActIndex} due to mod config");
#endif
                return false;
            }

#if EXPORTDEBUG
            MainFile.Logger.Info($"RoomSet: {eventName} allowed in act index {currentActIndex} by mod config");
#endif

            return true;
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
                _ => true
            };
        }

        private static bool CheckCrystalSphereRestrictions(IRunState runState)
            => runState.Players.All(p => p.Gold >= 100);

        private static bool CheckRelicTraderRestrictions(IRunState runState)
            => runState.Players.All(p => p.Relics.Count >= 5);

        private static bool CheckSlipperyBridgeRestrictions(IRunState runState)
            => runState.TotalFloor > 6 && runState.Players.All(p => p.Deck.Cards.Any(c => c.IsRemovable));

        private static bool CheckStoneOfAllTimeRestrictions(IRunState runState)
            => runState.Players.All(p => p.Potions.Any());

        private static bool CheckTeaMasterRestrictions(IRunState runState)
            => runState.Players.All(p => p.Gold >= 150);

        private static bool CheckTheFutureOfPotionsRestrictions(IRunState runState)
            => runState.Players.All(p => p.Potions.Count<PotionModel>() >= 2);

        private static bool CheckTheLegendsWereTrueRestrictions(IRunState runState)
            => runState.Players.All(p => p.Deck.Cards.Count > 0) && runState.Players.All(p => p.Creature.CurrentHp >= 10);
    }
}