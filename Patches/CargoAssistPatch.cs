using HarmonyLib;
using NO_AIDS.Core;

namespace NO_AIDS.Patches {
    [HarmonyPatch(typeof(CombatHUD), "ShowWeaponStation")]
    internal static class CargoAssistPatch {
        private static bool Qualifies(WeaponStation station) {
            if (station == null) return false;
            WeaponInfo info = station.WeaponInfo;
            if (info == null) return false;
            if (station.HasTurret()) return false;
            if (info.boresight || info.bomb) return false;
            return info.cargo || info.troops;
        }

        [HarmonyPrefix]
        private static void Prefix(WeaponStation weaponStation, out bool __state) {
            __state = Qualifies(weaponStation);
            if (__state) {
                weaponStation.WeaponInfo.bomb = true;
                Plugin.Logger.LogInfo("CargoAssist: routing " + weaponStation.WeaponInfo.weaponName + " through BombingUI.");
            }
        }

        [HarmonyPostfix]
        private static void Postfix(WeaponStation weaponStation, bool __state) {
            if (__state) weaponStation.WeaponInfo.bomb = false;
        }
    }
}
