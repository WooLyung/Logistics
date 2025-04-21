using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Logistics
{
    public static class ThingFinder
    {
        public static bool IsInRoom(this Thing thing, Room room)
        {
            if (thing.Map == null || room.Map == null)
                return false;

            if (thing.GetRoom() == room
                || (thing.Position + IntVec3.North).GetRoom(thing.Map) == room
                || (thing.Position + IntVec3.South).GetRoom(thing.Map) == room
                || (thing.Position + IntVec3.East).GetRoom(thing.Map) == room
                || (thing.Position + IntVec3.West).GetRoom(thing.Map) == room)
                return true;
            return false;
        }

        public static IEnumerable<IStorage> GetStorages(this Room room, bool network = true)
        {
            if (network)
            {
                var controllers = room.GetControllers();
                foreach (var linker in LCache.GetLCache(room.Map).GetActiveLinkers())
                {
                    if (controllers.Any(controller => 
                        controller is INetworkDevice device
                        && linker.LinkTargetID == device.NetworkID
                        && controller.Thing.IsActive()))
                    {
                        Room room2 = linker.Thing.GetRoom();
                        if (LogisticsSystem.IsAvailableSystem(room2))
                        {
                            foreach (var storage in room2.GetStorages(false))
                                yield return storage;
                        }
                    }
                }
            }

            foreach (var storage in LCache.GetLCache(room.Map).GetStorages())
                if (storage.Thing.IsInRoom(room))
                    yield return storage;
        }

        public static IEnumerable<IController> GetControllers(this Room room)
        {
            foreach (var controller in LCache.GetLCache(room.Map).GetControllers())
                if (controller.Thing.IsInRoom(room))
                    yield return controller;
        }

        public static IEnumerable<INetworkLinker> GetNetworkLinkers(this Room room)
        {
            foreach (var linker in LCache.GetLCache(room.Map).GetNetworkLinkers())
                if (linker.Thing.IsInRoom(room))
                    yield return linker;
        }

        public static IEnumerable<ITerminal> GetInputTerminals(this Room room)
        {
            foreach (var terminal in LCache.GetLCache(room.Map).GetInputTerminals())
                if (terminal.Thing.IsInRoom(room))
                    yield return terminal;
        }

        public static IEnumerable<ITerminal> GetOutputTerminals(this Room room)
        {
            foreach (var terminal in LCache.GetLCache(room.Map).GetOutputTerminals())
                if (terminal.Thing.IsInRoom(room))
                    yield return terminal;
        }

        public static IEnumerable<IConveyorPort> GetConveyorPorts(this Room room)
        {
            foreach (var port in LCache.GetLCache(room.Map).GetConveyorPorts())
                if (port.Thing.IsInRoom(room))
                    yield return port;
        }

        public static IEnumerable<INetworkDevice> GetRemoteInputTerminals(this Map map)
        {
            foreach (var terminal in LCache.GetLCache(map).GetInputTerminals())
                if (terminal is INetworkDevice device)
                    yield return device;
        }

        public static IEnumerable<INetworkDevice> GetRemoteOutputTerminals(this Map map)
        {
            foreach (var terminal in LCache.GetLCache(map).GetOutputTerminals())
                if (terminal is INetworkDevice device)
                    yield return device;
        }

        public static IEnumerable<INetworkLinker> GetActiveLinkers(this Room room)
        {
            foreach (var linker in room.GetNetworkLinkers())
                if (linker.Thing.IsActive())
                    yield return linker;
        }

        public static IController GetControllerWithID(this Map map, string ID)
        {
            foreach (var controller in LCache.GetLCache(map).GetControllers())
                if (controller is INetworkDevice device && device.NetworkID == ID)
                    return controller;
            return null;
        }

        public static IController GetActiveControllerWithID(this Map map, string ID)
        {
            foreach (var controller in LCache.GetLCache(map).GetControllers())
                if (controller is INetworkDevice device && device.NetworkID == ID)
                    if (controller != null && !controller.Thing.GetRoom().PsychologicallyOutdoors && controller.Thing.IsActive())
                        return controller;
            return null;
        }
        public static IEnumerable<INetworkLinker> GetActiveLinkers(this Map map, string ID)
        {
            foreach (var linker in LCache.GetLCache(map).GetActiveLinkers())
                if (linker.LinkTargetID == ID)
                    yield return linker;
                    
        }
    }
}
