using HarmonyLib;
using System.Collections.Generic;

namespace FLEETMOD_2.Core.Start
{
	[HarmonyPatch(typeof(PLServer), "SpawnPlayerShip")]
	internal class SpawnPlayerShip
	{
		public static void Postfix()
		{
			Global.FleetShips.Add(new ShipInfo(PLEncounterManager.Instance.PlayerShip.ShipID, new List<int>()));
		}
	}
	[HarmonyPatch(typeof(PLServer), "SpawnPlayerShipFromSaveData")]
	internal class SpawnPlayerShipFromSaveData
	{
		public static void Postfix()
		{
			Global.FleetShips.Add(new ShipInfo(PLEncounterManager.Instance.PlayerShip.ShipID, new List<int>()));
		}
	}
}
