using PulsarModLoader;

namespace FLEETMOD_2
{
    public class Mod : PulsarMod
    {
        public static string myversion = "FLEETMOD V2.0.0";
        public override string Version => Mod.myversion;
        public override string Author => "Mest";
        public override string Name => "FleetMod V2";
        public override string HarmonyIdentifier() => "Mest.Fleetmod";
    }
}
