using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using Verse;

namespace Logistics
{
    public class ITab_LogisticsFilter : ITab_Storage
    {
        protected override bool IsPrioritySettingVisible => false;

        public ITab_LogisticsFilter()
        {
            labelKey = "LogisticsFilter";
        }
    }
}
