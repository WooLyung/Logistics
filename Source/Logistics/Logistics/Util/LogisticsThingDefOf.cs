using Verse;

namespace Logistics
{
    public static class LogisticsThingDefOf
    {
        private static ThingDef logisticsSystemController = null;
        private static ThingDef conveyorPort = null;

        private static ThingDef logisticsInputTerminal = null;
        private static ThingDef logisticsOutputTerminal = null;
        private static ThingDef logisticsIOTerminal = null;

        private static ThingDef logisticsInputWallTerminal = null;
        private static ThingDef logisticsOutputWallTerminal = null;
        private static ThingDef logisticsIOWallTerminal = null;

        private static ThingDef remoteInputTerminal = null;
        private static ThingDef remoteOutputTerminal = null;
        private static ThingDef remoteIOTerminal = null;

        private static ThingDef logisticsNetworkLinker = null;

        public static ThingDef LogisticsSystemController => logisticsSystemController = logisticsSystemController ?? ThingDef.Named("LogisticsSystemController");
        public static ThingDef LogisticsInputTerminal => logisticsInputTerminal = logisticsInputTerminal ?? ThingDef.Named("LogisticsInputTerminal");
        public static ThingDef LogisticsOutputTerminal => logisticsOutputTerminal = logisticsOutputTerminal ?? ThingDef.Named("LogisticsOutputTerminal");
        public static ThingDef LogisticsIOTerminal => logisticsIOTerminal = logisticsIOTerminal ?? ThingDef.Named("LogisticsIOTerminal");
        public static ThingDef LogisticsInputWallTerminal => logisticsInputWallTerminal = logisticsInputWallTerminal ?? ThingDef.Named("LogisticsInputWallTerminal");
        public static ThingDef LogisticsOutputWallTerminal => logisticsOutputWallTerminal = logisticsOutputWallTerminal ?? ThingDef.Named("LogisticsOutputWallTerminal");
        public static ThingDef LogisticsIOWallTerminal => logisticsIOWallTerminal = logisticsIOWallTerminal ?? ThingDef.Named("LogisticsIOWallTerminal");
        public static ThingDef RemoteInputTerminal => remoteInputTerminal = remoteInputTerminal ?? ThingDef.Named("RemoteInputTerminal");
        public static ThingDef RemoteOutputTerminal => remoteOutputTerminal = remoteOutputTerminal ?? ThingDef.Named("RemoteOutputTerminal");
        public static ThingDef RemoteIOTerminal => remoteIOTerminal = remoteIOTerminal ?? ThingDef.Named("RemoteIOTerminal");
        public static ThingDef LogisticsNetworkLinker => logisticsNetworkLinker = logisticsNetworkLinker ?? ThingDef.Named("LogisticsNetworkLinker");
        public static ThingDef ConveyorPort => conveyorPort = conveyorPort ?? ThingDef.Named("ConveyorPort");
    }
}
