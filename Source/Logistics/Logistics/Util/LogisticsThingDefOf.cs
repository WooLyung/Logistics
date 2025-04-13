using Verse;

namespace Logistics
{
    public static class LogisticsThingDefOf
    {
        private static ThingDef logisticsSystemController = null;
        private static ThingDef conveyorInterface = null;
        private static ThingDef logisticsInputInterface = null;
        private static ThingDef logisticsOutputInterface = null;
        private static ThingDef logisticsIOInterface = null;
        private static ThingDef logisticsInputWallInterface = null;
        private static ThingDef logisticsOutputWallInterface = null;
        private static ThingDef logisticsIOWallInterface = null;
        private static ThingDef remoteInputInterface = null;
        private static ThingDef remoteOutputInterface = null;
        private static ThingDef remoteIOInterface = null;
        private static ThingDef logisticsNetworkLinker = null;

        public static ThingDef LogisticsSystemController => logisticsSystemController = logisticsSystemController ?? ThingDef.Named("LogisticsSystemController");
        public static ThingDef LogisticsInputInterface => logisticsInputInterface = logisticsInputInterface ?? ThingDef.Named("LogisticsInputInterface");
        public static ThingDef LogisticsOutputInterface => logisticsOutputInterface = logisticsOutputInterface ?? ThingDef.Named("LogisticsOutputInterface");
        public static ThingDef LogisticsIOInterface => logisticsIOInterface = logisticsIOInterface ?? ThingDef.Named("LogisticsIOInterface");
        public static ThingDef LogisticsInputWallInterface => logisticsInputWallInterface = logisticsInputWallInterface ?? ThingDef.Named("LogisticsInputWallInterface");
        public static ThingDef LogisticsOutputWallInterface => logisticsOutputWallInterface = logisticsOutputWallInterface ?? ThingDef.Named("LogisticsOutputWallInterface");
        public static ThingDef LogisticsIOWallInterface => logisticsIOWallInterface = logisticsIOWallInterface ?? ThingDef.Named("LogisticsIOWallInterface");
        public static ThingDef RemoteInputInterface => remoteInputInterface = remoteInputInterface ?? ThingDef.Named("RemoteInputInterface");
        public static ThingDef RemoteOutputInterface => remoteOutputInterface = remoteOutputInterface ?? ThingDef.Named("RemoteOutputInterface");
        public static ThingDef RemoteIOInterface => remoteIOInterface = remoteIOInterface ?? ThingDef.Named("RemoteIOInterface");
        public static ThingDef LogisticsNetworkLinker => logisticsNetworkLinker = logisticsNetworkLinker ?? ThingDef.Named("LogisticsNetworkLinker");
        public static ThingDef ConveyorInterface => conveyorInterface = conveyorInterface ?? ThingDef.Named("ConveyorInterface");
    }
}
