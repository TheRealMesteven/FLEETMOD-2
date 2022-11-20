﻿using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using PulsarModLoader.Utilities;

namespace FLEETMOD_2
{
    internal class Debug
    {
        [HarmonyPatch(typeof(PLServer), "Update")]
        internal class Update
        {
            public static void Postfix(PLServer __instance)
            {
                if (__instance != null && __instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLEncounterManager.Instance.PlayerShip != null)
                {
                    PLPlayer Player = PLNetworkManager.Instance.LocalPlayer;
                    if (!PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.F1))
                    {
                        GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/" + PLGlobal.Instance.PlayerShipNetworkPrefabNames[0], new Vector3(50f, 50f, 50f), Quaternion.identity, 0, null);
                        gameObject.GetComponent<PLShipInfo>().SetShipID(PLServer.ServerSpaceTargetIDCounter++);
                        Global.FleetShips.Add(new ShipInfo(gameObject.GetComponent<PLShipInfo>().ShipID, new List<int>()));
                        gameObject.GetComponent<PLShipInfo>().AutoTarget = false;
                        gameObject.GetComponent<PLShipInfo>().TeamID = 1;
                        gameObject.GetComponent<PLShipInfo>().OnIsNewStartingShip();
                        gameObject.GetComponent<PLShipInfo>().ShipNameValue = $"Test Ship {Random.Range(0, 50)}";
                        gameObject.GetComponent<PLShipInfo>().LastAIAutoYellowAlertSetupTime = Time.time;
                        gameObject.GetComponent<PLShipInfo>().SetupShipStats(false, true);
                    }
                    if (!PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.F2))
                    {
                        Messaging.Echo(Player, $"{Global.GetFleetShips().Count} \n {Player.MyCurrentTLI.MyShipInfo.ShipNameValue} {(Global.GetFleetShips().Contains(Player.MyCurrentTLI.MyShipInfo.ShipID) ? "IS" : "IS NOT")} part of the Fleet");
                    }
                }
            }
        }
    }
}
