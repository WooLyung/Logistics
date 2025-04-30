using RimWorld;
using System.Collections.Generic;
using System.Text;
using Verse;

namespace Logistics
{
    public class Building_RemoteInputInterface : Building, IStoreSettingsParent, INetworkDevice
    {
        private StorageSettings storageSettings;
        private string networkID;

        public Thing Thing => this;
        public bool StorageTabVisible => true;
        public string DefaultID => "RemoteInputInterface_" + Rand.Range(1000, 9999);
        public string NetworkID
        {
            get => networkID;
            set => networkID = value;
        }

        public Building_RemoteInputInterface()
        {
            networkID = DefaultID;
        }

        public StorageSettings GetParentStoreSettings()
        {
            StorageSettings fixedStorageSettings = def.building.fixedStorageSettings;
            if (fixedStorageSettings != null)
                return fixedStorageSettings;
            return StorageSettings.EverStorableFixedSettings();
        }
        public StorageSettings GetStoreSettings() => storageSettings;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            LCache.GetLCache(map).AddNetworkDevice(this);
            if (storageSettings == null)
                storageSettings = new StorageSettings(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref storageSettings, "storageSettings", this);
            Scribe_Values.Look(ref networkID, "NetworkID", DefaultID);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var g in base.GetGizmos()) yield return g;

            yield return new Command_Action
            {
                defaultLabel = "NetworkIDLabel".Translate(),
                defaultDesc = "NetworkIDDesc".Translate(),
                icon = TexButton.Rename,
                action = () => {
                    Find.WindowStack.Add(new Dialog_RenameController(this));
                }
            };
        }

        public override string GetInspectString()
        {
            StringBuilder sb = new StringBuilder();

            string baseStr = base.GetInspectString();
            if (!baseStr.NullOrEmpty())
                sb.AppendLine(baseStr);

            sb.AppendLine($"{"NetworkID".Translate()}: {networkID}");

            return sb.ToString().TrimEndNewlines();
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            LCache.GetLCache(Map).RemoveNetworkDevice(this);
            base.DeSpawn(mode);
        }

        public void Notify_SettingsChanged()
        {
        }

        public override void Tick()
        {
            base.Tick();

            if (this.IsHashIntervalTick(500) && this.IsActive())
                Translate();
        }

        private void Translate()
        {
            var linkers = ThingFinder.GetActiveLinkers(Map, networkID);
            foreach (var linker in linkers)
            {
                Room to = linker.Thing.GetRoom();
                if (!LogisticsSystem.IsAvailableSystem(to))
                    continue;

                foreach (Thing target in (Position + Rotation.FacingCell).GetThingList(Map))
                {
                    if (target is ThingWithComps target2)
                    {
                        foreach (ThingComp comp in target2.AllComps)
                            if (comp is IComp_InputCompotable compotable)
                                if (compotable.TryExtract(to, false))
                                    return;
                    }
                }
            }
        }
    }
}
