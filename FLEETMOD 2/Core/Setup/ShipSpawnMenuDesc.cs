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
    [HarmonyPatch(typeof(PLShipInfo), "GetLockedReasonString")]
    internal class AvailableToCreateGameDesc_ShipInfo
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return AvailableToCreateGameDesc_Patch.Patch(instructions);
        }
    }
    [HarmonyPatch(typeof(PLCivilianStartingShipInfo), "GetLockedReasonString")]
    internal class AvailableToCreateGameDesc_Civilian
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return AvailableToCreateGameDesc_Patch.Patch(instructions);
        }
    }
    [HarmonyPatch(typeof(PLOldWarsShip_Human), "GetLockedReasonString")]
    internal class AvailableToCreateGameDesc_Human
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return AvailableToCreateGameDesc_Patch.Patch(instructions);
        }
    }
    [HarmonyPatch(typeof(PLOldWarsShip_Sylvassi), "GetLockedReasonString")]
    internal class AvailableToCreateGameDesc_Sylvassi
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return AvailableToCreateGameDesc_Patch.Patch(instructions);
        }
    }
    [HarmonyPatch(typeof(PLOutriderInfo), "GetLockedReasonString")]
    internal class AvailableToCreateGameDesc_Outrider
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return AvailableToCreateGameDesc_Patch.Patch(instructions);
        }
    }
    [HarmonyPatch(typeof(PLWDAnnihilatorInfo), "GetLockedReasonString")]
    internal class AvailableToCreateGameDesc_Annihilator
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return AvailableToCreateGameDesc_Patch.Patch(instructions);
        }
    }


    internal class AvailableToCreateGameDesc_Patch
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
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(AvailableToCreateGameDesc_Patch), "StringOverridePatch", null, null)),
            };
            return PatchBySequence(instructions, targetSequence, patchSequence, PatchMode.BEFORE, CheckMode.NEVER, true);
        }

        // Adds additional checks for 'CampaignShipsOnly' Config and 'FactionShipsOnly' Config
        public static string StringOverridePatch(string OriginalOutcome, PLShipInfo pLShipInfo)
        {
            if (!Global.ModEnabled || PLServer.Instance == null || pLShipInfo == null || !PLServer.Instance.GameHasStarted)
            {
                Logger.Info($"[DESC] NULL");
                return OriginalOutcome;
            }
            Logger.Info($"[DESC] {pLShipInfo.GetShipTypeName()} VALID");
            if (Config.CampaignShipsOnly &&
                        ((CampaignMode == 1 && pLShipInfo.ShipTypeID != EShipType.E_POLYTECH_SHIP)
                        || (CampaignMode == 2 && pLShipInfo.ShipTypeID != EShipType.E_ABYSS_PLAYERSHIP)
                        || (CampaignMode == 0 && (pLShipInfo.ShipTypeID == EShipType.E_POLYTECH_SHIP || pLShipInfo.ShipTypeID == EShipType.E_ABYSS_PLAYERSHIP))))
            {
                return "Admiral has locked the Ships to Campaign Ships Only";
            }
            else if (Config.FactionShipsOnly && (PLServer.Instance.CrewFactionID != pLShipInfo.FactionID))
            {
                return "Admiral has locked the Ships to Faction Ships Only";
            }
            return OriginalOutcome;
        }
    }
}
