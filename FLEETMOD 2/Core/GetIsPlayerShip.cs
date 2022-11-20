using HarmonyLib;
using PulsarModLoader.Utilities;

namespace FLEETMOD_2.Core
{
	[HarmonyPatch(typeof(PLShipInfoBase), "GetIsPlayerShip")]
	internal class GetIsPlayerShipPatch
	{
		public static bool Prefix(PLShipInfoBase __instance, ref bool __result)
		{
			if (!Global.ModEnabled)
			{
				return true;
			}
			else
			{
				if (Global.GetFleetShips().Contains(__instance.ShipID))
				{
					Logger.Info($"[FM2DB] {__instance.ShipNameValue} configured as FRIENDLY ship");
					__result = true;
					return false;
				}
				else
				{
					Logger.Info($"[FM2DB] {__instance.ShipNameValue} configured as HOSTILE ship");
					return true;
				}
			}
		}
	}
}
