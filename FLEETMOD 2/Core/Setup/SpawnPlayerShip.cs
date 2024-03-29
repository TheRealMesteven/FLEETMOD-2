﻿using HarmonyLib;
using System.Collections.Generic;
using PulsarModLoader.Utilities;
using PulsarModLoader;
using System.Linq;

namespace FLEETMOD_2.Core.Setup
{
	[HarmonyPatch(typeof(PLServer), "SpawnPlayerShip")]
	internal class SpawnPlayerShip
	{
		public static void Postfix()
		{
			if (!Global.ModEnabled) return;
			// Initial Spawning Ship Adds to Fleet Crew
			Global.FleetShips.Add(new ShipInfo(PLEncounterManager.Instance.PlayerShip.ShipID, new List<int>()));
			ModMessage.SendRPC(Mod.HarmonyIdent, "FLEETMOD_2.ModMessages.FleetShipSync", PhotonTargets.Others, Global.SerializeFleetShips(Global.FleetShips).Cast<object>().ToArray());
		}
	}
	[HarmonyPatch(typeof(PLServer), "SpawnPlayerShipFromSaveData")]
	internal class SpawnPlayerShipFromSaveData
	{
		public static void Postfix()
		{
			if (!Global.ModEnabled) return;
			// Initial Spawning Ship Adds to Fleet Crew
			Global.FleetShips.Add(new ShipInfo(PLEncounterManager.Instance.PlayerShip.ShipID, new List<int>()));
			ModMessage.SendRPC(Mod.HarmonyIdent, "FLEETMOD_2.ModMessages.FleetShipSync", PhotonTargets.Others, Global.SerializeFleetShips(Global.FleetShips).Cast<object>().ToArray());
		}
	}
}
