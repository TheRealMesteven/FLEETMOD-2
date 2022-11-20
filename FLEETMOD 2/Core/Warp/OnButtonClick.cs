using HarmonyLib;

namespace FLEETMOD_2.Core.Warp
{
	[HarmonyPatch(typeof(PLWarpDriveScreen), "OnButtonClick")]
	internal class OnButtonClick
	{
		public static bool Prefix(UIWidget inButton)
		{
			if (!Global.ModEnabled) return true;
			return !(!PhotonNetwork.isMasterClient && !(inButton.name == "Jump"));
		}
	}
}
