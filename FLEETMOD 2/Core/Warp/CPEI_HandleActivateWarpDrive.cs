using HarmonyLib;
using PulsarModLoader.Utilities;

namespace FLEETMOD_2.Core.Warp
{
    [HarmonyPatch(typeof(PLServer), "CPEI_HandleActivateWarpDrive")]
    internal class CPEI_HandleActivateWarpDrive
    {
        public static bool Prefix(int shipID, int playerID)
        {
            if (!Global.ModEnabled)
            {
                return true;
            }

            //Stop if warping client doesn't have FM. 
            if (!Global.FleetModClients.Contains(playerID))
            {
                return false;
            }

            //Warp notification
            PLServer.Instance.photonView.RPC("AddNotification", PhotonTargets.All, new object[]
            {
                PLServer.Instance.GetPlayerFromPlayerID(playerID).GetPlayerName(false) + " has engaged the warp! Heading to: " + PLEncounterManager.Instance.GetShipFromID(shipID).WarpTargetID.ToString(),
                playerID,
                PLServer.Instance.GetEstimatedServerMs() + 3000,
                true
            });


            //Warp the ship.  -   Don't know why this localPlayer null check is here, might be able to remove it.
            if (PLNetworkManager.Instance.LocalPlayer != null)
            {
                PLServer.Instance.photonView.RPC("NetworkBeginWarp", PhotonTargets.All, new object[]
                {
                    PLEncounterManager.Instance.PlayerShip.ShipID,
                    PLEncounterManager.Instance.GetShipFromID(shipID).WarpTargetID,
                    PLServer.Instance.GetEstimatedServerMs(),
                    -1
                });
            }
            return false;
        }
    }
}
