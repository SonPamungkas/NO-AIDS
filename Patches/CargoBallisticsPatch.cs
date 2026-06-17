using System.Linq;
using HarmonyLib;
using NO_AIDS.Core;
using UnityEngine;
using UnityEngine.UI;

namespace NO_AIDS.Patches {
    [HarmonyPatch(typeof(HUDBombingState), "SetHUDWeaponState")]
    internal static class CargoBallisticsPatch {
        private static Missile referenceMissile;
        private static bool lookedUp;

        private static Missile GetReferenceMissile() {
            if (lookedUp) return referenceMissile;
            lookedUp = true;
            WeaponInfo info = Resources.FindObjectsOfTypeAll<WeaponInfo>()
                .FirstOrDefault(w => w.weaponName == "Demolition Bomb");
            if (info != null && info.weaponPrefab != null) {
                referenceMissile = info.weaponPrefab.GetComponent<Missile>();
            }
            if (referenceMissile == null) {
                Plugin.Logger.LogWarning("CargoBallistics: could not find Demolition Bomb reference Missile, falling back to generic values.");
            }
            return referenceMissile;
        }

        [HarmonyPrefix]
        private static bool Prefix(
            HUDBombingState __instance,
            Image targetDesignator,
            Aircraft aircraft,
            WeaponStation weaponStation,
            ref WeaponStation ___weaponStation,
            ref WeaponInfo ___weaponInfo,
            ref float ___dragCoef,
            ref float ___finArea,
            ref float ___mass) {
            WeaponInfo info = weaponStation.WeaponInfo;
            if (info.weaponPrefab != null && info.weaponPrefab.GetComponent<Missile>() != null) {
                return true; // real bomb/missile - let the original method run unmodified
            }

            ___weaponStation = weaponStation;
            ___weaponInfo = info;

            MountedCargo cargo = weaponStation.Weapons.Count > 0 ? weaponStation.Weapons[0] as MountedCargo : null;
            ___mass = cargo != null ? cargo.GetMass() : info.massPerRound;

            Missile reference = GetReferenceMissile();
            ___dragCoef = reference != null ? reference.GetDragCoef(0.008726646f) : info.dragCoef;
            ___finArea = reference != null ? reference.GetFinArea() : 0f;

            targetDesignator.color = Color.green;
            targetDesignator.transform.localScale = Vector3.one;

            SceneSingleton<CameraStateManager>.i.SetDesiredFoV(PlayerSettings.defaultFoV, 0f);
            SceneSingleton<FlightHud>.i.waterline.enabled = true;
            SceneSingleton<FlightHud>.i.velocityVector.transform.localScale = Vector3.one;

            return false; // skip the original - it would have crashed on GetComponent<Missile>()
        }
    }
}
