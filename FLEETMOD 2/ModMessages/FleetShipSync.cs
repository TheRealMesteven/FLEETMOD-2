using PulsarModLoader;
using System.Collections.Generic;
using PulsarModLoader.Utilities;
using System;
using System.Linq;

namespace FLEETMOD_2.ModMessages
{
    internal class FleetShipSync : ModMessage
    {
        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if (!Global.ModEnabled) return;
            List<ShipInfo> shipInfo = Global.DeSerializeFleetShips(arguments.Cast<byte>().ToArray());
            if (shipInfo == null || shipInfo.Count() == 0) return;
            if (Global.FleetShips == null || shipInfo.Count >= Global.FleetShips.Count())
            {
                Global.FleetShips = shipInfo;
            }
            else
            {
                foreach (ShipInfo shipinfo in shipInfo)
                {
                    Global.UpdateFleetShips(shipinfo.ShipID, shipinfo);
                }
            }
        }
    }
}
