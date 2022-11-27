﻿using PulsarModLoader.CustomGUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
    }
}
