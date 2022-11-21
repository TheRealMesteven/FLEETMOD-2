using System;
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

        /// <summary>
        /// Returns a list of Fleet ShipIDs
        /// </summary>
        public static List<int> GetFleetShips()
        {
            List<int> ShipIDs = new List<int>();
            foreach (ShipInfo sh in FleetShips)
            {
                //Logger.Info($"[GFS] {sh != null} | {PLEncounterManager.Instance != null} | {sh.ShipID} | {PLEncounterManager.Instance.GetShipFromID(sh.ShipID) != null}");
                if (sh != null && PLEncounterManager.Instance.GetShipFromID(sh.ShipID) != null)
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
                    ShipInfo[] shipInfos = new ShipInfo[shipInfosCount];
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
                        shipInfos.Append(new ShipInfo(ShipID, Crew.ToList(), RoleLimits));
                    }
                    return shipInfos.ToList();
                }
            }
            catch (Exception ex)
            {
                Logger.Info($"Failed to read FleetShip List, returning null.\n{ex.Message}");
                return null;
            }
        }
    }
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
        public bool CanJoinClass(int classID)
        {
            return GetPlayerOfClass(classID).Count < RoleLimits[classID];
        }
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
