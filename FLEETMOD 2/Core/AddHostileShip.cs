﻿using HarmonyLib;

namespace FLEETMOD_2.Core
{
	[HarmonyPatch(typeof(PLShipInfoBase), "AddHostileShip")]
	internal class AddHostileShip
	{
		public static bool Prefix(PLShipInfoBase __instance, PLShipInfoBase inShip, ref bool ___HostileShipAdded_NeedsResetForTargeting)
		{
			if (!Global.ModEnabled)
			{
				return true;
			}
			else
			{
				if (inShip != null && __instance != null && inShip != __instance && !__instance.HostileShips.Contains(inShip.ShipID) && !(inShip.GetIsPlayerShip() && __instance.GetIsPlayerShip()))
				{
					__instance.HostileShips.Add(inShip.ShipID);
					___HostileShipAdded_NeedsResetForTargeting = true;
				}
				return false;
			}
		}
	}
}
