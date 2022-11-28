using HarmonyLib;
using PulsarModLoader.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static PulsarModLoader.Patches.HarmonyHelpers;

namespace FLEETMOD_2.Core.Setup
{
    // Overrides which Ships are Locked based on Config
    [HarmonyPatch(typeof(PLShipInfo), "IsCurrentlyAvailableToCreateGameWith")]
    internal class AvailableToCreateGame_ShipInfo
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return AvailableToCreateGame_Patch.Patch(instructions);
        }
    }
    [HarmonyPatch(typeof(PLCivilianStartingShipInfo), "IsCurrentlyAvailableToCreateGameWith")]
    internal class AvailableToCreateGame_Civilian
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return AvailableToCreateGame_Patch.Patch(instructions);
        }
    }
    [HarmonyPatch(typeof(PLOldWarsShip_Human), "IsCurrentlyAvailableToCreateGameWith")]
    internal class AvailableToCreateGame_Human
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return AvailableToCreateGame_Patch.Patch(instructions);
        }
    }
    [HarmonyPatch(typeof(PLOldWarsShip_Sylvassi), "IsCurrentlyAvailableToCreateGameWith")]
    internal class AvailableToCreateGame_Sylvassi
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return AvailableToCreateGame_Patch.Patch(instructions);
        }
    }
    [HarmonyPatch(typeof(PLOutriderInfo), "IsCurrentlyAvailableToCreateGameWith")]
    internal class AvailableToCreateGame_Outrider
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return AvailableToCreateGame_Patch.Patch(instructions);
        }
    }
    [HarmonyPatch(typeof(PLWDAnnihilatorInfo), "IsCurrentlyAvailableToCreateGameWith")]
    internal class AvailableToCreateGame_Annihilator
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return AvailableToCreateGame_Patch.Patch(instructions);
        }
    }


    internal class AvailableToCreateGame_Patch
    {
        public static int CampaignMode = -1;

        // Due to multiple 'IsCurrentlyAvailableToCreateGameWith' needing to be patched, easier to create a callable method.
        public static IEnumerable<CodeInstruction> Patch(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> targetSequence = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ret)
            };
            List<CodeInstruction> patchSequence = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AvailableToCreateGame_Patch), "BooleanOverridePatch", null, null)),
            };
            return PatchBySequence(instructions, targetSequence, patchSequence, PatchMode.BEFORE, CheckMode.NEVER, false);
        }

        // Adds additional checks for 'CampaignShipsOnly' Config and 'FactionShipsOnly' Config
        public static bool BooleanOverridePatch(bool OriginalOutcome, PLShipInfo pLShipInfo)
        {
            if (!Global.ModEnabled || PLServer.Instance == null || pLShipInfo == null || !PLServer.Instance.GameHasStarted)
            {
                Logger.Info($"[LOCK] NULL");
                return OriginalOutcome;
            }
            Logger.Info($"[LOCK] {pLShipInfo.GetShipTypeName()} VALID");
            if (Config.CampaignShipsOnly &&
                        ((CampaignMode == 1 && pLShipInfo.ShipTypeID != EShipType.E_POLYTECH_SHIP)
                        || (CampaignMode == 2 && pLShipInfo.ShipTypeID != EShipType.E_ABYSS_PLAYERSHIP)
                        || (CampaignMode == 0 && (pLShipInfo.ShipTypeID == EShipType.E_POLYTECH_SHIP || pLShipInfo.ShipTypeID == EShipType.E_ABYSS_PLAYERSHIP))))
            {
                return false;
            }
            else if (Config.FactionShipsOnly && (PLServer.Instance.CrewFactionID != pLShipInfo.FactionID))
            {
                return false;
            }
            return OriginalOutcome;
        }
    }

    [HarmonyPatch(typeof(PLUICreateGameMenu), "Enter")]
    internal class SpawnSameCampaign
    {
        // Config - Allow only ships of the same Campaign (Gets Campaign)
        protected static FieldInfo CampaignModeField = AccessTools.Field(typeof(PLUICreateGameMenu), "CampaignMode");
        public static void Postfix(PLUICreateGameMenu __instance)
        {
            if (!Global.ModEnabled || PLServer.Instance == null || !PLServer.Instance.GameHasStarted) return;
            ECampaignMode eCampaignMode = (ECampaignMode)CampaignModeField.GetValue(__instance);
            switch (eCampaignMode)
            {
                case ECampaignMode.LOST_COLONY:
                    AvailableToCreateGame_Patch.CampaignMode = 0;
                    return;
                case ECampaignMode.PT:
                    AvailableToCreateGame_Patch.CampaignMode = 1;
                    return;
                case ECampaignMode.ABYSS:
                    AvailableToCreateGame_Patch.CampaignMode = 2;
                    return;
                default:
                    break;
            }
            AvailableToCreateGame_Patch.CampaignMode = -1;
        }
    }

    // Due to Locked Ships needing to be clicked to unlock, we need to break that.
    [HarmonyPatch(typeof(PLUICreateGameMenu), "Update")]
    internal class ShipUnlockPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {

            List<CodeInstruction> targetSequence = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(PLGameProgressManager), "IsShipUnlockOpened")),
            };
            List<CodeInstruction> patchSequence = new List<CodeInstruction>
            {
                new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(typeof(ShipUnlockPatch), "BooleanOverridePatch")),
            };
            return PatchBySequence(instructions, targetSequence, patchSequence, PatchMode.REPLACE, CheckMode.ALWAYS, true);
        }
        public bool BooleanOverridePatch(int shipType, int variant = 0)
        {
            /// In future, maybe look into readding this as it may allow players to use ships that havent been unlocked yet.
            if (!Global.ModEnabled || PLServer.Instance == null || !PLServer.Instance.GameHasStarted) return true;
            if (Config.CampaignShipsOnly &&
                        ((AvailableToCreateGame_Patch.CampaignMode == 1 && shipType != (int)EShipType.E_POLYTECH_SHIP)
                        || (AvailableToCreateGame_Patch.CampaignMode == 2 && shipType != (int)EShipType.E_ABYSS_PLAYERSHIP)
                        || (AvailableToCreateGame_Patch.CampaignMode == 0 && (shipType == (int)EShipType.E_POLYTECH_SHIP || shipType == (int)EShipType.E_ABYSS_PLAYERSHIP))))
            {
                return false;
            }
            else if (Config.FactionShipsOnly && (PLServer.Instance.CrewFactionID != Global.ShipTypeToFaction(shipType)))
            {
                return false;
            }
            return true;
        }
    }
}
