using HarmonyLib;
using PulsarModLoader;

namespace FLEETMOD_2.Core.Start
{
	[HarmonyPatch(typeof(PLPlayer), "SpawnPawnForPlayer")]
	internal class SpawnPawnForPlayer
	{
		public static void Postfix()
		{
			if (!Global.ModEnabled) return;
			ModMessage.SendRPC("Mest.Fleetmod", "FLEETMOD_2.ModMessages.FleetModClient", PhotonTargets.MasterClient, new object[] { });
		}
	}
}
