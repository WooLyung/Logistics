using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Logistics
{
    public static class LCache
    {
        public class MapCache
        {
            private HashSet<INetworkDevice> networkDevices = new HashSet<INetworkDevice>();
            private HashSet<INetworkLinker> networkLinkers = new HashSet<INetworkLinker>();
            private HashSet<IController> controllers = new HashSet<IController>();
            private HashSet<IStorage> storages = new HashSet<IStorage>();
            private HashSet<ITerminal> inputTerminals = new HashSet<ITerminal>();
            private HashSet<ITerminal> outputTerminals = new HashSet<ITerminal>();
            private HashSet<IConveyorPort> conveyorPorts = new HashSet<IConveyorPort>();

            public IEnumerable<IStorage> GetStorages()
            {
                foreach (var storage in storages)
                    yield return storage;
            }

            public IEnumerable<ITerminal> GetInputTerminals()
            {
                foreach (var inputTerminal in inputTerminals)
                    yield return inputTerminal;
            }

            public IEnumerable<ITerminal> GetOutputTerminals()
            {
                foreach (var outputTerminal in outputTerminals)
                    yield return outputTerminal;
            }


            public IEnumerable<INetworkDevice> GetNetworkDevices()
            {
                foreach (var device in networkDevices)
                    yield return device;
            }

            public IEnumerable<IConveyorPort> GetConveyorPorts()
            {
                foreach (var port in conveyorPorts)
                    yield return port;
            }
           
            public IEnumerable<IController> GetControllers()
            {
                foreach (var controller in controllers)
                    yield return controller;
            }

            public IEnumerable<INetworkLinker> GetNetworkLinkers()
            {
                foreach (var linker in networkLinkers)
                    yield return linker;
            }

            public IEnumerable<INetworkLinker> GetActiveLinkers()
            {
                foreach (var linker in networkLinkers)
                    if (linker.Thing.IsActive())
                        yield return linker;
            }


            public void AddNetworkDevice(INetworkDevice networkDevice)
            {
                networkDevices.Add(networkDevice);
            }

            public void AddNetworkLinker(INetworkLinker networkLinker)
            {
                networkLinkers.Add(networkLinker);
            }

            public void AddController(IController controller)
            {
                controllers.Add(controller);
            }

            public void AddStorage(IStorage storage)
            {
                storages.Add(storage);
            }

            public void AddTerminal(ITerminal terminal)
            {
                if (terminal.TermType != TerminalType.Input)
                    outputTerminals.Add(terminal);
                if (terminal.TermType != TerminalType.Output)
                    inputTerminals.Add(terminal);
            }

            public void AddConveyorPort(IConveyorPort port)
            {
                conveyorPorts.Add(port);
            }

            public void RemoveConveyorPort(IConveyorPort port)
            {
                conveyorPorts.Remove(port);
            }

            public void RemoveTerminal(ITerminal terminal)
            {
                inputTerminals.Remove(terminal);
                outputTerminals.Remove(terminal);
            }

            public void RemoveNetworkDevice(INetworkDevice networkDevice)
            { 
                networkDevices.Remove(networkDevice); 
            }

            public void RemoveNetworkLinker(INetworkLinker networkLinker)
            {
                networkLinkers.Remove(networkLinker);
            }

            public void RemoveController(IController controller)
            {
                controllers.Remove(controller);
            }

            public void RemoveStorage(IStorage storage)
            {
                storages.Remove(storage);
            }
        }

        private static Dictionary<Map, MapCache> caches = new Dictionary<Map, MapCache>();

        public static void UpdateCaches(Map map)
        {
            var maps = caches.Keys.ToList();
            foreach (Map map0 in maps)
                if (map0.Disposed)
                    caches.Remove(map0);

            if (!caches.ContainsKey(map))
                caches.Add(map, new MapCache());
        }

        public static MapCache GetLCache(Map map)
        {
            UpdateCaches(map);
            return caches[map];
        }
    }
}
