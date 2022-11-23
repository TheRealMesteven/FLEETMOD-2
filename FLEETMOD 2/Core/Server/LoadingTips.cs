using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FLEETMOD_2.Core.Server
{
	[HarmonyPatch(typeof(PLNetworkManager), "Update")]
	internal class LoadingTips
	{
		public static bool Prefix(ref int ___TipNumber, ref List<string> ___TipStrings)
		{
			if (___TipStrings.Count > 20)
			{
				if (PLXMLOptionsIO.Instance.CurrentOptions.HasStringValue("TipNumber"))
				{
					___TipNumber = PLXMLOptionsIO.Instance.CurrentOptions.GetStringValueAsInt("TipNumber");
				}
				else
				{
					___TipNumber = 0;
					PLXMLOptionsIO.Instance.CurrentOptions.SetStringValue("TipNumber", ___TipNumber.ToString());
				}
				___TipStrings.Clear();
				___TipStrings.Add("<color=yellow>Fleet Mod - Sometimes I dream about Cheese</color>");
				___TipStrings.Add("<color=yellow>Fleet Mod - Hello There! General Kenobi.</color>");
			}
			return true;
		}
	}
}
