using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD_2.Core.Ship
{
    [HarmonyPatch(typeof(PLServer), "Update")]
    internal class FleetShipIsPlayerShip
    {
        // Starting Ship & TeamID manipulation on FleetShips to trick the Game into having multiple PlayerShips.
        public static void Postfix(PLServer __instance)
        {
            if (!Global.ModEnabled || __instance == null || !__instance.GameHasStarted || PLNetworkManager.Instance.LocalPlayer == null || !PLNetworkManager.Instance.LocalPlayer.GetHasStarted()
            || PLEncounterManager.Instance.PlayerShip == null) return;
            if (PhotonNetwork.isMasterClient)
            {
                /// Setting player starting ships dictates which ship they spawn at etc.
                foreach (ShipInfo shipInfo in Global.FleetShips)
                {
                    PLShipInfoBase shipInfoBase = PLEncounterManager.Instance.GetShipFromID(shipInfo.ShipID);
                    if (shipInfo != null && shipInfoBase != null)
                    {
                        foreach (int i in shipInfo.Crew)
                        {
                            PLPlayer pLPlayer = PLServer.Instance.GetPlayerFromPlayerID(i);
                            if (pLPlayer != null && pLPlayer.StartingShip != (PLShipInfo)shipInfoBase)
                            {
                                pLPlayer.StartingShip = (PLShipInfo)shipInfoBase;
                            }
                        }
                    }
                }
            }
            else
            {
                /// With multiple player ships, we can use the TeamID to set the 'main' playership as TeamID -1 are treated as unclaimed.
                PLNetworkManager.Instance.LocalPlayer.StartingShip.LastBeginWarpServerTime = PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.LastBeginWarpServerTime;
                if (PLNetworkManager.Instance.LocalPlayer.StartingShip != null && PLNetworkManager.Instance.LocalPlayer.StartingShip.TeamID != 0)
                {
                    PLNetworkManager.Instance.LocalPlayer.StartingShip.TeamID = 0;
                }
                if (PLNetworkManager.Instance.LocalPlayer.GetPawn() != null && PLNetworkManager.Instance.LocalPlayer.GetPawn().Lifetime > 2f && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip != null)
                {
                    if (PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.GetIsPlayerShip() && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.TeamID != -1 && PLNetworkManager.Instance.LocalPlayer.StartingShip != null && PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip != PLNetworkManager.Instance.LocalPlayer.StartingShip)
                    {
                        PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.TeamID = -1;
                    }
                }
            }
        }
    }
}
