using RimWorld;
using System.Collections.Generic;
using Verse;

namespace Logistics
{
    public class Building_VerticalStorage : Building, IStorage, IStoreSettingsParent, IStorageGroupMember
    {
        private StorageSettings storageSettings;
        private StorageGroup storageGroup;
        private Comp_VerticalStorage comp;

        private Comp_VerticalStorage Comp => (comp = comp ?? GetComp<Comp_VerticalStorage>());
        public bool StorageTabVisible => true;
        public bool IsActive => this.IsActive();

        public StorageSettings GetParentStoreSettings()
        {
            StorageSettings fixedStorageSettings = def.building.fixedStorageSettings;
            if (fixedStorageSettings != null)
                return fixedStorageSettings;
            return StorageSettings.EverStorableFixedSettings();
        }
        public StorageSettings GetStoreSettings()
        {
            if (storageGroup != null)
                return storageGroup.GetStoreSettings();
            return storageSettings;
        }
        public Thing Thing => this;

        public IEnumerable<Thing> StoredThings => Comp.innerContainer;

        StorageGroup IStorageGroupMember.Group
        {
            get => storageGroup;
            set => storageGroup = value;
        }
        StorageSettings IStorageGroupMember.StoreSettings => GetStoreSettings();
        StorageSettings IStorageGroupMember.ParentStoreSettings => GetParentStoreSettings();
        StorageSettings IStorageGroupMember.ThingStoreSettings => storageSettings;
        string IStorageGroupMember.StorageGroupTag => def.building.storageGroupTag;
        bool IStorageGroupMember.DrawConnectionOverlay => Spawned;
        bool IStorageGroupMember.DrawStorageTab => true;
        bool IStorageGroupMember.ShowRenameButton => false;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            LCache.GetLCache(map).AddStorage(this);
            if (storageSettings == null)
                storageSettings = new StorageSettings(this);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref storageSettings, "storageSettings", this);
            Scribe_References.Look(ref storageGroup, "storageGroup");
        }

        public void Notify_SettingsChanged()
        {
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            LCache.GetLCache(Map).RemoveStorage(this);
            base.DeSpawn(mode);
        }

        public Thing GetAnyStack(ThingFilter filter1, StorageSettings filter2)
        {
            foreach (Thing thing in StoredThings)
                if (thing.stackCount > 0 && (filter1 == null || filter1.Allows(thing)) && (filter2 == null || filter2.AllowedToAccept(thing)))
                    return thing;
            return null;
        }

        public int TryConsume(ThingFilter filter1, StorageSettings filter2, int max)
        {
            int cnt = 0;
            while (max > 0)
            {
                Thing thing = GetAnyStack(filter1, filter2);
                if (thing == null)
                    return cnt;

                if (thing.stackCount >= max)
                {
                    thing.SplitOff(max).Destroy();
                    cnt += max;
                    return cnt;
                }
                else
                {
                    max -= thing.stackCount;
                    cnt += thing.stackCount;
                    thing.Destroy();
                }
            }
            return cnt;
        }

        public bool HasThing(Thing thing)
        {
            foreach (Thing t in StoredThings)
                if (thing == t)
                    return true;
            return false;
        }

        public bool TryInsert(Thing thing, out int remained)
        {
            if (!GetStoreSettings().AllowedToAccept(thing))
            {
                remained = thing.stackCount;
                return false;
            }
            return Comp.TryInsert(thing, out remained);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
                yield return gizmo;

            foreach (Gizmo item in StorageSettingsClipboard.CopyPasteGizmosFor(GetStoreSettings()))
                yield return item;

            if (!StorageTabVisible || base.MapHeld == null)
                yield break;

            foreach (Gizmo item2 in StorageGroupUtility.StorageGroupMemberGizmos(this))
                yield return item2;
        }
    }
}
