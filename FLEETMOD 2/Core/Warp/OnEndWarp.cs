using HarmonyLib;
using System.Collections.Generic;

namespace FLEETMOD_2.Core.Warp
{
    [HarmonyPatch(typeof(PLShipInfoBase), "OnEndWarp")]
    internal class OnEndWarp
    {
        // Skips warp for ALL Fleet Ships
        public static bool Prefix(PLShipInfoBase __instance)
        {
            if (!Global.ModEnabled) return true;
            if (PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip == __instance as PLShipInfo)
            {
                foreach (int plshipid in Global.GetFleetShips())
                {
                    PLShipInfoBase pLShipInfoBase = PLEncounterManager.Instance.GetShipFromID(plshipid);
                    if (pLShipInfoBase != null && pLShipInfoBase.InWarp && pLShipInfoBase != __instance)
                    {
                        pLShipInfoBase.SkipWarp();
                        pLShipInfoBase.InWarp = false;
                        pLShipInfoBase.WarpChargeStage = EWarpChargeStage.E_WCS_COLD_START;
                    }
                }
            }
            return false;
        }
    }
}
