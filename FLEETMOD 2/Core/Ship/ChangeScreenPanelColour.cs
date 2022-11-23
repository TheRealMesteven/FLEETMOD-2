using HarmonyLib;
using UnityEngine;

namespace FLEETMOD_2.Core.Ship
{
	[HarmonyPatch(typeof(PLUIScreen), "TargetRootPanelColor")]
	internal class ChangeScreenPanelColour
	{
		private static bool Prefix(PLUIScreen __instance, ref Color __result)
		{
			if (__instance.MyScreenHubBase.OptionalShipInfo != null)
			{
				if (__instance.MyScreenHubBase.OptionalShipInfo.ShipTypeID == EShipType.OLDWARS_HUMAN)
				{
					__result = Color.green;
				}
				if (__instance.MyScreenHubBase.OptionalShipInfo.TeamID == 0 || __instance.MyScreenHubBase.OptionalShipInfo.GetIsPlayerShip() || __instance.MyScreenHubBase.OptionalShipInfo.ShipTypeID == EShipType.E_ACADEMY)
				{
					if (__instance.MyScreenHubBase.OptionalShipInfo.FactionID == 3)
					{
						__result = PLGlobal.Instance.Galaxy.FactionColors[3];
					}
					__result = ChangeScreenPanelColour.UI_White;
				}
				else
				{
					if (__instance.MyScreenHubBase.OptionalShipInfo.TeamID == -1)
					{
						__result = ChangeScreenPanelColour.UI_Yellow;
					}
					__result = ChangeScreenPanelColour.UI_Red;
				}
			}
			else
			{
				if (PLServer.GetCurrentSector() != null && PLServer.GetCurrentSector().VisualIndication == ESectorVisualIndication.TOPSEC)
				{
					__result = Color.green;
				}
				__result = ChangeScreenPanelColour.UI_White;
			}
			return false;
		}
		private static Color UI_White = new Color(0.65f, 0.65f, 0.65f);
		private static Color UI_Yellow = new Color(0.7f, 0.7f, 0.1f, 1f);
		private static Color UI_Red = new Color(0.8f, 0f, 0f, 1f);
	}
}
