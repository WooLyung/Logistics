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

        public static IEnumerable<Building_ConveyorPort> GetAllConveyorPorts(this Room room)
        {
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.ConveyorPort))
                if (thing is Building_ConveyorPort port)
                    yield return port;
        }

        public static IEnumerable<Building_Terminal> GetAllInputTerminals(this Room room)
        {
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsInputTerminal))
                if (thing is Building_Terminal terminal)
                    yield return terminal;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsInputWallTerminal))
                if (thing is Building_Terminal terminal)
                    yield return terminal;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.RemoteInputTerminal))
                if (thing is Building_Terminal terminal)
                    yield return terminal;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsIOTerminal))
                if (thing is Building_Terminal terminal)
                    yield return terminal;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsIOWallTerminal))
                if (thing is Building_Terminal terminal)
                    yield return terminal;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.RemoteIOTerminal))
                if (thing is Building_Terminal terminal)
                    yield return terminal;
        }

        public static IEnumerable<Building_Terminal> GetAllOutputTerminals(this Room room)
        {
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsOutputTerminal))
                if (thing is Building_Terminal terminal)
                    yield return terminal;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsOutputWallTerminal))
                if (thing is Building_Terminal terminal)
                    yield return terminal;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.RemoteOutputTerminal))
                if (thing is Building_Terminal terminal)
                    yield return terminal;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsIOTerminal))
                if (thing is Building_Terminal terminal)
                    yield return terminal;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.LogisticsIOWallTerminal))
                if (thing is Building_Terminal terminal)
                    yield return terminal;
            foreach (var thing in room.ThingsOfDef(LogisticsThingDefOf.RemoteIOTerminal))
                if (thing is Building_Terminal terminal)
                    yield return terminal;
        }

        public static IEnumerable<Building_RemoteTerminal> GetAllRemoteInputTerminals(this Map map)
        {
            foreach (var thing in map.listerThings.ThingsOfDef(LogisticsThingDefOf.RemoteInputTerminal))
                if (thing is Building_RemoteTerminal terminal)
                    yield return terminal;
            foreach (var thing in map.listerThings.ThingsOfDef(LogisticsThingDefOf.RemoteIOTerminal))
                if (thing is Building_RemoteTerminal terminal)
                    yield return terminal;
        }

        public static IEnumerable<Building_RemoteTerminal> GetAllRemoteOutputTerminals(this Map map)
        {
            foreach (var thing in map.listerThings.ThingsOfDef(LogisticsThingDefOf.RemoteOutputTerminal))
                if (thing is Building_RemoteTerminal terminal)
                    yield return terminal;
            foreach (var thing in map.listerThings.ThingsOfDef(LogisticsThingDefOf.RemoteIOTerminal))
                if (thing is Building_RemoteTerminal terminal)
                    yield return terminal;
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
