using HarmonyLib;
using PulsarModLoader.Patches;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace FLEETMOD_2.Core.Ship
{
    [HarmonyPatch(typeof(PLSpaceScrap), "Update")]
    internal class ScrapPickup
    {
		// Replaces Playership with a method that calculates the closest ship.
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
			return HarmonyHelpers.PatchBySequence(instructions, new CodeInstruction[]
			{
				new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(PLEncounterManager), "Instance")),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLLevelSync), "PlayerShip")),
				new CodeInstruction(OpCodes.Stloc_0, null)
			}, 
			new CodeInstruction[]
			{
				new CodeInstruction(OpCodes.Ldarg_0, null),
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ScrapPickupReplace), "CalculateNearestShip", null, null)),
				new CodeInstruction(OpCodes.Stloc_0, null)
			}, HarmonyHelpers.PatchMode.AFTER, HarmonyHelpers.CheckMode.ALWAYS, false);
		}
    }

	[HarmonyPatch(typeof(PLSpaceScrap), "OnCollect")]
	internal class ScrapPickupCollect
	{
		// Replaces Playership with a method that calculates the closest ship.
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			List<CodeInstruction> targetSequence = new List<CodeInstruction>
			{
				new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(PLEncounterManager), "Instance")),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLLevelSync), "PlayerShip"))
			};
			List<CodeInstruction> patchSequence = new List<CodeInstruction>
			{
				new CodeInstruction(OpCodes.Ldarg_0, null),
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ScrapPickupReplace), "CalculateNearestShip", null, null))
			};
			return HarmonyHelpers.PatchBySequence(instructions, targetSequence, patchSequence, HarmonyHelpers.PatchMode.REPLACE, HarmonyHelpers.CheckMode.ALWAYS, false);
		}
	}
	public class ScrapPickupReplace
    {
		// Changes PlayerShip -> Nearest ship for pickup calculations.
		public static PLShipInfo CalculateNearestShip(PLSpaceScrap scrap)
        {
			if (!Global.ModEnabled) return PLEncounterManager.Instance.PlayerShip;
			if (scrap != null)
			{
				Vector3 position = scrap.transform.position;
				float Closest = float.MaxValue;
				PLShipInfo NearestShip = PLEncounterManager.Instance.PlayerShip;
				foreach (PLShipInfoBase pLShipInfoBase in PLEncounterManager.Instance.AllShips.Values)
				{
					PLShipInfo pLShipInfo = (PLShipInfo)pLShipInfoBase;
					if (pLShipInfo != null && !pLShipInfo.IsDrone)
					{
						float Distance = Vector3.Distance(pLShipInfo.GetSpaceLoc(), position);
						if (Closest > Distance)
						{
							Closest = Distance;
							NearestShip = pLShipInfo;
						}
					}
				}
				return NearestShip;
			}
			else
			{
				return PLEncounterManager.Instance.PlayerShip;
			}
		}
    }
}
