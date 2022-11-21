using HarmonyLib;
using System.Collections.Generic;
using PulsarModLoader.Utilities;

namespace FLEETMOD_2.Core.Start
{
    [HarmonyPatch(typeof(PLServer), "Start")]
    class Start
    {
        static void Postfix()
        {
            Global.FleetShips.Clear();
            Global.FleetModClients.Clear();
        }
    }
}
