﻿using CloneDroneOverhaul.Utilities;

namespace CloneDroneOverhaul.EffectsAndAbilitiesV4
{
    public static class RobotsExpansionManager
    {
        public class SpecialAbilityCollection
        {
            public FirstPersonMover Owner;


        }

        public static void OnRobotSpawned(RobotShortInformation mover)
        {
            SpecialAbilityCollection collection = new SpecialAbilityCollection();
            collection.Owner = (FirstPersonMover)mover.Instance;

            OverhaulCacheManager.AddTemporalObject<SpecialAbilityCollection>(collection, "SpecialAbilitiesCollection_" + mover.Instance.GetInstanceID());
        }
        public static void OnRobotDestroyed(RobotShortInformation mover)
        {
            OverhaulCacheManager.RemoveTemporalObject("SpecialAbilitiesCollection_" + mover.Instance.GetInstanceID());
        }
        public static void FixedUpdate()
        {

        }
    }
}
