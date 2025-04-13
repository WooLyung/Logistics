using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public static class ThingFinder
    {
        public static bool IsInRoom(this Thing thing, Room room)
        {
            if (thing.GetRoom() == room
                || (thing.Position + IntVec3.North).GetRoom(thing.Map) == room
                || (thing.Position + IntVec3.South).GetRoom(thing.Map) == room
                || (thing.Position + IntVec3.East).GetRoom(thing.Map) == room
                || (thing.Position + IntVec3.West).GetRoom(thing.Map) == room)
                return true;
            return false;
        }

        public static IEnumerable<Thing> ThingsOfDef(this Room room, ThingDef def)
        {
            foreach (var thing in room.Map.listerThings.ThingsOfDef(def))
                if (thing.IsInRoom(room))
                    yield return thing;
        }

        public static IEnumerable<Building_ConveyorInterface> GetAllConveyorInterfaces(this Room room)
        {
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.ConveyorInterface))
                if (thing is Building_ConveyorInterface itf)
                    yield return itf;
        }

        public static IEnumerable<Building_Interface> GetAllInputInterfaces(this Room room)
        {
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsInputInterface))
                if (thing is Building_Interface itf)
                    yield return itf;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsInputWallInterface))
                if (thing is Building_Interface itf)
                    yield return itf;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.RemoteInputInterface))
                if (thing is Building_Interface itf)
                    yield return itf;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsIOInterface))
                if (thing is Building_Interface itf)
                    yield return itf;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsIOWallInterface))
                if (thing is Building_Interface itf)
                    yield return itf;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.RemoteIOInterface))
                if (thing is Building_Interface itf)
                    yield return itf;
        }

        public static IEnumerable<Building_Interface> GetAllOutputInterfaces(this Room room)
        {
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsOutputInterface))
                if (thing is Building_Interface itf)
                    yield return itf;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsOutputWallInterface))
                if (thing is Building_Interface itf)
                    yield return itf;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.RemoteOutputInterface))
                if (thing is Building_Interface itf)
                    yield return itf;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsIOInterface))
                if (thing is Building_Interface itf)
                    yield return itf;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsIOWallInterface))
                if (thing is Building_Interface itf)
                    yield return itf;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.RemoteIOInterface))
                if (thing is Building_Interface itf)
                    yield return itf;
        }

        public static IEnumerable<Building_RemoteInterface> GetAllRemoteInputInterface(this Map map)
        {
            foreach (var thing in map.listerThings.ThingsOfDef(LogisticsThingDefOf.RemoteInputInterface))
                if (thing is Building_RemoteInterface itf)
                    yield return itf;
            foreach (var thing in map.listerThings.ThingsOfDef(LogisticsThingDefOf.RemoteIOInterface))
                if (thing is Building_RemoteInterface itf)
                    yield return itf;
        }

        public static IEnumerable<Building_RemoteInterface> GetAllRemoteOutputInterface(this Map map)
        {
            foreach (var thing in map.listerThings.ThingsOfDef(LogisticsThingDefOf.RemoteOutputInterface))
                if (thing is Building_RemoteInterface itf)
                    yield return itf;
            foreach (var thing in map.listerThings.ThingsOfDef(LogisticsThingDefOf.RemoteIOInterface))
                if (thing is Building_RemoteInterface itf)
                    yield return itf;
        }

        public static IEnumerable<Building_LogisticsNetworkLinker> GetAllOperationalLinkers(this Room room)
        {
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsNetworkLinker))
                if (thing is Building_LogisticsNetworkLinker linker && linker.IsOperational())
                    yield return linker;
        }

        public static Building_LogisticsSystemController GetControllerWithID(this Map map, string ID)
        {
            foreach (var thing in map.listerThings.ThingsOfDef(LogisticsThingDefOf.LogisticsSystemController))
                if (thing is Building_LogisticsSystemController controller && controller.NetworkID == ID)
                    return controller;
            return null;
        }
    }
}
