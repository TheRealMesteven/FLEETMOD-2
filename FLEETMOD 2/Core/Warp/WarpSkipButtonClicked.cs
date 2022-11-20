using HarmonyLib;
using System;

namespace FLEETMOD_2.Core.Warp
{
    [HarmonyPatch(typeof(PLInGameUI), "WarpSkipButtonClicked")]
    internal class WarpSkipButtonClicked
    {
        public static bool Prefix()
        {
            if (!Global.ModEnabled) return true;
            if (PLServer.Instance.GetPlayerFromPlayerID(0).GetPhotonPlayer().NickName == "skipwarp" && PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0)
            {
                PLServer.Instance.GetPlayerFromPlayerID(0).StartingShip.photonView.RPC("SkipWarp", PhotonTargets.All, Array.Empty<object>());
            }
            return false;
        }
    }
}
