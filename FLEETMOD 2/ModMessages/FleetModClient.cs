using System;
using System.Linq;
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
                Global.FleetModClients.Add(pLPlayer.GetPlayerID());
                ModMessage.SendRPC("Mest.Fleetmod", "FLEETMOD_2.ModMessages.FleetShipSync", PhotonTargets.All, Global.SerializeFleetShips(Global.FleetShips).Cast<object>().ToArray());
                ModMessage.SendRPC("Mest.Fleetmod", "FLEETMOD_2.ModMessages.FleetModClientSync", PhotonTargets.All, Global.FleetModClients.ToArray().Cast<object>().ToArray());
            }
        }
    }
}
