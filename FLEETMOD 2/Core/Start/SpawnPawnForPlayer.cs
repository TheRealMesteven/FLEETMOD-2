using HarmonyLib;
using PulsarModLoader;
using PulsarModLoader.Utilities;

namespace FLEETMOD_2.Core.Start
{
	[HarmonyPatch(typeof(PLPlayer), "SpawnPawnForPlayer")]
	internal class SpawnPawnForPlayer
	{
		public static void Postfix()
		{
			if (!Global.ModEnabled /*|| PhotonNetwork.isMasterClient*/) return;
			ModMessage.SendRPC("Mest.Fleetmod", "FLEETMOD_2.ModMessages.FleetModClient", PhotonTargets.MasterClient, new object[] { });
		}
	}
	/*
	[HarmonyPatch(typeof(PLServer), "StartPlayer")]
	internal class StartPlayer
	{
		public static void Postfix()
		{
			if (!Global.ModEnabled) return;
			Messaging.Echo(PLNetworkManager.Instance.LocalPlayer, "HELLO");
		}
	}
	*/
}
