using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD_2.Core.Ship
{
    [HarmonyPatch(typeof(PLRepairDepot), "Update")]
    internal class RepairDepot
    {
        public static bool Prefix(PLRepairDepot __instance, ref PLSensorObjectString[] ___SensorStrings)
        {
            if (!Global.ModEnabled) return true;
            __instance.TargetShip = null;
            float num = 50f;
            if (PLEncounterManager.Instance.GetCPEI() != null && __instance.MySensorObject != null)
            {
                if (!PLEncounterManager.Instance.GetCPEI().MySensorObjects.Contains(__instance.MySensorObject))
                {
                    PLEncounterManager.Instance.GetCPEI().MySensorObjects.Add(__instance.MySensorObject);
                }
                __instance.MySensorObject.ManualName = "Repair Depot";
                __instance.MySensorObject.ManualEMSignature = 1f;
                __instance.MySensorObject.ManualSensorStrings = ___SensorStrings;
            }
            foreach (PLShipInfoBase plshipInfoBase in PLEncounterManager.Instance.AllShips.Values)
            {
                if (plshipInfoBase != null && (plshipInfoBase.Exterior.transform.position - __instance.transform.position).sqrMagnitude < num * num)
                {
                    __instance.TargetShip = plshipInfoBase;
                }
            }
            return false;
        }
    }
}
