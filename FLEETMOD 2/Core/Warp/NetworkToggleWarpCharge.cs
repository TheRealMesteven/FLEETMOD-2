﻿using HarmonyLib;

namespace FLEETMOD_2.Core.Warp
{
    [HarmonyPatch(typeof(PLServer), "NetworkToggleWarpCharge")]
    internal class NetworkToggleWarpCharge
    {
        public static bool Prefix(PLServer __instance, int inShipID, int inWarpCharge, PhotonMessageInfo pmi)
        {
            if (!Global.ModEnabled) return true;
            PLShipInfoBase shipFromID = PLEncounterManager.Instance.GetShipFromID(inShipID);
            if (shipFromID != null)
            {
                // Allow Non-FleetModded Clients to interact with warp aside from Engaging / Skipping.
                if (!Global.FleetModClients.Contains(PLServer.GetPlayerForPhotonPlayer(pmi.sender).GetPlayerID()) && (shipFromID.WarpChargeStage == EWarpChargeStage.E_WCS_READY || shipFromID.WarpChargeStage == EWarpChargeStage.E_WCS_ACTIVE))
                {
                    PLServer.Instance.photonView.RPC("AddNotification", pmi.sender, new object[]
                    {
                        "Sorry, You cannot use the Jump Computer without Fleet Mod.",
                        PLServer.GetPlayerForPhotonPlayer(pmi.sender).GetPlayerID(),
                        PLServer.Instance.GetEstimatedServerMs() + 3000,
                        true
                    });
                    return false;
                }

                // Notification Management
                if (PhotonNetwork.isMasterClient && pmi.sender != null && shipFromID.WarpChargeStage != (EWarpChargeStage)inWarpCharge && shipFromID != null && shipFromID.GetIsPlayerShip())
                {
                    PLPlayer playerForPhotonPlayer = PLServer.GetPlayerForPhotonPlayer(pmi.sender);
                    if (playerForPhotonPlayer != null && playerForPhotonPlayer.TeamID == 0 && !playerForPhotonPlayer.IsBot)
                    {
                        PLPlayer cachedFriendlyPlayerOfClass = PLServer.Instance.GetCachedFriendlyPlayerOfClass(0);
                        if (cachedFriendlyPlayerOfClass != null && playerForPhotonPlayer != cachedFriendlyPlayerOfClass)
                        {
                            if (inWarpCharge != 1)
                            {
                                if (inWarpCharge == 2)
                                {
                                    PLServer.Instance.photonView.RPC("AddNotification", cachedFriendlyPlayerOfClass.GetPhotonPlayer(), new object[]
                                    {
                                                "[PL] has paused the jump prep",
                                                playerForPhotonPlayer.GetPlayerID(),
                                                PLServer.Instance.GetEstimatedServerMs() + 6000,
                                                true
                                    });
                                }
                            }
                            else
                            {
                                PLServer.Instance.photonView.RPC("AddNotification", cachedFriendlyPlayerOfClass.GetPhotonPlayer(), new object[]
                                {
                                            "[PL] has started jump prep",
                                            playerForPhotonPlayer.GetPlayerID(),
                                            PLServer.Instance.GetEstimatedServerMs() + 6000,
                                            true
                                });
                            }
                        }
                    }
                }
                shipFromID.WarpChargeStage = (EWarpChargeStage)inWarpCharge;
            }
            return false;
        }
    }
}
