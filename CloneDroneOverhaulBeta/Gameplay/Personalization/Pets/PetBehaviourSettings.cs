using System;
using System.Reflection;
using UnityEngine;

namespace CDOverhaul.Gameplay.Pets
{
    /// <summary>
    /// Customize pet's behaviour
    /// </summary>
    public class PetBehaviourSettings
    {
        public Tuple<string, Vector3>[] OffsetTargetPositionNodes;
        public Vector3 OffsetTargetRotation;
        public Vector3 OffsetScale = Vector3.one;

        public float FlyEffectMultiplier1 = 1.25f;
        public float FlyEffectMultiplier2 = 0.025f;

        public float RangeToLookAtEnemy = -1;

        public bool FollowHeadRotation;

        /// <summary>
        /// Stuff for editor
        /// </summary>
        /// <returns></returns>
        public static FieldInfo[] GetAllFields() => typeof(PetBehaviourSettings).GetFields(BindingFlags.Instance | BindingFlags.Public);
    }
}
