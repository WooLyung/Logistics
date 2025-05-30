﻿using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Logistics
{
    using Dev = IConveyorDevice;
    using Conv = IConveyor;

    public static class ConveyorSystem
    {
        private static Dictionary<Map, bool> isUpdated
            = new Dictionary<Map, bool>();
        private static Dictionary<Map, Dictionary<Dev, List<Dev>>> i2o
            = new Dictionary<Map, Dictionary<Dev, List<Dev>>>();
        private static Dictionary<Map, Dictionary<Dev, List<Dev>>> o2i
            = new Dictionary<Map, Dictionary<Dev, List<Dev>>>();
        private static Dictionary<Map, List<Conv>> conveyors
            = new Dictionary<Map, List<Conv>>();

        private static void ResetMaps(Map map)
        {
            var maps = isUpdated.Keys.ToList();
            foreach (Map map0 in maps)
            {
                if (map0.Disposed)
                {
                    i2o.Remove(map0);
                    o2i.Remove(map0);
                    conveyors.Remove(map0);
                    isUpdated.Remove(map0);
                }
            }

            if (!i2o.ContainsKey(map))
                i2o.Add(map, new Dictionary<Dev, List<Dev>>());
            if (!o2i.ContainsKey(map))
                o2i.Add(map, new Dictionary<Dev, List<Dev>>());
            if (!conveyors.ContainsKey(map))
                conveyors.Add(map, new List<Conv>());
            if (!isUpdated.ContainsKey(map))
                isUpdated.Add(map, false);
        }

        private static int GetTotalInputs(Map map, Conv conveyor, HashSet<Dev> inputs)
        {
            int totalInputs = 0;
            foreach (Thing thing in (conveyor.Thing.Position + IntVec3.North).GetThingList(map))
                if (thing is Conv neighbor && neighbor.Thing.Rotation.FacingCell == IntVec3.South
                    || thing is Dev input && inputs.Contains(input) && (input.InputDir == ConveyorDeviceDir.All || input.InputDir == ConveyorDeviceDir.South))
                    totalInputs++;
            foreach (Thing thing in (conveyor.Thing.Position + IntVec3.South).GetThingList(map))
                if (thing is Conv neighbor && neighbor.Thing.Rotation.FacingCell == IntVec3.North
                    || thing is Dev input && inputs.Contains(input) && (input.InputDir == ConveyorDeviceDir.All || input.InputDir == ConveyorDeviceDir.North))
                    totalInputs++;
            foreach (Thing thing in (conveyor.Thing.Position + IntVec3.East).GetThingList(map))
                if (thing is Conv neighbor && neighbor.Thing.Rotation.FacingCell == IntVec3.West
                    || thing is Dev input && inputs.Contains(input) && (input.InputDir == ConveyorDeviceDir.All || input.InputDir == ConveyorDeviceDir.West))
                    totalInputs++;
            foreach (Thing thing in (conveyor.Thing.Position + IntVec3.West).GetThingList(map))
                if (thing is Conv neighbor && neighbor.Thing.Rotation.FacingCell == IntVec3.East
                    || thing is Dev input && inputs.Contains(input) && (input.InputDir == ConveyorDeviceDir.All || input.InputDir == ConveyorDeviceDir.East))
                    totalInputs++;

            return totalInputs;
        }

        private static void DFS(Map map, HashSet<Dev> from, Conv conveyor, HashSet<Dev> inputs, HashSet<Dev> outputs, Dictionary<Conv, HashSet<Dev>> conveyorInputs, Dictionary<Conv, int> inputCount)
        {
            if (!conveyorInputs.ContainsKey(conveyor))
                conveyorInputs.Add(conveyor, new HashSet<Dev>());
            conveyorInputs[conveyor].AddRange(from);

            if (!inputCount.ContainsKey(conveyor))
                inputCount.Add(conveyor, 0);
            inputCount[conveyor]++;

            if (GetTotalInputs(map, conveyor, inputs) <= inputCount[conveyor])
            {
                foreach (Thing thing in (conveyor.Thing.Position + conveyor.Thing.Rotation.FacingCell).GetThingList(map))
                {
                    if (thing is Conv neighbor)
                        DFS(map, conveyorInputs[conveyor], neighbor, inputs, outputs, conveyorInputs, inputCount);
                    else if (thing is Dev output && outputs.Contains(output))
                    {
                        if (output.OutputDir == ConveyorDeviceDir.All
                            || conveyor.Thing.Rotation.FacingCell == IntVec3.North && output.OutputDir == ConveyorDeviceDir.North
                            || conveyor.Thing.Rotation.FacingCell == IntVec3.South && output.OutputDir == ConveyorDeviceDir.South
                            || conveyor.Thing.Rotation.FacingCell == IntVec3.East && output.OutputDir == ConveyorDeviceDir.East
                            || conveyor.Thing.Rotation.FacingCell == IntVec3.West && output.OutputDir == ConveyorDeviceDir.West)
                            o2i[map][output].AddRange(conveyorInputs[conveyor]);
                    }
                }
            }
        }

        public static void CalculateConveyorSystem(Map map)
        {
            foreach (Dev dev in i2o[map].Keys)
                i2o[map][dev].Clear();
            foreach (Dev dev in o2i[map].Keys)
                o2i[map][dev].Clear();

            HashSet<Dev> inputs = new HashSet<Dev>(i2o[map].Keys);
            HashSet<Dev> outputs = new HashSet<Dev>(o2i[map].Keys);
            Dictionary<Conv, HashSet<Dev>> conveyorInputs = new Dictionary<Conv, HashSet<Dev>>();
            Dictionary<Conv, int> inputCount = new Dictionary<Conv, int>();

            foreach (Conv conveyor in conveyors[map])
                if (GetTotalInputs(map, conveyor, inputs) == 0)
                    DFS(map, new HashSet<Dev>(), conveyor, inputs, outputs, conveyorInputs, inputCount);

            foreach (Dev _input in inputs)
            {
                if (!(_input is Thing))
                    continue;

                Thing input = (Thing)_input;
                if (_input.InputDir == ConveyorDeviceDir.All || _input.InputDir == ConveyorDeviceDir.North)
                    foreach (Thing thing in (input.Position + IntVec3.North).GetThingList(map))
                        if (thing is Conv conveyor)
                            DFS(map, new HashSet<Dev> { _input }, conveyor, inputs, outputs, conveyorInputs, inputCount);
                        else if (thing is Dev output && outputs.Contains(output) && (output.OutputDir == ConveyorDeviceDir.All || output.OutputDir == ConveyorDeviceDir.North))
                            o2i[map][output].Add(_input);
                if (_input.InputDir == ConveyorDeviceDir.All || _input.InputDir == ConveyorDeviceDir.South)
                    foreach (Thing thing in (input.Position + IntVec3.South).GetThingList(map))
                        if (thing is Conv conveyor)
                            DFS(map, new HashSet<Dev> { _input }, conveyor, inputs, outputs, conveyorInputs, inputCount);
                        else if (thing is Dev output && outputs.Contains(output) && (output.OutputDir == ConveyorDeviceDir.All || output.OutputDir == ConveyorDeviceDir.South))
                            o2i[map][output].Add(_input);
                if (_input.InputDir == ConveyorDeviceDir.All || _input.InputDir == ConveyorDeviceDir.East)
                    foreach (Thing thing in (input.Position + IntVec3.East).GetThingList(map))
                        if (thing is Conv conveyor)
                            DFS(map, new HashSet<Dev> { _input }, conveyor, inputs, outputs, conveyorInputs, inputCount);
                        else if (thing is Dev output && outputs.Contains(output) && (output.OutputDir == ConveyorDeviceDir.All || output.OutputDir == ConveyorDeviceDir.East))
                            o2i[map][output].Add(_input);
                if (_input.InputDir == ConveyorDeviceDir.All || _input.InputDir == ConveyorDeviceDir.West)
                    foreach (Thing thing in (input.Position + IntVec3.West).GetThingList(map))
                        if (thing is Conv conveyor)
                            DFS(map, new HashSet<Dev> { _input }, conveyor, inputs, outputs, conveyorInputs, inputCount);
                        else if (thing is Dev output && outputs.Contains(output) && (output.OutputDir == ConveyorDeviceDir.All || output.OutputDir == ConveyorDeviceDir.West))
                            o2i[map][output].Add(_input);
            }

            foreach (Dev output in o2i[map].Keys)
                foreach (Dev input in o2i[map][output])
                    i2o[map][input].Add(output);

            isUpdated[map] = true;
        }

        public static void AddConveyor(Map map, Conv conveyor, bool update)
        {
            ResetMaps(map);
            conveyors[map].Add(conveyor);
            if (update)
                CalculateConveyorSystem(map);
        }

        public static void AddInput(Map map, Dev dev, bool update)
        {
            ResetMaps(map);
            i2o[map].Add(dev, new List<Dev>());
            if (update)
                CalculateConveyorSystem(map);
        }

        public static void AddOutput(Map map, Dev dev, bool update)
        {
            ResetMaps(map);
            o2i[map].Add(dev, new List<Dev>());
            if (update)
                CalculateConveyorSystem(map);
        }

        public static void AddIO(Map map, Dev dev, bool update)
        {
            ResetMaps(map);
            i2o[map].Add(dev, new List<Dev>());
            o2i[map].Add(dev, new List<Dev>());
            if (update)
                CalculateConveyorSystem(map);
        }

        public static void RemoveConveyor(Map map, Conv conveyor)
        {
            ResetMaps(map);
            if (conveyors.ContainsKey(map))
                conveyors[map].Remove(conveyor);
            CalculateConveyorSystem(map);
        }


        public static void RemoveInput(Map map, Dev dev)
        {
            ResetMaps(map);
            if (i2o.ContainsKey(map))
                i2o[map].Remove(dev);
            CalculateConveyorSystem(map);
        }

        public static void RemoveOutput(Map map, Dev dev)
        {
            ResetMaps(map);
            if (i2o.ContainsKey(map))
                i2o[map].Remove(dev);
            CalculateConveyorSystem(map);
        }

        public static void RemoveIO(Map map, Dev dev)
        {
            ResetMaps(map);
            if (i2o.ContainsKey(map))
                i2o[map].Remove(dev);
            if (o2i.ContainsKey(map))
                o2i[map].Remove(dev);
            CalculateConveyorSystem(map);
        }

        public static IEnumerable<Dev> GetInputs(Dev output)
        {
            Map map = output.Thing.Map;

            ResetMaps(map);
            if (!isUpdated[map])
                CalculateConveyorSystem(map);

            foreach (Dev dev in o2i[map][output])
                yield return dev;
        }

        public static IEnumerable<Dev> GetOutputs(Dev input)
        {
            Map map = input.Thing.Map;

            ResetMaps(map);
            if (!isUpdated[map])
                CalculateConveyorSystem(map);

            foreach (Dev dev in i2o[map][input])
                yield return dev;
        }
    }
}
