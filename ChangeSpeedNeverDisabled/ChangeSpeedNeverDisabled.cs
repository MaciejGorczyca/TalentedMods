using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ChangeSpeedNeverDisabled
{
    [BepInPlugin(Guid, Name, Version)]
    public class ChangeSpeedNeverDisabled : BaseUnityPlugin
    {
        private const string Guid = "coust.talentedmods.changespeedneverdisabled";
        private const string Name = "Change Speed Never Disabled";
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

    // This patch targets the TimeSpeedStub class, which controls the speed UI.
    [HarmonyPatch(typeof(TimeSpeedStub))]
    internal class TimeSpeedStub_Patch
    {
        [HarmonyPatch("get_available")]
        [HarmonyPostfix]
        public static void GetAvailable_Postfix(ref bool __result)
        {
            // We simply force the result to be true, overriding all of the game's
            // original conditions (like checking for endless mode).
            __result = true;
        }
    }
}