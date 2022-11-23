using HarmonyLib;
using PulsarModLoader.Utilities;

namespace FLEETMOD_2.Core.Ship
{
    [HarmonyPatch(typeof(PLShipInfoBase), "GetIsPlayerShip")]
    internal class GetIsPlayerShipPatch
    {
        public static bool Prefix(PLShipInfoBase __instance, ref bool __result)
        {
            if (!Global.ModEnabled) return true;
            if (Global.GetFleetShips().Contains(__instance.ShipID))
            {
                __result = true;
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
