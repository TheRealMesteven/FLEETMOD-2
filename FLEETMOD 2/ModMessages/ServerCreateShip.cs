using HarmonyLib;
using PulsarModLoader;
using PulsarModLoader.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace FLEETMOD_2.ModMessages
{
    internal class ServerCreateShip : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (!Global.ModEnabled || !PhotonNetwork.isMasterClient) return;

            // Spawn Ship from Prefabs
            PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID((int)arguments[1]);
            GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/" + PLGlobal.Instance.PlayerShipNetworkPrefabNames[(int)arguments[0]], new Vector3(50f, 50f, 50f), Quaternion.identity, 0, null);
            int ShipID = PLServer.ServerSpaceTargetIDCounter++;
            gameObject.GetComponent<PLShipInfo>().SetShipID(ShipID);
            ShipInfo shipInfo = new ShipInfo(gameObject.GetComponent<PLShipInfo>().ShipID, new List<int>());
            Global.FleetShips.Add(shipInfo);
            gameObject.GetComponent<PLShipInfo>().AutoTarget = false;
            gameObject.GetComponent<PLShipInfo>().TeamID = 1;
            gameObject.GetComponent<PLShipInfo>().OnIsNewStartingShip();
            gameObject.GetComponent<PLShipInfo>().ShipNameValue = (string)arguments[2];
            gameObject.GetComponent<PLShipInfo>().LastAIAutoYellowAlertSetupTime = Time.time;
            gameObject.GetComponent<PLShipInfo>().SetupShipStats(false, true);

            // Allow Admiral to spawn ships with Menu without becoming new Captain
            if (playerFromPlayerID != PLNetworkManager.Instance.LocalPlayer)
            {
                ModMessage.SendRPC(Mod.HarmonyIdent, "FLEETMOD_2.ModMessages.SetPlayerAsShip", PhotonTargets.MasterClient, new object[]
                {
                    PLNetworkManager.Instance.LocalPlayer.GetPlayerID(),
                    ShipID,
                    0
                });
            }
            else
            {
                // FleetShipSync occurs in SetPlayerAsShip so no need to call twice
                ModMessage.SendRPC(Mod.HarmonyIdent, "FLEETMOD_2.ModMessages.FleetShipSync", PhotonTargets.Others, Global.SerializeFleetShips(Global.FleetShips).Cast<object>().ToArray());
            }
            PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
            {
                "The " + (string)arguments[2] + " Has Joined!",
                Color.green,
                0,
                "SHIP"
            });
        }
    }
}

