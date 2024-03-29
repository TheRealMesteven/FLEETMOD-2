﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PulsarModLoader.Utilities;
namespace FLEETMOD_2
{
    internal class Global
    {
        public static bool ModEnabled = true;
        public static List<ShipInfo> FleetShips = new List<ShipInfo>();
        public static List<int> FleetModClients = new List<int>();
        public static Dictionary<int, int> FleetModSpawningPlayers = new Dictionary<int, int>();

        /// <summary>
        /// Returns a list of Fleet ShipIDs
        /// </summary>
        public static List<int> GetFleetShips()
        {
            List<int> ShipIDs = new List<int>();
            foreach (ShipInfo sh in FleetShips)
            {
                //Logger.Info($"[GFS] {sh != null} | {PLEncounterManager.Instance != null} | {sh.ShipID} | {PLEncounterManager.Instance.GetShipFromID(sh.ShipID) != null}");
                if (sh != null && PLEncounterManager.Instance != null && PLEncounterManager.Instance.GetShipFromID(sh.ShipID) != null)
                {
                    ShipIDs.Add(sh.ShipID);
                }
            }
            return ShipIDs;
        }

        /// <summary>
        /// Gets the Index from FleetShips using ShipID
        /// </summary>
        public static int GetFleetShipIndex(int ShipID)
        {
            foreach (ShipInfo sh in FleetShips)
            {
                if (sh.ShipID == ShipID)
                {
                    return FleetShips.IndexOf(sh);
                }
            }
            return -1;
        }

        /// <summary>
        /// Gets the PlayerID's Ship
        /// </summary>
        public static int GetPlayersShip(int PlayerID)
        {
            foreach (ShipInfo sh in FleetShips)
            {
                if (sh.Crew.Contains(PlayerID))
                {
                    return sh.ShipID;
                }
            }
            return -1;
        }
        
        /// <summary>
        /// Adds to FleetShips / Updates FleetShip
        /// </summary>
        public static void UpdateFleetShips(int ShipID, ShipInfo shipInfo)
        {
            if (GetFleetShips().Contains(ShipID))
            {
                FleetShips.Remove(FleetShips[GetFleetShipIndex(ShipID)]);
                FleetShips.Add(shipInfo);
            }
            else
            {
                FleetShips.Add(shipInfo);
            }
        }

        /// <summary>
        /// Datastream storage structure of ShipInfos
        /// </summary>
        public static byte[] SerializeFleetShips(List<ShipInfo> shipInfos)
        {
            MemoryStream dataStream = new MemoryStream();
            dataStream.Position = 0;
            using (BinaryWriter writer = new BinaryWriter(dataStream))
            {
                writer.Write(shipInfos.Count());
                foreach (ShipInfo shipInfo in shipInfos)
                {
                    writer.Write(shipInfo.ShipID);
                    writer.Write(shipInfo.Crew.Count());
                    foreach (int i in shipInfo.Crew)
                    {
                        writer.Write(i);
                    }
                    foreach (int i in shipInfo.RoleLimits)
                    {
                        writer.Write(i);
                    }
                }
            }
            return dataStream.ToArray();
        }

        /// <summary>
        /// ShipInfos of Datastream storage structure
        /// </summary>
        public static List<ShipInfo> DeSerializeFleetShips(byte[] byteData)
        {
            MemoryStream memoryStream = new MemoryStream(byteData);
            memoryStream.Position = 0;
            try
            {
                using (BinaryReader reader = new BinaryReader(memoryStream))
                {
                    int shipInfosCount = reader.ReadInt32();
                    List<ShipInfo> shipInfos = new List<ShipInfo>();
                    for (int i = 0; i < shipInfosCount; i++)
                    {
                        int ShipID = reader.ReadInt32();
                        int CrewCount = reader.ReadInt32();
                        int[] Crew = new int[CrewCount];
                        for (int j = 0; j < CrewCount; j++)
                        {
                            Crew.Append(reader.ReadInt32());
                        }
                        int[] RoleLimits = new int[5];
                        for (int k = 0; k < 5; k++)
                        {
                            RoleLimits.Append(reader.ReadInt32());
                        }
                        shipInfos.Add(new ShipInfo(ShipID, Crew.ToList(), RoleLimits));
                    }
                    return shipInfos;
                }
            }
            catch (Exception ex)
            {
                Logger.Info($"Failed to read FleetShip List, returning null.\n{ex.Message}");
                return null;
            }
        }

        public static int ShipTypeToFaction(int ShipType)
        {
            switch (ShipType)
            {
                case (int)EShipType.E_INTREPID:
                case (int)EShipType.E_ROLAND:
                case (int)EShipType.E_OUTRIDER:
                case (int)EShipType.E_ABYSS_PLAYERSHIP:
                    return 0;
                case (int)EShipType.E_STARGAZER:
                case (int)EShipType.E_CARRIER:
                    return 1;
                case (int)EShipType.E_WDCRUISER:
                case (int)EShipType.E_DESTROYER:
                case (int)EShipType.E_ANNIHILATOR:
                    return 2;
                case (int)EShipType.E_FLUFFY_DELIVERY:
                case (int)EShipType.E_FLUFFY_TWO:
                    return 3;
                case (int)EShipType.E_POLYTECH_SHIP:
                    return 5;
                default:
                    return -1;
            }
        }
    }

    /// <summary>
    /// Fleetmod Critical Information about FleetShips
    /// </summary>
    internal class ShipInfo
    {
        public int ShipID;
        public List<int> Crew;
        public int[] RoleLimits = new int[5];
        public ShipInfo(int _ShipID, List<int> _Crew, int[] _RoleLimits = null)
        {
            ShipID = _ShipID;
            Crew = _Crew;
            if (_RoleLimits == null || _RoleLimits.Count() != 5)
            {
                RoleLimits = new int[5] { 1,1,1,1,1 };
            }
            else
            {
                RoleLimits = _RoleLimits;
            }
        }
        
        // Checks if Ship has slot available as class
        public bool CanJoinClass(int classID)
        {
            return GetPlayerOfClass(classID).Count < RoleLimits[classID];
        }

        // Gets list of Players of class
        public List<int> GetPlayerOfClass(int classID)
        {
            List<int> list = new List<int>();
            foreach (int i in Crew)
            {
                if (PLServer.Instance.GetPlayerFromPlayerID(i) != null && PLServer.Instance.GetPlayerFromPlayerID(i).GetClassID() == classID)
                {
                    list.Add(i);
                }
            }
            return list;
        }
    }
}
