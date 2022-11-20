using PulsarModLoader;
using System.Collections.Generic;
using PulsarModLoader.Utilities;

namespace FLEETMOD_2.ModMessages
{
    internal class FleetModClientSync : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, $"Recieved FleetModClientSync");
            Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, $"{Global.ModEnabled}");
            Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, $"{!PhotonNetwork.isMasterClient}");
            Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, $"{sender.sender.IsMasterClient}");
            if (Global.ModEnabled && !PhotonNetwork.isMasterClient && sender.sender.IsMasterClient)
            {
                Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, $"Check Passed");
                Global.FleetShips = (List<ShipInfo>)arguments[0];
                Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, $"{Global.FleetShips.Count} = Fleet Ship Count");
                Global.FleetModClients = (List<int>)arguments[1];
                Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, $"{Global.FleetModClients.Count} = Fleet Client Count");
            }
        }
    }
}
