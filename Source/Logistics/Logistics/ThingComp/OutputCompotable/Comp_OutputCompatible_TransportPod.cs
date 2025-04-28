using RimWorld;
using Verse;

namespace Logistics
{
    public class Comp_OutputCompatible_TransportPod : ThingComp, IComp_OutputCompotable
    {
        private CompTransporter transporter;
        public CompTransporter Transporter
        {
            get
            {
                if (transporter == null)
                    transporter = parent.GetComp<CompTransporter>();
                return transporter;
            }
        }

        public bool TryInsert(Room room)
        {
            return false; 
        }
    }
}
