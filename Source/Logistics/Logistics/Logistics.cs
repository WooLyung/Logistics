using HarmonyLib;
using System.Reflection;
using Verse;

namespace Logistics
{
    [StaticConstructorOnStartup]
    public class Logistics
    {
        private static Harmony harmony;

        static Logistics()
        {
            harmony = new Harmony("ng.lyu.logistics");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
