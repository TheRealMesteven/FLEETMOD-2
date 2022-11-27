using HarmonyLib;
using PulsarModLoader;
using PulsarModLoader.Utilities;
using System.Linq;
using System.Reflection;

namespace FLEETMOD_2.ModMessages
{
    internal class SetPlayerAsShip : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (!Global.ModEnabled || !PhotonNetwork.isMasterClient) return;
            if (int.TryParse(arguments[0].ToString(), out int PlayerID) && int.TryParse(arguments[1].ToString(), out int ShipID) && int.TryParse(arguments[2].ToString(), out int ClassID)){
                ShipInfo NewShip = Global.FleetShips[Global.GetFleetShipIndex(ShipID)];
                ShipInfo OldShip = Global.FleetShips[Global.GetFleetShipIndex(Global.GetPlayersShip(PlayerID))];
                if (NewShip != null && OldShip != null && NewShip.CanJoinClass(ClassID))
                {
                    NewShip.Crew.Add(PlayerID);
                    OldShip.Crew.Remove(PlayerID);
                    MethodInfo methodInfo = AccessTools.Method(PLServer.Instance.GetType(), "SetPlayerAsClassID", null, null);
                    methodInfo.Invoke(PLServer.Instance, new object[]
                    {
                        PlayerID,
                        ClassID,
                        sender
                    });
                    ModMessage.SendRPC(Mod.HarmonyIdent, "FLEETMOD_2.ModMessages.FleetShipSync", PhotonTargets.Others, Global.SerializeFleetShips(Global.FleetShips).Cast<object>().ToArray());
                }
                else
                {
                    Messaging.Centerprint("The slot on that ship is full, choose another one or another ship.", PLServer.Instance.GetPlayerFromPlayerID(PlayerID), "ROL", PLPlayer.GetClassColorFromID(ClassID), EWarningType.E_NORMAL);
                    Messaging.Notification($"Player {PLServer.Instance.GetPlayerFromPlayerID(PlayerID).GetPlayerName(false)} Is trying to join as {PLPlayer.GetClassNameFromID(ClassID)} on {PLEncounterManager.Instance.GetShipFromID(ShipID).ShipNameValue}", PLNetworkManager.Instance.LocalPlayer, 0, 6000, false);
                }
            }
        }
    }
}

