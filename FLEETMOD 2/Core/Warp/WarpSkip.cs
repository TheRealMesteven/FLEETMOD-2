using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FLEETMOD_2.Core.Warp
{
	[HarmonyPatch(typeof(PLServer), "Update")]
	internal class WarpSkip
	{
		public static void Postfix(PLServer __instance)
		{
			if (__instance == null || PLNetworkManager.Instance == null || PLEncounterManager.Instance == null) return;
			if (Global.ModEnabled && PhotonNetwork.isMasterClient && PLEncounterManager.Instance.PlayerShip != null && PLNetworkManager.Instance.LocalPlayer.GetHasStarted() && PLServer.Instance.GameHasStarted)
			{
				if (PLServer.Instance.AllPlayersLoaded() && Mathf.Abs((float)((long)PLServer.Instance.GetEstimatedServerMs() - (long)PLEncounterManager.Instance.PlayerShip.LastBeginWarpServerTime)) > 16000f && PhotonNetwork.player.NickName != "skipwarp")
				{
					PhotonNetwork.player.NickName = "skipwarp";
				}
				if (!PLEncounterManager.Instance.PlayerShip.InWarp && PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().NickName == "skipwarp")
				{
					PhotonNetwork.player.NickName = "null";
				}
			}
		}
	}
	[HarmonyPatch(typeof(PLInGameUI), "Update")]
	internal class UpdatePLInGameUI
	{
		public static void Postfix(PLInGameUI __instance, ref List<PLPlayer> ___relevantPlayersForCrewStatus, ref PLCachedFormatString<string> ___cSkipWarpLabel, ref Text[] ___CrewStatusSlots_HPs, ref Text[] ___CrewStatusSlots_Names, ref Image[] ___CrewStatusSlots_BGs, ref Image[] ___CrewStatusSlots_Fills, ref Image[] ___CrewStatusSlots_TalkingImages, ref Image[] ___CrewStatusSlots_SlowFills)
		{
			if (!Global.ModEnabled || PLServer.Instance == null || PLNetworkManager.Instance == null || PLNetworkManager.Instance.LocalPlayer == null) return;
			if (PLServer.Instance.GameHasStarted && PLNetworkManager.Instance.LocalPlayer.GetHasStarted())
			{
				if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0) // If the skipwarp label should appear (Host nickname as skipwarp indicates we're in warp)
				{
					PLGlobal.SafeLabelSetText(__instance.SkipWarpLabel, ___cSkipWarpLabel.ToString(PLInput.Instance.GetPrimaryKeyStringForAction(PLInputBase.EInputActionName.skip_warp, true)));
					__instance.SkipWarpLabel.enabled = true;
				}
				else
				{
					__instance.SkipWarpLabel.enabled = false;
				}
				string a2 = "";
				if (PLCameraSystem.Instance != null)
				{
					a2 = PLCameraSystem.Instance.GetModeString();
				}
				if (UnityEngine.Random.Range(0, 20) == 0 || PLServer.Instance == null)
				{
					string text = "";
					if (__instance.ControlsText.enabled)
					{
						if (a2 == "LocalPawn")
						{
							if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp")
							{
								text = text + "<color=#AAAAAA><color=#ffff00>" + PLInput.Instance.GetPrimaryKeyStringForAction(PLInputBase.EInputActionName.skip_warp, true) + "</color> Skip Warp</color>\n";
							}
						}
						else
						{
							text = "";
						}
					}
					PLGlobal.SafeLabelSetText(__instance.ControlsText, text);
				}
			}
		}
	}
}
