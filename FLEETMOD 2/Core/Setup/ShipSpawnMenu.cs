using HarmonyLib;
using PulsarModLoader;
using PulsarModLoader.Patches;
using PulsarModLoader.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static PulsarModLoader.Patches.HarmonyHelpers;

namespace FLEETMOD_2.Core.Setup
{
    [HarmonyPatch(typeof(PLServer), "Update")]
    internal class ShipSpawnMenuPatch
    {
        // Detect the Keybind to create a ship and then generate menu
        public static void Postfix(PLServer __instance)
        {
            if (!Global.ModEnabled || __instance == null || !__instance.GameHasStarted || PLNetworkManager.Instance.LocalPlayer == null || !PLNetworkManager.Instance.LocalPlayer.GetHasStarted()
            || PLEncounterManager.Instance.PlayerShip == null || (PLNetworkManager.Instance.LocalPlayer.GetClassID() == 0 && !PhotonNetwork.isMasterClient) || PLNetworkManager.Instance.IsTyping || !Input.GetKeyDown(KeyCode.F6))
                return;
            PLMusic.PostEvent("play_sx_playermenu_click_major", PLServer.Instance.gameObject);
            PLNetworkManager.Instance.MainMenu.CloseActiveMenu();
            PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLCreateGameMenu(true));
            PLTabMenu.Instance.TabMenuActive = false;
        }
    }
    [HarmonyPatch(typeof(PLUICreateGameMenu), "ClickEngage")]
    internal class ShipSpawnMenu
    {
        // Overriding the ClickEngage button on the CreateGameMenu so that ingame it creates a ship using the information provided.
        protected static FieldInfo FieldInfo = AccessTools.Field(typeof(PLUICreateGameMenu), "CampaignMode");
        public static bool Prefix(PLUICreateGameMenu __instance, ref int ___CurrentSelectedShipIndex)
        {
            if (!Global.ModEnabled || PLServer.Instance == null || !PLServer.Instance.GameHasStarted) return true;
            PLMusic.PostEvent("play_sx_playermenu_click_major", PLGlobal.Instance.gameObject);

            // GetCurrentShipTypeID (Used by default code)
            PLNetworkManager.Instance.SelectedShipTypeID = ___CurrentSelectedShipIndex + 1;
            if (___CurrentSelectedShipIndex > 8)
                PLNetworkManager.Instance.SelectedShipTypeID = ___CurrentSelectedShipIndex;

            PLNetworkManager.Instance.MainMenu.CloseActiveMenu();

            // Class Change & Ship Spawning occurs in this ModMessage
            ModMessage.SendRPC(Mod.HarmonyIdent, "FLEETMOD_2.ModMessages.ServerCreateShip", PhotonTargets.MasterClient, new object[]
            {
                    PLNetworkManager.Instance.SelectedShipTypeID,
                    PLNetworkManager.Instance.LocalPlayer.GetPlayerID(),
                    __instance.ShipNameField.text
            });

            // Allow Admiral to spawn ships with Menu without becoming new Captain
            if (!PhotonNetwork.isMasterClient)
            {
                PLNetworkManager.Instance.MainMenu.AddActiveMenu(new PLErrorMessageMenu(string.Concat(new string[]
                {
                    "<color=#0066FF>You Are Now The Captain Of ",
                    __instance.ShipNameField.text,
                    "</color>\n\n<color=#c0c0c0>",
                    PLGlobal.Instance.ClassDesc[0],
                    "</color>"
                })));
            }
            return false;
        }
    }
    [HarmonyPatch(typeof(PLUIInsideWorldUI), "Update")]
    internal class UIInsideWorldUIPatch
    {
        // Update has an exception when called in-game due to missing a null check for PLAbyssShipInfo.Instance.Map
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> targetSequence = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldsfld, AccessTools.Field(typeof(PLAbyssShipInfo), "Instance")),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLAbyssShipInfo), "Map")),
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLAbyssMap), "ShowOnScreen")),
            };
            // Gets CodeInstruction to exit if when null == true
            int InstructionIndex = FindSequence(instructions, targetSequence, CheckMode.ALWAYS, true) - (targetSequence.Count() + 1);
            List<CodeInstruction> patchSequence = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(UIInsideWorldUIFix), "NullCheck", null, null)),
                instructions.ToList()[InstructionIndex]
            };
            return PatchBySequence(instructions, targetSequence, patchSequence, PatchMode.BEFORE, CheckMode.ALWAYS, true);
        }
    }
    internal class UIInsideWorldUIFix
    {
        public static bool NullCheck()
        {
            return PLAbyssShipInfo.Instance.Map == null;
        }
    }
}