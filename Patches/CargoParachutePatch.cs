using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using NO_AIDS.Core;
using UnityEngine;

namespace NO_AIDS.Patches {
    [HarmonyPatch(typeof(HUDBombingState), "CCIPTrajectory")]
    internal static class CargoParachutePatch {
        private const float DistanceFactor = 0.75f;

        private static readonly FieldInfo ParachuteSystemField = AccessTools.Field(typeof(Container), "parachuteSystem");
        private static readonly FieldInfo ListParachutesField = AccessTools.Field(typeof(CargoDeploymentSystem), "listParachutes");

        private static readonly Dictionary<MountedCargo, bool> hasParachuteCache = new Dictionary<MountedCargo, bool>();

        private static bool HasParachute(MountedCargo cargo) {
            if (cargo == null) return false;
            bool cached;
            if (hasParachuteCache.TryGetValue(cargo, out cached)) return cached;

            bool result = false;
            if (cargo.cargo != null && cargo.cargo.unitPrefab != null) {
                Container container = cargo.cargo.unitPrefab.GetComponent<Container>();
                GameObject parachuteSystemPrefab = container != null ? ParachuteSystemField.GetValue(container) as GameObject : null;
                CargoDeploymentSystem deploySystem = parachuteSystemPrefab != null ? parachuteSystemPrefab.GetComponent<CargoDeploymentSystem>() : null;
                if (deploySystem != null) {
                    var list = ListParachutesField.GetValue(deploySystem) as List<Parachute>;
                    result = list != null && list.Count > 0;
                }
            }
            hasParachuteCache[cargo] = result;
            if (result) {
                Plugin.Logger.LogInfo("CargoParachute: detected parachute on " + cargo.cargo.name + ", applying distance correction.");
            }
            return result;
        }

        [HarmonyPostfix]
        private static void Postfix(Aircraft aircraft, ref WeaponStation ___weaponStation, ref GlobalPosition ___ccipImpactPoint) {
            MountedCargo mountedCargo = ___weaponStation != null && ___weaponStation.Weapons.Count > 0
                ? ___weaponStation.Weapons[0] as MountedCargo
                : null;
            if (!HasParachute(mountedCargo)) return;

            GlobalPosition aircraftPos = GlobalPositionExtensions.GlobalPosition(aircraft);
            Vector3 offset = ___ccipImpactPoint - aircraftPos;
            offset *= DistanceFactor;
            ___ccipImpactPoint = aircraftPos + offset;
        }
    }
}
