using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace NO_AIDS.Core {
    [BepInPlugin("com.noaids.cargoassist", "NO-AIDS", "1.0")] //permanent
    public class Plugin : BaseUnityPlugin {
        public static Harmony harmony;
        internal static new ManualLogSource Logger;

        private void Awake() {
            Logger = base.Logger;
            harmony = new Harmony("noaids.cargoassist");
            harmony.PatchAll();
            Logger.LogInfo("NO-AIDS loaded: bomb assist pip now available for cargo drops.");
        }
    }
}
