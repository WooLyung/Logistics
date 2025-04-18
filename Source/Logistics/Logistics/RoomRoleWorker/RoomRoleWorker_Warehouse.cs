using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public class RoomRoleWorker_Warehouse : RoomRoleWorker
    {
        public override float GetScore(Room room)
        {
            List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
            for (int i = 0; i < containedAndAdjacentThings.Count; i++)
                if (containedAndAdjacentThings[i] is IController)
                    return 10100f;

            return 0f;
        }
    }
}
