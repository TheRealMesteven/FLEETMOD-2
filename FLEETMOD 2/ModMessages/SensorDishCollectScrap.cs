using PulsarModLoader;
using UnityEngine;

namespace FLEETMOD_2.ModMessages
{
	public class SensorDishCollectScrap : ModMessage
	{
		public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
		{
			if (!Global.ModEnabled || !PhotonNetwork.isMasterClient) return;
			if (int.TryParse(arguments[0].ToString(), out int ObjectID) && int.TryParse(arguments[1].ToString(), out int ShipID))
            {
				PLSpaceScrap plspaceScrap = PLSpecialEncounterNetObject.GetObjectAtID(ObjectID) as PLSpaceScrap;
				PLShipInfo plshipInfo = (PLShipInfo)PLEncounterManager.Instance.GetShipFromID(ShipID);
				PLSlot slot = plshipInfo.MyStats.GetSlot(ESlotType.E_COMP_CARGO);
				if (slot != null && (slot.Count < slot.MaxItems || plshipInfo.ShipTypeID == EShipType.E_ACADEMY))
                {
					plspaceScrap.Collected = true;
					PLServer.Instance.photonView.RPC("ScrapCollectedEffect", PhotonTargets.All, new object[]
					{
						plspaceScrap.transform.position
					});
					if (plshipInfo.ShipTypeID != EShipType.E_ACADEMY)
					{
						PLServer.Instance.photonView.RPC("ScrapLateCollected", PhotonTargets.All, new object[]
						{
							ObjectID
						});
						if (plspaceScrap.IsSpecificComponentScrap)
						{
							plshipInfo.MyStats.AddShipComponent(PLWare.CreateFromHash(1, plspaceScrap.SpecificComponent_CompHash) as PLShipComponent, -1, ESlotType.E_COMP_CARGO);
						}
						else
						{
							PLRand plrand = new PLRand(PLServer.Instance.GalaxySeed + PLServer.Instance.GetCurrentHubID() + ObjectID);
							int inLevel = 0;
							if (plspaceScrap.CanGiveComponent)
							{
								int num = plrand.Next(0, 200);
								if (PLEncounterManager.Instance.PlayerShip.ShipTypeID == EShipType.E_CARRIER)
								{
									num = plrand.Next(0, 75);
								}
								Mathf.RoundToInt(Mathf.Pow(plrand.Next(0f, 1f), 4f) * PLServer.Instance.ChaosLevel);
								PLShipComponent plshipComponent = null;
								if (num < 50 && plspaceScrap.SpecificComponent_CompHash != -1)
								{
									plshipComponent = PLShipComponent.CreateShipComponentFromHash(plspaceScrap.SpecificComponent_CompHash, null);
								}
								if (plshipComponent == null)
								{
									plshipComponent = new PLScrapCargo(inLevel);
								}
								plshipInfo.MyStats.AddShipComponent(plshipComponent, -1, ESlotType.E_COMP_CARGO);
							}
							else
							{
								plshipInfo.MyStats.AddShipComponent(new PLScrapCargo(inLevel), -1, ESlotType.E_COMP_CARGO);
							}
						}
					}
				}
			}
		}
	}
}
