using UnityEngine;

namespace OverhaulAPI
{
    public class AddedWeaponModel : WeaponModel
    {
        public ModelOffset ModelOffset { get; internal set; }
        public MechBodyPartType BodyPartType { get; internal set; }

        public virtual void Initialize(FirstPersonMover newOwner) { }

        public virtual void ApplyColor(Color color) { }
    }
}