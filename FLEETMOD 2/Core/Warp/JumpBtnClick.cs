using HarmonyLib;
using System;

namespace FLEETMOD_2.Core.Warp
{
    [HarmonyPatch(typeof(PLWarpDriveScreen), "JumpBtnClick")]
    internal class JumpBtnClick
    {
        // Patch to ensure Fleet cannot warp if Unaligned / Uncharged / Unfueled
        public static bool Prefix(PLWarpDriveScreen __instance)
        {
            if (!Global.ModEnabled) return true;
            bool CantWarp = false;

            // Get Status of Each FleetShip
            foreach (int pLShipID in Global.GetFleetShips())
            {
                PLShipInfoBase plshipInfoBase = PLEncounterManager.Instance.GetShipFromID(pLShipID);
                if (plshipInfoBase.GetIsPlayerShip() && plshipInfoBase != null && (PLStarmap.Instance.CurrentShipPath.Count <= 1 
                || !(PLServer.Instance.ClientHasFullStarmap && plshipInfoBase.WarpTargetID != PLStarmap.Instance.CurrentShipPath[1].ID)
                || plshipInfoBase.WarpChargeStage != EWarpChargeStage.E_WCS_READY || plshipInfoBase.NumberOfFuelCapsules < 1))
                {
                    CantWarp = true;
                    break;
                }
            }

            // Check if should sync button press or a requirement has not been met
            if (__instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip() && !__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard)
            {
                bool flag8 = false;
                EWarpChargeStage ewarpChargeStage = EWarpChargeStage.E_WCS_COLD_START;
                switch (__instance.MyScreenHubBase.OptionalShipInfo.WarpChargeStage)
                {
                    case EWarpChargeStage.E_WCS_PREPPING:
                        ewarpChargeStage = EWarpChargeStage.E_WCS_PAUSED;
                        flag8 = true;
                        break;
                    case EWarpChargeStage.E_WCS_PAUSED:
                        ewarpChargeStage = EWarpChargeStage.E_WCS_PREPPING;
                        flag8 = true;
                        break;
                    case EWarpChargeStage.E_WCS_READY:
                        {
                            if (!CantWarp && __instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID != -1
                                && PLBeaconInfo.GetBeaconStatAdditive(EBeaconType.E_WARP_DISABLE, __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip()) < 0.5f)
                            {
                                flag8 = true;
                                if (PLNetworkManager.Instance.LocalPlayer != null)
                                {
                                    PLServer.Instance.photonView.RPC("CPEI_HandleActivateWarpDrive", PhotonTargets.MasterClient, new object[]
                                    {
                                    __instance.MyScreenHubBase.OptionalShipInfo.ShipID,
                                    __instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID,
                                    PLNetworkManager.Instance.LocalPlayer.GetPlayerID()
                                    });
                                }
                                ewarpChargeStage = EWarpChargeStage.E_WCS_ACTIVE;
                            }
                            break;
                        }
                    case EWarpChargeStage.E_WCS_ACTIVE:
                        {
                            if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
                            {
                                PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.photonView.RPC("SkipWarp", PhotonTargets.All, Array.Empty<object>());
                            }
                            break;
                        }
                    default:
                        ewarpChargeStage = EWarpChargeStage.E_WCS_PREPPING;
                        flag8 = true;
                        break;
                }
                if (flag8)
                {
                    if (!PhotonNetwork.isMasterClient)
                    {
                        __instance.MyScreenHubBase.OptionalShipInfo.WarpChargeStage = ewarpChargeStage;
                    }
                    PLServer.Instance.QueueRPC("NetworkToggleWarpCharge", PhotonTargets.MasterClient, new object[]
                    {
                            __instance.MyScreenHubBase.OptionalShipInfo.ShipID,
                            (int)ewarpChargeStage
                    });
                }
            }
            return false;
        }
    }
}
