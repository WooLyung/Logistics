using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace Logistics
{
    public class Building_AutoTransferNode : Building, INetworkDevice, IStoreSettingsParent
    {
        private string networkID;
        private int scanRadius = 3;
        private const int MinScanRadius = 1;
        private const int MaxScanRadius = 6;
        private StorageSettings storageSettings;

        public Thing Thing => this;
        public bool StorageTabVisible => true;
        StorageSettings IStoreSettingsParent.GetStoreSettings() => storageSettings;
        public StorageSettings GetParentStoreSettings()
        {
            StorageSettings fixedStorageSettings = def.building.fixedStorageSettings;
            if (fixedStorageSettings != null)
                return fixedStorageSettings;
            return StorageSettings.EverStorableFixedSettings();
        }

        public string DefaultID => "TransferNode_" + Rand.Range(1000, 9999);
        public string NetworkID
        {
            get => networkID;
            set => networkID = value;
        }

        public Building_AutoTransferNode()
        {
            networkID = DefaultID;
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            LCache.GetLCache(map).AddNetworkDevice(this);
            if (storageSettings == null)
                storageSettings = new StorageSettings(this);
        }

        public override void Tick()
        {
            base.Tick();

            if (this.IsHashIntervalTick(500) && this.IsActive())
                Translate();
        }

        private void Translate()
        {
            List<Room> rooms = new List<Room>();
            foreach (var linker in ThingFinder.GetActiveLinkers(Map, networkID))
            {
                Room room = linker.Thing.GetRoom();
                if (LogisticsSystem.IsAvailableSystem(room))
                    rooms.Add(room);
            }

            foreach (IntVec3 cell in GenRadial.RadialCellsAround(Position, scanRadius, useCenter: true))
            {
                var thingList = cell.GetThingList(Map);
                foreach (Thing thing in thingList)
                    if (thing.def.EverHaulable && storageSettings.AllowedToAccept(thing))
                        foreach (var room in rooms)
                            if (Translator.ToStorageAny(thing, room, false))
                                return;
            }
        }

        public override void DrawExtraSelectionOverlays()
        {
            base.DrawExtraSelectionOverlays();
            GenDraw.DrawFieldEdges(GenRadial.RadialCellsAround(Position, scanRadius, true).ToList());
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref networkID, "NetworkID", DefaultID);
            Scribe_Values.Look(ref scanRadius, "scanRadius", 3);
            Scribe_Deep.Look(ref storageSettings, "storageSettings", this);
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

            yield return new Command_Action
            {
                defaultLabel = "ScanRadiusLabel".Translate(),
                defaultDesc = "ScanRadiusDesc".Translate(),
                icon = ContentFinder<Texture2D>.Get("UI/Commands/RenameZone"),
                action = () =>
                {
                    Find.WindowStack.Add(new Dialog_Slider("ScanRadiusEnter".Translate(), MinScanRadius, MaxScanRadius, x => {
                        scanRadius = x;
                        Messages.Message("ScanRadiusMessage".Translate(), MessageTypeDefOf.NeutralEvent);
                    }, scanRadius));
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
            sb.AppendLine($"{"ScanRadius".Translate()}: {scanRadius}");

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
    }
}
