using CDOverhaul.Gameplay.Editors.Personalization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay.Pets
{
    /// <summary>
    /// Customize pet's behaviour
    /// </summary>
    public class PetBehaviourSettings
    {
        [PersonalizationEditorProperty("Behaviour")]
        public List<Tuple<string, Vector3>> OffsetTargetPositionNodes;

        [PersonalizationEditorProperty("Behaviour")]
        public Vector3 OffsetTargetRotation;
        [PersonalizationEditorProperty("Behaviour")]
        public Vector3 OffsetScale = Vector3.one;

        [PersonalizationEditorProperty("Behaviour")]
        public float FlyEffectMultiplier1 = 1.25f;
        [PersonalizationEditorProperty("Behaviour")]
        public float FlyEffectMultiplier2 = 0.025f;

        [PersonalizationEditorProperty("Behaviour")]
        public float RangeToLookAtEnemy = -1;

        [PersonalizationEditorProperty("Behaviour")]
        public bool FollowHeadRotation;
    }
}
