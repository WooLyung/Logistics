using RimWorld;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Verse;

namespace Logistics
{
    using Dev = Building_ConveyorDevice;
    using Conveyor = Building_Conveyor;

    public static class ConveyorSystem
    {
        private static Dictionary<Map, bool> isUpdated
            = new Dictionary<Map, bool>();
        private static Dictionary<Map, Dictionary<Dev, List<Dev>>> i2o
            = new Dictionary<Map, Dictionary<Dev, List<Dev>>>();
        private static Dictionary<Map, Dictionary<Dev, List<Dev>>> o2i
            = new Dictionary<Map, Dictionary<Dev, List<Dev>>>();
        private static Dictionary<Map, List<Conveyor>> conveyors
            = new Dictionary<Map, List<Conveyor>>();

        private static void ResetMaps(Map map)
        {
            foreach (Map map0 in i2o.Keys)
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
                conveyors.Add(map, new List<Conveyor>());
            if (!isUpdated.ContainsKey(map))
                isUpdated.Add(map, false);
        }

        private static int GetTotalInputs(Map map, Conveyor conveyor, HashSet<Dev> inputs)
        {
            int totalInputs = 0;
            foreach (Thing thing in (conveyor.Position + IntVec3.North).GetThingList(map))
                if (thing is Conveyor neighbor && neighbor.Rotation.FacingCell == IntVec3.South
                    || thing is Dev input && inputs.Contains(input) && (input.InputDir == Dev.Dir.ALL || input.InputDir == Dev.Dir.SOUTH))
                    totalInputs++;
            foreach (Thing thing in (conveyor.Position + IntVec3.South).GetThingList(map))
                if (thing is Conveyor neighbor && neighbor.Rotation.FacingCell == IntVec3.North
                    || thing is Dev input && inputs.Contains(input) && (input.InputDir == Dev.Dir.ALL || input.InputDir == Dev.Dir.NORTH))
                    totalInputs++;
            foreach (Thing thing in (conveyor.Position + IntVec3.East).GetThingList(map))
                if (thing is Conveyor neighbor && neighbor.Rotation.FacingCell == IntVec3.West
                    || thing is Dev input && inputs.Contains(input) && (input.InputDir == Dev.Dir.ALL || input.InputDir == Dev.Dir.EAST))
                    totalInputs++;
            foreach (Thing thing in (conveyor.Position + IntVec3.West).GetThingList(map))
                if (thing is Conveyor neighbor && neighbor.Rotation.FacingCell == IntVec3.East
                    || thing is Dev input && inputs.Contains(input) && (input.InputDir == Dev.Dir.ALL || input.InputDir == Dev.Dir.WEST))
                    totalInputs++;

            return totalInputs;
        }

        private static void DFS(Map map, HashSet<Dev> from, Conveyor conveyor, HashSet<Dev> inputs, HashSet<Dev> outputs, Dictionary<Conveyor, HashSet<Dev>> conveyorInputs, Dictionary<Conveyor, int> inputCount)
        {
            if (!conveyorInputs.ContainsKey(conveyor))
                conveyorInputs.Add(conveyor, new HashSet<Dev>());
            conveyorInputs[conveyor].AddRange(from);

            if (!inputCount.ContainsKey(conveyor))
                inputCount.Add(conveyor, 0);
            inputCount[conveyor]++;

            if (GetTotalInputs(map, conveyor, inputs) <= inputCount[conveyor])
            {
                foreach (Thing thing in (conveyor.Position + conveyor.Rotation.FacingCell).GetThingList(map))
                {
                    if (thing is Conveyor neighbor)
                        DFS(map, conveyorInputs[conveyor], neighbor, inputs, outputs, conveyorInputs, inputCount);
                    else if (thing is Dev output && outputs.Contains(output))
                    {
                        if (output.OutputDir == Dev.Dir.ALL
                            || conveyor.Rotation.FacingCell == IntVec3.North && output.OutputDir == Dev.Dir.SOUTH
                            || conveyor.Rotation.FacingCell == IntVec3.North && output.OutputDir == Dev.Dir.SOUTH
                            || conveyor.Rotation.FacingCell == IntVec3.North && output.OutputDir == Dev.Dir.SOUTH
                            || conveyor.Rotation.FacingCell == IntVec3.North && output.OutputDir == Dev.Dir.SOUTH)
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
            Dictionary<Conveyor, HashSet<Dev>> conveyorInputs = new Dictionary<Conveyor, HashSet<Dev>>();
            Dictionary<Conveyor, int> inputCount = new Dictionary<Conveyor, int>();

            foreach (Conveyor conveyor in conveyors[map])
                if (GetTotalInputs(map, conveyor, inputs) == 0)
                    DFS(map, new HashSet<Dev>(), conveyor, inputs, outputs, conveyorInputs, inputCount);

            foreach (Dev input in inputs)
            {
                if (input.InputDir == Dev.Dir.ALL || input.InputDir == Dev.Dir.NORTH)
                    foreach (Thing thing in (input.Position + IntVec3.North).GetThingList(map))
                        if (thing is Conveyor conveyor)
                            DFS(map, new HashSet<Dev> { input }, conveyor, inputs, outputs, conveyorInputs, inputCount);
                        else if (thing is Dev output && outputs.Contains(output) && (output.OutputDir == Dev.Dir.ALL || output.OutputDir == Dev.Dir.SOUTH))
                            o2i[map][output].Add(input);
                if (input.InputDir == Dev.Dir.ALL || input.InputDir == Dev.Dir.SOUTH)
                    foreach (Thing thing in (input.Position + IntVec3.South).GetThingList(map))
                        if (thing is Conveyor conveyor)
                            DFS(map, new HashSet<Dev> { input }, conveyor, inputs, outputs, conveyorInputs, inputCount);
                        else if (thing is Dev output && outputs.Contains(output) && (output.OutputDir == Dev.Dir.ALL || output.OutputDir == Dev.Dir.NORTH))
                            o2i[map][output].Add(input);
                if (input.InputDir == Dev.Dir.ALL || input.InputDir == Dev.Dir.EAST)
                    foreach (Thing thing in (input.Position + IntVec3.East).GetThingList(map))
                        if (thing is Conveyor conveyor)
                            DFS(map, new HashSet<Dev> { input }, conveyor, inputs, outputs, conveyorInputs, inputCount);
                        else if (thing is Dev output && outputs.Contains(output) && (output.OutputDir == Dev.Dir.ALL || output.OutputDir == Dev.Dir.WEST))
                            o2i[map][output].Add(input);
                if (input.InputDir == Dev.Dir.ALL || input.InputDir == Dev.Dir.WEST)
                    foreach (Thing thing in (input.Position + IntVec3.West).GetThingList(map))
                        if (thing is Conveyor conveyor)
                            DFS(map, new HashSet<Dev> { input }, conveyor, inputs, outputs, conveyorInputs, inputCount);
                        else if (thing is Dev output && outputs.Contains(output) && (output.OutputDir == Dev.Dir.ALL || output.OutputDir == Dev.Dir.EAST))
                            o2i[map][output].Add(input);
            }

            foreach (Dev output in o2i[map].Keys)
                foreach (Dev input in o2i[map][output])
                    i2o[map][input].Add(output);

            isUpdated[map] = true;
        }

        public static void AddConveyor(Map map, Conveyor conveyor, bool updated)
        {
            ResetMaps(map);
            conveyors[map].Add(conveyor);
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

        public static void RemoveConveyor(Map map, Conveyor conveyor)
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
            Map map = output.Map;

            ResetMaps(map);
            if (!isUpdated[map])
                CalculateConveyorSystem(map);

            foreach (Dev dev in o2i[map][output])
                yield return dev;
        }

        public static IEnumerable<Dev> GetOutputs(Dev input)
        {
            Map map = input.Map;

            ResetMaps(map);
            if (!isUpdated[map])
                CalculateConveyorSystem(map);

            foreach (Dev dev in i2o[map][input])
                yield return dev;
        }
    }
}
