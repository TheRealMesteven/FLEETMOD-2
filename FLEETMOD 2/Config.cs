using PulsarModLoader.CustomGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace FLEETMOD_2
{
    internal class Config : ModSettingsMenu
    {
        public override string Name() => $"{Mod.myversion} Configuration";
        public override void Draw()
        {
            // Mod Enabled
            // < - - - - - - - - - - - - Gameplay - - - - - - - - - - - - >
            // Use Dialogue Spawning System       // Use Tab Menu (F1) Spawning System
            // Scale Hostile Count to Fleet
            // < - - - - - - - - - - - - Races    - - - - - - - - - - - - >
            // Relay Race?     // Scale To Fleet? // Any Fleet Winner?
            // < - - - - - - - - - - - - Admiral  - - - - - - - - - - - - >
            // Use Admiral Captain Commands
            // Use Admiral Ship Targetting
            // Allow targetting teleportation
            // Allow Fleet teleportation through shields
            // in-Crew PVP     // in-Fleet PVP    // Ship PVP
            // Faction Only Ship Spawns           // Need to convert to Faction
            // (Slider) Max Ship Count

            Global.ModEnabled = GUILayout.Toggle(Global.ModEnabled, $"Enable {Mod.myversion}");
            GUILayout.Label("-- Ship Spawning --");
            GUILayout.BeginHorizontal();
            //CampaignShipsOnly = GUILayout.Toggle(CampaignShipsOnly, "Campaign Ships Only");
            FactionShipsOnly = GUILayout.Toggle(FactionShipsOnly, "Faction Ships Only");
            GUILayout.EndHorizontal();
        }
        public static bool CampaignShipsOnly = false;
        public static bool FactionShipsOnly = false;
    }
}
