using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace AutoAbilityToggle
{
    [BepInPlugin(Guid, Name, Version)]
    public class AutoAbilityToggle : BaseUnityPlugin
    {
        private const string Guid = "coust.talentedmods.autoabilitytoggle";
        private const string Name = "Auto Ability Toggle";
        private const string Version = "1.0.0.0";

        private readonly Harmony _harmony = new Harmony(Guid);

        internal static ManualLogSource Log;

        // This array will store the toggle state (ON/OFF) for each of the 5 abilities.
        // We make it internal static so our patch class can access it.
        internal static readonly bool[] AutoAbilityToggles = new bool[5];

        private void Awake()
        {
            Log = Logger;
            _harmony.PatchAll();
            Log.LogInfo($"Plugin {Name} is loaded!");
        }
        
        private void Update()
        {
            for (int i = 0; i < AutoAbilityToggles.Length; i++)
            {
                // KeyCode for F1 is 282. F2 is 283, and so on.
                // So, KeyCode.F1 + i gives us the correct F-key for the current loop iteration.
                if (Input.GetKeyDown(KeyCode.F1 + i))
                {
                    AutoAbilityToggles[i] = !AutoAbilityToggles[i];
                    string status = AutoAbilityToggles[i] ? "ON" : "OFF";
                    Log.LogInfo($"Auto-Ability {i + 1} toggled {status}");
                }
            }
        }
    }
    
    [HarmonyPatch(typeof(InputManager))]
    internal class InputManager_Patch
    {
        [HarmonyPatch("RefreshAbilitiesHeld")]
        [HarmonyPostfix]
        public static void RefreshAbilitiesHeld_Postfix(InputManager __instance)
        {
            for (int i = 0; i < AutoAbilityToggle.AutoAbilityToggles.Length; i++)
            {
                if (AutoAbilityToggle.AutoAbilityToggles[i])
                {
                    __instance.CurrentAbilityInputs[i] = true;
                }
            }
        }
    }
}