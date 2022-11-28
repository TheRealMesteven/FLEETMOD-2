using HarmonyLib;
using Photon;
using PulsarModLoader;
using PulsarModLoader.Patches;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace FLEETMOD_2.Core.Ship
{
	[HarmonyPatch(typeof(PLShipInfo), "UpdateSensorDish")]
	internal class SensorDish
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			return HarmonyHelpers.PatchBySequence(instructions, new CodeInstruction[]
			{
				new CodeInstruction(OpCodes.Ldarg_0, null),
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MonoBehaviour), "get_photonView", null, null)),
				new CodeInstruction(OpCodes.Ldstr, "RequestScrapCollectFromSensorDish"),
				new CodeInstruction(OpCodes.Ldc_I4_2, null),
				new CodeInstruction(OpCodes.Ldc_I4_1, null),
				new CodeInstruction(OpCodes.Newarr, typeof(object)),
				new CodeInstruction(OpCodes.Dup, null),
				new CodeInstruction(OpCodes.Ldc_I4_0, null),
				new CodeInstruction(OpCodes.Ldarg_0, null),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfo), "SensorDishCurrentSecondaryTarget_Scrap")),
				new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSpecialEncounterNetObject), "get_EncounterNetID", null, null)),
				new CodeInstruction(OpCodes.Box, typeof(int)),
				new CodeInstruction(OpCodes.Stelem_Ref, null),
				new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PhotonView), "RPC", new Type[]
				{
					typeof(string),
					typeof(PhotonTargets),
					typeof(object[])
				}, null))
			}, 
			new CodeInstruction[]
			{
				new CodeInstruction(OpCodes.Ldarg_0, null),
				new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfo), "SensorDishCurrentSecondaryTarget_Scrap")),
				new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLSpecialEncounterNetObject), "get_EncounterNetID", null, null)),
				new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SensorDish), "Fix", null, null))
			}, 
			HarmonyHelpers.PatchMode.REPLACE, HarmonyHelpers.CheckMode.ALWAYS, false);
		}

		public static void Fix(int NetID)
		{
			if (!Global.ModEnabled)
            {
				PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.photonView.RPC("RequestScrapCollectFromSensorDish", PhotonTargets.MasterClient, new object[]
				{
					NetID
				});
			}
			ModMessage.SendRPC(Mod.HarmonyIdent, "FLEETMOD_2.ModMessages.SensorDishCollectScrap", PhotonTargets.Others, new object[]
			{
				NetID,
				PLNetworkManager.Instance.LocalPlayer.GetPawn().CurrentShip.ShipID
			});
		}
	}
}
