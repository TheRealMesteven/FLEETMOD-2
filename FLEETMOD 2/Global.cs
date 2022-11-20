using System;
using System.Collections.Generic;
using System.Linq;
namespace FLEETMOD_2
{
    internal class Global
    {
        public static bool ModEnabled = true;
        public static List<ShipInfo> FleetShips = new List<ShipInfo>();
        public static List<int> FleetModClients = new List<int>();

        public static List<int> GetFleetShips()
        {
            List<int> ShipIDs = new List<int>();
            foreach (ShipInfo sh in FleetShips)
            {
                ShipIDs.Add(sh.ShipID);
            }
            return ShipIDs;
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
