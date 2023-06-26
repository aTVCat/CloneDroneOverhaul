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
        public Vector3 OffsetTargetPosition;
        public float LerpMultiplier;

        /// <summary>
        /// Stuff for editor
        /// </summary>
        /// <returns></returns>
        public static FieldInfo[] GetAllFields() => typeof(PetBehaviourSettings).GetFields(BindingFlags.Instance | BindingFlags.Public);
    }
}
