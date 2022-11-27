using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FLEETMOD_2.Core.Ship
{
    [HarmonyPatch(typeof(PLShipInfo), "CheckForIntruders")]
    internal class IntruderAlarm
    {
		protected static FieldInfo FieldInfo = AccessTools.Field(typeof(PLShipInfo), "LastIntruderAlarmStartedTime");
        public static bool Prefix(PLShipInfo __instance)
        {
            if (!Global.ModEnabled || !Global.GetFleetShips().Contains(__instance.ShipID)) return true;
            if (__instance.IsAuxSystemActive(6))
            {
				bool flag = false;
				foreach (PLPlayer plplayer in PLServer.Instance.AllPlayers)
				{
					if (plplayer != null && plplayer.StartingShip != null && plplayer.StartingShip != __instance && !Global.GetFleetShips().Contains(plplayer.StartingShip.ShipID) && plplayer.PlayerLifeTime > 5f && plplayer.GetPawn() != null && plplayer.GetPawn().CurrentShip == __instance)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					foreach (PLCombatTarget plcombatTarget in PLGameStatic.Instance.AllCombatTargets)
					{
						if (plcombatTarget != null && plcombatTarget.GetPlayer() == null && plcombatTarget.Lifetime > 4f && plcombatTarget.ShouldShowInHUD() && !plcombatTarget.GetIsFriendly() && plcombatTarget.CurrentShip == __instance)
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					__instance.AlertLevel = 2;
					FieldInfo.SetValue(__instance, Time.time);
					PLServer.Instance.photonView.RPC("AddCrewWarning", PhotonTargets.All, new object[]
					{
						"INTRUDER ALARM!",
						Color.red,
						0,
						"SHIP"
					});
					PLServer.Instance.photonView.RPC("ConsoleMessage", PhotonTargets.All, new object[]
					{
						"INTRUDER ALARM!"
					});
				}
			}
			return false;
        }
    }
}
