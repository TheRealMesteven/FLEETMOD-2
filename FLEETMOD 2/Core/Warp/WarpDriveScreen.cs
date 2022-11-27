﻿using System;
using HarmonyLib;
using UnityEngine;

namespace FLEETMOD_2.Core.Warp
{
    [HarmonyPatch(typeof(PLWarpDriveScreen), "Update")]
    internal class WarpDriveScreen
    {
        public static void Postfix(PLWarpDriveScreen __instance, ref UISprite ___JumpComputerPanel, ref UISprite ___WarpDrivePanel, ref UISprite ___m_BlockingTargetOnboardPanel, ref UILabel ___m_JumpButtonLabel, ref UILabel ___BlindJumpBtnLabel, ref UILabel ___BlindJumpWarning, ref UISprite ___BlindJumpBtn, ref float ___TargetAlpha_WarpPanel, ref UILabel ___m_JumpButtonLabelTop, ref UILabel ___m_BlockingTargetOnboardPanelTitle, ref UIPanel ___m_JumpButtonMask, ref UIPanel[] ___ChargeStage_BarMask, ref UILabel[] ___ChargeStage_Label, string[] ___ChargeStage_Name)
        {
            if (!Global.ModEnabled) return;
            int UnalignedShips = 0;
            int UnchargedShips = 0;
            int UnFueledShips = 0;
            string text = "";
            string str = "";
            string str2 = "";

            // Get Next Sector ID (For Non-Admiral Blind Jump Override)
            PLSectorInfo map = null;
            if (__instance != null && PLStarmap.Instance != null && PLStarmap.Instance.CurrentShipPath != null && PLStarmap.Instance.CurrentShipPath.Count > 1)
            {
                map = PLStarmap.Instance.CurrentShipPath[1];
            }

            // Get Status of Each FleetShip
            foreach (int pLShipID in Global.GetFleetShips())
            {
                PLShipInfoBase plshipInfoBase = PLEncounterManager.Instance.GetShipFromID(pLShipID);
                if (plshipInfoBase != null && PLServer.Instance.m_ShipCourseGoals.Count > 0)
                {
                    if (map != null)
                    {
                        if (plshipInfoBase.WarpTargetID != map.ID)
                        {
                            UnalignedShips++;
                            text = plshipInfoBase.ShipNameValue;
                        }
                    }
                    if (plshipInfoBase.WarpChargeStage != EWarpChargeStage.E_WCS_READY)
                    {
                        UnchargedShips++;
                        str = plshipInfoBase.ShipNameValue;
                    }
                    if (plshipInfoBase.NumberOfFuelCapsules < 1)
                    {
                        UnFueledShips++;
                        str2 = plshipInfoBase.ShipNameValue;
                    }
                }
            }

            // Default-Game Screen Visuals
            PLGlobal.SafeGameObjectSetActive(___WarpDrivePanel.gameObject, !__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard && __instance.MyScreenHubBase.OptionalShipInfo.WarpChargeStage != EWarpChargeStage.E_WCS_ACTIVE && __instance.MyScreenHubBase.OptionalShipInfo.NumberOfFuelCapsules > 0 && !__instance.MyScreenHubBase.OptionalShipInfo.InWarp && !__instance.MyScreenHubBase.OptionalShipInfo.Abandoned);
            PLGlobal.SafeGameObjectSetActive(___JumpComputerPanel.gameObject, !__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard);
            PLGlobal.SafeGameObjectSetActive(___m_BlockingTargetOnboardPanel.gameObject, __instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard);
            if (__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard)
            {
                Color b = Color.Lerp(new Color(0.8f, 0f, 0f, 1f), new Color(0.65f, 0.65f, 0.65f), Time.time % 1f);
                ___m_BlockingTargetOnboardPanelTitle.color = Color.Lerp(___m_BlockingTargetOnboardPanelTitle.color, b, Time.deltaTime * 2f);
            }
            if (__instance.MyScreenHubBase.OptionalShipInfo.BlockingCombatTargetOnboard || __instance.MyScreenHubBase.OptionalShipInfo.HasVirusOfType(EVirusType.WARP_DISABLE) || PLBeaconInfo.GetBeaconStatAdditive(EBeaconType.E_WARP_DISABLE, __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip()) > 0.5f)
            {
                ___BlindJumpBtnLabel.text = "ERROR";
                ___BlindJumpBtnLabel.color = Color.black;
                ___BlindJumpWarning.text = "ERROR: Blind Jump Unavailable!";
                ___BlindJumpBtn.spriteName = "button_fill";
            }
            else
            {
                // Fleetmod #1 Blindjump Override features.

                /*if (__instance.MyScreenHubBase.OptionalShipInfo.BlindJumpUnlocked && PhotonNetwork.isMasterClient)
                {
                    if (PhotonNetwork.isMasterClient && __instance.MyScreenHubBase.OptionalShipInfo.ShipID == PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID)
                    {
                        ___BlindJumpBtnLabel.text = "BLIND JUMP";
                        ___BlindJumpBtnLabel.color = Color.black;
                        ___BlindJumpWarning.text = "ADMIRAL - BLIND JUMP";
                        ___BlindJumpBtn.spriteName = "button_fill";
                    }
                    else
                    {
                        if (PhotonNetwork.isMasterClient && __instance.MyScreenHubBase.OptionalShipInfo.ShipID != PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID)
                        {
                            ___BlindJumpBtnLabel.text = "DESTROY SHIP";
                            ___BlindJumpBtnLabel.color = Color.black;
                            ___BlindJumpWarning.text = "For Emergency Use By The Admiral";
                            ___BlindJumpBtn.spriteName = "button_fill";
                        }
                    }
                }
                else
                {
                    if (PhotonNetwork.isMasterClient && !__instance.MyScreenHubBase.OptionalShipInfo.BlindJumpUnlocked && __instance.MyScreenHubBase.OptionalShipInfo.ShipID != PLNetworkManager.Instance.LocalPlayer.StartingShip.ShipID)
                    {
                        ___BlindJumpWarning.text = "ADMIRAL - DESTROY SHIP";
                        ___BlindJumpBtn.spriteName = "button";
                        ___BlindJumpBtnLabel.color = Color.red;
                        ___BlindJumpBtnLabel.text = "UNLOCK";
                    }
                    else
                    {
                        if (PhotonNetwork.isMasterClient && !__instance.MyScreenHubBase.OptionalShipInfo.BlindJumpUnlocked)
                        {
                            ___BlindJumpWarning.text = "ADMIRAL - BLIND JUMP";
                            ___BlindJumpBtn.spriteName = "button";
                            ___BlindJumpBtnLabel.color = Color.red;
                            ___BlindJumpBtnLabel.text = "UNLOCK";
                        }
                        else
                        {
                            if (!PhotonNetwork.isMasterClient)
                            {*/

                // Overrides Blind Jump Label with Next-Course-Target Sector Display
                ___BlindJumpWarning.text = "Target Sector";
                if (map != null && PLServer.Instance.m_ShipCourseGoals.Count > 0)
                {
                    ___BlindJumpBtnLabel.text = map.ID.ToString();
                }
                else
                {
                    ___BlindJumpBtnLabel.text = "No Course Set";
                }
                            /*}
                        }
                    }
                }*/
            }

            // Override the WarpDisplay Screen
            float warpChargePercentTotal = __instance.MyScreenHubBase.OptionalShipInfo.GetWarpChargePercentTotal();
            ___m_JumpButtonMask.clipOffset = new Vector2((warpChargePercentTotal - 1f) * ___m_JumpButtonMask.width, 0f);
            switch (__instance.MyScreenHubBase.OptionalShipInfo.WarpChargeStage)
            {
                case EWarpChargeStage.E_WCS_PREPPING:
                    ___TargetAlpha_WarpPanel = 1f;
                    ___m_JumpButtonLabel.text = "Charging Warp Drive";
                    ___m_JumpButtonLabelTop.text = "Charging Warp Drive";
                    break;
                case EWarpChargeStage.E_WCS_PAUSED:
                    ___TargetAlpha_WarpPanel = 0.75f;
                    ___m_JumpButtonLabel.text = "Jump Prep Paused";
                    ___m_JumpButtonLabelTop.text = "Jump Prep Paused";
                    break;
                case EWarpChargeStage.E_WCS_READY:
                    {
                        ___TargetAlpha_WarpPanel = 0.3f;
                        if (!__instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip())
                        {
                            ___m_JumpButtonLabel.text = "Not Responding";
                            ___m_JumpButtonLabelTop.text = "Not Responding";
                        }
                        else
                        {
                            if (UnchargedShips > 0)
                            {
                                ___m_JumpButtonLabel.text = "Prep The " + str;
                                ___m_JumpButtonLabelTop.text = "Prep The " + str;
                            }
                            else
                            {
                                if (UnFueledShips > 0)
                                {
                                    ___m_JumpButtonLabel.text = "No Fuel on the " + str2;
                                    ___m_JumpButtonLabelTop.text = "No Fuel on the " + str2;
                                }
                                else
                                {
                                    if (PLServer.Instance.m_ShipCourseGoals.Count == 0)
                                    {
                                        ___m_JumpButtonLabel.text = "Captain Has Not Set Course";
                                        ___m_JumpButtonLabelTop.text = "Captain Has Not Set Course";
                                    }
                                    else
                                    {
                                        if (__instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID != -1 && UnalignedShips == 0)
                                        {
                                            ___m_JumpButtonLabel.text = "Warp To Sector " + __instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID.ToString();
                                            ___m_JumpButtonLabelTop.text = "Warp To Sector " + __instance.MyScreenHubBase.OptionalShipInfo.WarpTargetID.ToString();
                                        }
                                        else
                                        {
                                            if (map == null || PLBeaconInfo.GetBeaconStatAdditive(EBeaconType.E_WARP_DISABLE, __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip()) > 0.5f)
                                            {
                                                ___m_JumpButtonLabel.text = "Not Responding";
                                                ___m_JumpButtonLabelTop.text = "Not Responding";
                                            }
                                            else
                                            {
                                                ___m_JumpButtonLabel.text = string.Concat(new object[]
                                        {
                                                "Align ",
                                                text,
                                                "to ",
                                                map.ID
                                        });
                                                ___m_JumpButtonLabelTop.text = string.Concat(new object[]
                                                {
                                                "Align ",
                                                text,
                                                " to ",
                                                map.ID
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    }
                case EWarpChargeStage.E_WCS_ACTIVE:
                    {
                        ___TargetAlpha_WarpPanel = 0.3f;
                        if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
                        {
                            ___m_JumpButtonLabel.text = "Skip Warp (" + __instance.MyScreenHubBase.OptionalShipInfo.GetWarpTimerString() + ")";
                            ___m_JumpButtonLabelTop.text = "Skip Warp (" + __instance.MyScreenHubBase.OptionalShipInfo.GetWarpTimerString() + ")";
                        }
                        else
                        {
                            ___m_JumpButtonLabel.text = "Jump In Progress (" + __instance.MyScreenHubBase.OptionalShipInfo.GetWarpTimerString() + ")";
                            ___m_JumpButtonLabelTop.text = "Jump In Progress (" + __instance.MyScreenHubBase.OptionalShipInfo.GetWarpTimerString() + ")";
                        }
                        break;
                    }
                default:
                    ___TargetAlpha_WarpPanel = 0.3f;
                    ___m_JumpButtonLabel.text = "Initiate Jump Prep";
                    ___m_JumpButtonLabelTop.text = "Initiate Jump Prep";
                    break;
            }
            for (int i = 0; i < PLGlobal.NumWarpChargeStages; i++)
            {
                ___ChargeStage_Label[i].text = ___ChargeStage_Name[i];
                ___ChargeStage_BarMask[i].clipOffset = new Vector2((__instance.MyScreenHubBase.OptionalShipInfo.WarpChargeState_Levels[i] - 1f) * ___ChargeStage_BarMask[i].width, 0f);
            }
        }
    }
}
