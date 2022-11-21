using PulsarModLoader;
using System.Linq;
using PulsarModLoader.Utilities;

namespace FLEETMOD_2.ModMessages
{
    internal class FleetModClientSync : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (!Global.ModEnabled || PhotonNetwork.isMasterClient) return;
            Global.FleetModClients = (arguments.Cast<int>()).ToList();
        }
    }
}
