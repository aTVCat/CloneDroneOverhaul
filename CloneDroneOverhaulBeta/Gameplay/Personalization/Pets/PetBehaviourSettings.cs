using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

        public float LerpMultiplier;

        public bool FollowHeadRotation;

        /// <summary>
        /// Stuff for editor
        /// </summary>
        /// <returns></returns>
        public static FieldInfo[] GetAllFields() => typeof(PetBehaviourSettings).GetFields(BindingFlags.Instance | BindingFlags.Public);
    }
}
