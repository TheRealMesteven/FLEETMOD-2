using System;
using PulsarModLoader;
using PulsarModLoader.Utilities;
using UnityEngine;

namespace FLEETMOD_2.ModMessages
{
    internal class FleetModClient : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            PLPlayer pLPlayer = PLServer.GetPlayerForPhotonPlayer(sender.sender);
            if (Global.ModEnabled && pLPlayer != null && !Global.FleetModClients.Contains(pLPlayer.GetPlayerID()))
            {
                Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, $"{pLPlayer.GetPlayerName()} has been added to the FleetModClientList");
                Global.FleetModClients.Add(pLPlayer.GetPlayerID());
                Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, $"{Global.FleetModClients.Count}");
                ModMessage.SendRPC("Mest.Fleetmod", "FLEETMOD_2.ModMessages.FleetModClientSync", PhotonTargets.All, new object[] { Global.FleetShips, Global.FleetModClients });
            }
        }
    }
}
