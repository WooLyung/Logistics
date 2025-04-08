using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public class RoomRoleWorker_Warehouse : RoomRoleWorker
    {
        public override float GetScore(Room room)
        {
            int num = 0;
            List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
            for (int i = 0; i < containedAndAdjacentThings.Count; i++)
                if (containedAndAdjacentThings[i] is Building_LogisticsInterfaceBase)
                    num++;

            return 10100f;
        }
    }
}
