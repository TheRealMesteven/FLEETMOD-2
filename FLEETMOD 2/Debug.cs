using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using PulsarModLoader.Utilities;
using System.Linq;
using PulsarModLoader;
using System.Text;
using PulsarModLoader.Chat.Commands.CommandRouter;

namespace FLEETMOD_2
{
    internal class Debug
    {
        [HarmonyPatch(typeof(PLServer), "Update")]
        internal class Update
        {
            // Debug Keybinds
            public static void Postfix(PLServer __instance)
            {
                if (__instance != null && __instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLEncounterManager.Instance.PlayerShip != null)
                {
                    PLPlayer Player = PLNetworkManager.Instance.LocalPlayer;

                    // F1 = Spawn Fleet Ship (Intrepid)
                    if (!PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.F1))
                    {
                        GameObject gameObject = PhotonNetwork.Instantiate("NetworkPrefabs/" + PLGlobal.Instance.PlayerShipNetworkPrefabNames[0], new Vector3(50f, 50f, 50f), Quaternion.identity, 0, null);
                        gameObject.GetComponent<PLShipInfo>().SetShipID(PLServer.ServerSpaceTargetIDCounter++);
                        ShipInfo shipInfo = new ShipInfo(gameObject.GetComponent<PLShipInfo>().ShipID, new List<int>());
                        Global.FleetShips.Add(shipInfo);
                        gameObject.GetComponent<PLShipInfo>().AutoTarget = false;
                        gameObject.GetComponent<PLShipInfo>().TeamID = 1;
                        gameObject.GetComponent<PLShipInfo>().OnIsNewStartingShip();
                        gameObject.GetComponent<PLShipInfo>().ShipNameValue = $"Test Ship {Random.Range(0, 50)}";
                        gameObject.GetComponent<PLShipInfo>().LastAIAutoYellowAlertSetupTime = Time.time;
                        gameObject.GetComponent<PLShipInfo>().SetupShipStats(false, true);
                        ModMessage.SendRPC(Mod.HarmonyIdent, "FLEETMOD_2.ModMessages.FleetShipSync", PhotonTargets.Others, Global.SerializeFleetShips(Global.FleetShips).Cast<object>().ToArray());
                    }

                    // F2 = Get Count of Fleetships & If current Ship is Fleetship
                    if (!PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.F2))
                    {
                        Messaging.Echo(Player, $"{Global.GetFleetShips().Count} \n {Player.MyCurrentTLI.MyShipInfo.ShipNameValue} {(Global.GetFleetShips().Contains(Player.MyCurrentTLI.MyShipInfo.ShipID) ? "IS" : "IS NOT")} part of the Fleet");
                    }

                    // F3 = Get Count of Fleetmodded Clients
                    if (!PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.F3))
                    {
                        Messaging.Echo(Player, $"Count of Fleetmod Clients: {Global.FleetModClients.Count}");
                    }

                    // F4 = Get Fleetship Names & Crew List
                    if (!PLNetworkManager.Instance.IsTyping && Input.GetKeyDown(KeyCode.F4))
                    {
                        StringBuilder Sb = new StringBuilder();
                        foreach (ShipInfo shipInfo in Global.FleetShips)
                        {
                            if (shipInfo != null)
                            {
                                Sb.AppendLine($"{PLEncounterManager.Instance.GetShipFromID(shipInfo.ShipID).ShipNameValue} | {shipInfo.Crew.Count} | Crew Output: ");
                                if (shipInfo.Crew.Count > 0)
                                {
                                    foreach (int i in shipInfo.Crew)
                                    {
                                        Sb.Append($"{PLServer.Instance.GetPlayerFromPlayerID(i).GetPlayerName()} ");
                                    }
                                    Sb.Append("\n");
                                }
                            }
                        }
                        Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, Sb.ToString());
                    }
                }
            }
        }
        internal class Command
        {
            /// <summary>
            /// Turns PlayerID/Name into PlayerID
            /// </summary>
            public static int GetPlayerID(string PlayerValue)
            {
                if (int.TryParse(PlayerValue, out int PlayerID))
                {
                    return PlayerID;
                }
                else
                {
                    int PlayerID2 = -1;
                    foreach (PLPlayer pLPlayer in PLServer.Instance.AllPlayers)
                    {
                        if (pLPlayer != null)
                        {
                            if (pLPlayer.GetPlayerName().ToLower() == PlayerValue.ToLower())
                            {
                                return pLPlayer.GetPlayerID();
                            }
                            else
                            {
                                if (pLPlayer.GetPlayerName().ToLower().Contains(PlayerValue.ToLower()))
                                {
                                    PlayerID2 = pLPlayer.GetPlayerID();
                                }
                            }
                        }
                    }
                    return PlayerID2;
                }
            }

            /// <summary>
            /// Turns ShipID/Name into ShipID
            /// </summary>
            public static int GetShipID(string ShipValue)
            {
                if (int.TryParse(ShipValue, out int ShipID))
                {
                    return ShipID;
                }
                else
                {
                    int ShipID2 = -1;
                    foreach (PLShipInfoBase pLShipInfoBase in PLEncounterManager.Instance.AllShips.Values)
                    {
                        if (pLShipInfoBase != null)
                        {
                            if (pLShipInfoBase.ShipNameValue.ToLower() == ShipValue.ToLower())
                            {
                                return pLShipInfoBase.ShipID;
                            }
                            else
                            {
                                if (pLShipInfoBase.ShipNameValue.ToLower().Contains(ShipValue.ToLower()))
                                {
                                    ShipID2 = pLShipInfoBase.ShipID;
                                }
                            }
                        }
                    }
                    return ShipID2;
                }
            }

            /// <summary>
            /// Turns ClassID/Name into ClassID
            /// </summary>
            public static int GetClassID(string ClassValue)
            {
                if (int.TryParse(ClassValue, out int ClassID))
                {
                    return ClassID;
                }
                else
                {
                    switch (ClassValue)
                    {
                        case "c":
                            return 0;
                        case "p":
                            return 1;
                        case "s":
                            return 2;
                        case "w":
                            return 3;
                        case "e":
                            return 4;
                        default:
                            return -1;
                    }
                }
            }

            /// <summary>
            /// Command to Change Class & Ship ([PlayerID/Name] [ShipID/Name] [ClassID/Name])
            /// </summary>
            public class ChangeClass : ChatCommand
            {
                public override string[] CommandAliases() => new string[] { "fmcc" };
                public override string Description() => "Debug command to change class";
                public string UsageExample() => $"/{this.CommandAliases()[0]} [PlayerID/Name] [ShipID/Name] [ClassID/Name]";
                public override void Execute(string arguments)
                {
                    string[] args = arguments.Split(' ');
                    if (args.Count() < 3)
                    {
                        Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, $"Usage Example: {UsageExample()}");
                    }
                    if (!Global.ModEnabled) return;
                    int PlayerID = Command.GetPlayerID(args[0]);
                    int ShipID = Command.GetShipID(args[1]);
                    int ClassID = Command.GetClassID(args[2]);
                    if (PlayerID == -1 || ShipID == -1 || ClassID == -1)
                    {
                        Messaging.Notification($"The Player {PlayerID} / Ship {ShipID} / Class {ClassID} is invalid (-1)", PLServer.Instance.GetPlayerFromPlayerID(PlayerID));
                        return;
                    }
                    ModMessage.SendRPC(Mod.HarmonyIdent, "FLEETMOD_2.ModMessages.SetPlayerAsShip", PhotonTargets.MasterClient, new object[]
                    {
                        PlayerID,
                        ShipID,
                        ClassID
                    });
                }
            }
        }
    }
}
