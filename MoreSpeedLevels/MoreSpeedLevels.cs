using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace MoreSpeedLevels
{
    [BepInPlugin(Guid, Name, Version)]
    public class MoreSpeedLevels : BaseUnityPlugin
    {
        private const string Guid = "coust.talentedmods.morespeedlevels";
        private const string Name = "More Speed Levels";
        private const string Version = "1.0.0.0";

        private readonly Harmony _harmony = new Harmony(Guid);
        
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            _harmony.PatchAll();
            Log.LogInfo($"Plugin {Name} is loaded!");
        }
    }

    [HarmonyPatch(typeof(LevelManager))]
    internal class LevelManager_Patch
    {
        // The total number of speed levels after adding our modded ones (4 original + 4 modded).
        private const int TotalSpeedLevels = 8;
        private const int MaxSpeedLevelIndex = TotalSpeedLevels - 1;

        // This patch runs *after* the LevelManager's Awake method.
        // This ensures the dictionary is already initialized before we modify it.
        [HarmonyPatch("Awake")]
        [HarmonyPostfix]
        public static void Awake_Postfix(LevelManager __instance)
        {
            var speedScales = AccessTools.Field(typeof(LevelManager), "speedTypeTimeScales")
                                         .GetValue(__instance) as Dictionary<SpeedType, float>;

            if (speedScales == null)
            {
                MoreSpeedLevels.Log.LogError("Could not find speedTypeTimeScales dictionary!");
                return;
            }

            // We can't add to the enum, but we can cast integers to the enum type for the dictionary keys.
            // The original enum has values 0, 1, 2, 3. We will add 4, 5, 6, 7.
            speedScales[(SpeedType)4] = 3f;  // Modded1
            speedScales[(SpeedType)5] = 5f;  // Modded2
            speedScales[(SpeedType)6] = 10f; // Modded3
            speedScales[(SpeedType)7] = 20f; // Modded4

            MoreSpeedLevels.Log.LogInfo("Added 4 new speed levels (3x, 5x, 10x, 20x).");
        }

        // This Prefix patch *replaces* the original TimeSpeedUp method.
        [HarmonyPatch("TimeSpeedUp")]
        [HarmonyPrefix]
        public static bool TimeSpeedUp_Prefix(LevelManager __instance)
        {
            int newSpeedIndex = (int)__instance.CurrentTimeSpeedType + 1;
            // Clamp the value to our new maximum index.
            __instance.CurrentTimeSpeedType = (SpeedType)Mathf.Clamp(newSpeedIndex, 0, MaxSpeedLevelIndex);
            AccessTools.Method(typeof(LevelManager), "RefreshCurrentTimeSpeed").Invoke(__instance, null);
            return false; // Skip the original method.
        }

        // This Prefix patch *replaces* the original TimeSpeedDown method.
        [HarmonyPatch("TimeSpeedDown")]
        [HarmonyPrefix]
        public static bool TimeSpeedDown_Prefix(LevelManager __instance)
        {
            int newSpeedIndex = (int)__instance.CurrentTimeSpeedType - 1;
            // Clamp the value to our new maximum index (minimum is always 0).
            __instance.CurrentTimeSpeedType = (SpeedType)Mathf.Clamp(newSpeedIndex, 0, MaxSpeedLevelIndex);
            AccessTools.Method(typeof(LevelManager), "RefreshCurrentTimeSpeed").Invoke(__instance, null);
            return false; // Skip the original method.
        }
    }
}