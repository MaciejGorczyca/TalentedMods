using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace AutoAimToggle
{
    [BepInPlugin(Guid, Name, Version)]
    public class AutoAimToggle : BaseUnityPlugin
    {
        private const string Guid = "coust.talentedmods.autoaimtoggle";
        private const string Name = "Auto Aim Toggle";
        private const string Version = "1.0.0.0";
        
        private readonly Harmony _harmony = new Harmony(Guid);
        
        internal static ManualLogSource Log;
        
        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"Plugin {Name} is loaded!");
            _harmony.PatchAll();
        }
    }
    
    [HarmonyPatch(typeof(InputManager))]
    internal class InputManager_Patch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        public static void Update_Postfix(InputManager __instance)
        {
            if (Input.GetKeyDown(KeyCode.KeypadMultiply))
            {
                __instance.AutoAimEnabled = !__instance.AutoAimEnabled;
                
                string status = __instance.AutoAimEnabled ? "ENABLED" : "DISABLED";
                AutoAimToggle.Log.LogInfo($"Auto Aim Toggled: {status}");
            }
        }
    }
}