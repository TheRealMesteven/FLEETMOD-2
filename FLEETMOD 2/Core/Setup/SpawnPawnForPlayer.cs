using HarmonyLib;
using PulsarModLoader;
using PulsarModLoader.Utilities;
using System.Linq;

namespace FLEETMOD_2.Core.Setup
{
    [HarmonyPatch(typeof(PLPlayer), "SpawnPawnForPlayer")]
    internal class SpawnPawnForPlayer
    {
        public static void Postfix()
        {
            if (!Global.ModEnabled) return;
            // Update Admiral on Player having FleetMod
            ModMessage.SendRPC(Mod.HarmonyIdent, "FLEETMOD_2.ModMessages.FleetModClient", PhotonTargets.MasterClient, new object[] { });
        }
    }
    [HarmonyPatch(typeof(PLServer), "StartPlayer")]
    internal class StartPlayer
    {
        public static void Postfix(PLServer __instance, int inID)
        {
            if (!Global.ModEnabled) return;
            // Adds Spawning Player to Fleet and Update Crew
            PLPlayer playerAtID = PLServer.Instance.GetPlayerFromPlayerID(inID);
            if (playerAtID != null && Global.FleetModSpawningPlayers.ContainsKey(inID))
            {
                ShipInfo shipInfo = Global.FleetShips[Global.GetFleetShipIndex(Global.FleetModSpawningPlayers[inID])];
                shipInfo.Crew.Add(inID);
                ModMessage.SendRPC(Mod.HarmonyIdent, "FLEETMOD_2.ModMessages.FleetShipSync", PhotonTargets.Others, Global.SerializeFleetShips(Global.FleetShips).Cast<object>().ToArray());
            }

        }
    }
}
