using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public class RoomRoleWorker_LogisticsIncinerationFacility : RoomRoleWorker
    {
        public override float GetScore(Room room)
        {
            bool a = false, b = true;

            List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
            for (int i = 0; i < containedAndAdjacentThings.Count; i++)
                if (containedAndAdjacentThings[i] is Building_LogisticsIncinerator)
                    a = true;
                else if (containedAndAdjacentThings[i] is IController)
                    b = true;

            if (a && b)
                return 10200;
            return 0f;
        }
    }
}
