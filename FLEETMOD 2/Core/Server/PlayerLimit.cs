using ExitGames.Client.Photon;
using HarmonyLib;
using PulsarModLoader.Utilities;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FLEETMOD_2.Core.Server
{
    [HarmonyPatch(typeof(PLServer), "Update")]
    internal class PlayerLimitOverride
    {
        // Overrides the PlayerLimit to scale with the Total Sum of the Role Limits
        public static void Postfix(PLServer __instance)
        {
            if (!Global.ModEnabled || !PhotonNetwork.isMasterClient
            || __instance == null || !__instance.GameHasStarted || PLNetworkManager.Instance.LocalPlayer == null || !PLNetworkManager.Instance.LocalPlayer.GetHasStarted() || PLEncounterManager.Instance.PlayerShip == null) return;

            // Recreate the normal Hashtable data
            if (Time.time - __instance.LobbyPropertiesUpdateLastTime > 0.5f && PhotonNetwork.room != null)
            {
                __instance.LobbyPropertiesUpdateLastTime = Time.time;
                int num = 0;
                for (int i = 0; i < __instance.AllPlayers.Count; i++)
                {
                    PLPlayer plplayer = __instance.AllPlayers[i];
                    if (plplayer != null && plplayer.IsBot && plplayer.TeamID == 0)
                    {
                        num++;
                    }
                }
                Hashtable hashtable = new Hashtable();
                hashtable.Add("CurrentPlayersPlusBots", PhotonNetwork.room.PlayerCount + num);
                hashtable.Add("Private", PLNetworkManager.Instance.IsPrivateGame);
                if (PLGlobal.Instance.Galaxy != null && PLGlobal.Instance.Galaxy.GenerationSettings != null)
                {
                    hashtable.Add("GenSettings", PLGlobal.Instance.Galaxy.GenerationSettings.CreateDataString());
                }
                if (PLEncounterManager.Instance.PlayerShip != null)
                {
                    hashtable.Add("Ship_Name", PLEncounterManager.Instance.PlayerShip.ShipNameValue);
                    hashtable.Add("Ship_Type", Mod.myversion);
                }
                else
                {
                    hashtable.Add("Ship_Name", PhotonNetwork.room.CustomProperties["Ship_Name"]);
                    hashtable.Add("Ship_Type", PhotonNetwork.room.CustomProperties["Ship_Type"]);
                }
                if (PhotonNetwork.room.CustomProperties.ContainsKey("SteamServerID"))
                {
                    hashtable.Add("SteamServerID", PhotonNetwork.room.CustomProperties["SteamServerID"]);
                    CSteamID steamID = SteamUser.GetSteamID();
                    string text = steamID.m_SteamID.ToString() + ",";
                    foreach (PhotonPlayer photonPlayer in PhotonNetwork.playerList)
                    {
                        if (photonPlayer != null && photonPlayer.SteamID != CSteamID.Nil && photonPlayer.SteamID != steamID)
                        {
                            text += photonPlayer.SteamID.m_SteamID.ToString() + ",";
                        }
                    }
                    hashtable.Add("PlayerSteamIDs", text);
                }
                bool flag = hashtable.Count != PhotonNetwork.room.CustomProperties.Count;
                if (!flag)
                {
                    foreach (object obj in hashtable.Keys)
                    {
                        string text2 = (string)obj;
                        if (PhotonNetwork.room.CustomProperties[text2].ToString() != hashtable[text2].ToString())
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (flag)
                {
                    PhotonNetwork.room.SetCustomProperties(hashtable, null, false);
                }
            }

            // Iterate over the FleetShip variables to get the total Crew Count and replace the MaxPlayer count
            if (Time.time - PLServer.Instance.LobbyPropertiesUpdateLastTime > 0.2f && PhotonNetwork.room != null)
            {
                int num2 = 0;
                foreach (ShipInfo shipInfo in Global.FleetShips)
                {
                    PLShipInfoBase Ship = PLEncounterManager.Instance.GetShipFromID(shipInfo.ShipID);
                    if (Ship != null && Ship.GetIsPlayerShip())
                    {
                        num2 += shipInfo.RoleLimits.Sum();
                    }
                }
                PhotonNetwork.room.MaxPlayers = num2;
            }
        }
    }
    [HarmonyPatch(typeof(PLServer), "SetPlayerAsClassID")]
    internal class ShipClassLimitOverride
    {
        // Override the RPC "SetPlayerAsClassID" to allow multiple players in one Class type.
        private static bool Prefix(PLServer __instance, ref int playerID, ref int classID, ref List<PLPlayer> ___LocalCachedPlayerByClass)
        {
            PLPlayer Player = PLNetworkManager.Instance.LocalPlayer;
            if (!PhotonNetwork.isMasterClient || !Global.ModEnabled) return true;

            if (classID > 4 || classID < -1) return false;
            PLPlayer playerFromPlayerID = PLServer.Instance.GetPlayerFromPlayerID(playerID);
            if (playerFromPlayerID != null)
            {
                // If the Player hasnt started, they've just joined and have no ship.
                if (!playerFromPlayerID.GetHasStarted())
                {
                    bool flag = false;
                    // Iterate over all Fleet Ships looking for an Available Class
                    foreach (ShipInfo shipInfo in Global.FleetShips)
                    {
                        if (shipInfo.CanJoinClass(classID) && playerFromPlayerID.GetClassID() != classID)
                        {
                            playerFromPlayerID.SetClassID(classID);
                            MethodInfo methodInfo = AccessTools.Method(__instance.GetType(), "ClassChangeMessage", null, null);
                            methodInfo.Invoke(__instance, new object[]
                            {
                                playerFromPlayerID.GetPlayerName(false),
                                classID
                            });
                            // When spawning the Player, they need to know what Ship they should be assigned to.
                            if (Global.FleetModSpawningPlayers.ContainsKey(playerID))
                            {
                                Global.FleetModSpawningPlayers[playerID] = shipInfo.ShipID;
                            }
                            else
                            {
                                Global.FleetModSpawningPlayers.Add(playerID, shipInfo.ShipID);
                            }
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        Messaging.Centerprint("That slot is full, choose another one.", playerFromPlayerID, "ROL", PLPlayer.GetClassColorFromID(classID), EWarningType.E_NORMAL);
                        Messaging.Notification("Player " + playerFromPlayerID.GetPlayerName(false) + " Is trying to join as " + PLPlayer.GetClassNameFromID(classID), PLNetworkManager.Instance.LocalPlayer, 0, 6000, false);
                    }
                }
                // Else, Player has an existing ship to look for available role with.
                // (Players with a ship already use a different Mod Message to change Ship & Class)
                else
                {
                    int PlayerShip = Global.GetPlayersShip(playerID);
                    if (PlayerShip != -1)
                    {
                        ShipInfo shipInfo = Global.FleetShips[Global.GetFleetShipIndex(PlayerShip)];
                        if (shipInfo.CanJoinClass(classID) && playerFromPlayerID.GetClassID() != classID)
                        {
                            playerFromPlayerID.SetClassID(classID);
                            MethodInfo methodInfo = AccessTools.Method(__instance.GetType(), "ClassChangeMessage", null, null);
                            methodInfo.Invoke(__instance, new object[]
                            {
                                playerFromPlayerID.GetPlayerName(false),
                                classID
                            });
                        }
                        else
                        {
                            Messaging.Centerprint("That slot is full, choose another one or another ship.", playerFromPlayerID, "ROL", PLPlayer.GetClassColorFromID(classID), EWarningType.E_NORMAL);
                            Messaging.Notification($"Player {playerFromPlayerID.GetPlayerName(false)} Is trying to join as {PLPlayer.GetClassNameFromID(classID)} on {PLEncounterManager.Instance.GetShipFromID(PlayerShip).ShipNameValue}", PLNetworkManager.Instance.LocalPlayer, 0, 6000, false);
                        }
                    }
                }
            }
            return false;
        }
    }
}
