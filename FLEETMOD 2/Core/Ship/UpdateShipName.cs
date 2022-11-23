using HarmonyLib;

namespace FLEETMOD_2.Core.Ship
{
    [HarmonyPatch(typeof(PLEditGameSettingsMenu), "UpdateShipName")]
    internal class UpdateShipName
    {
        public static bool Prefix(ref DBM_InputField ___ShipNameInput, ref string ___CachedShipName)
        {
            if (!Global.ModEnabled) return true;
            if (___ShipNameInput.Field != null && ___CachedShipName != ___ShipNameInput.Field.text)
            {
                PLEncounterManager.Instance.GetShipFromID(Global.GetPlayersShip(PLNetworkManager.Instance.LocalPlayerID)).photonView.RPC("Captain_NameShip", PhotonTargets.All, new object[]
                {
                        ___ShipNameInput.Field.text
                });
            }
            return false;
        }
    }
}
