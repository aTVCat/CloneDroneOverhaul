using ModLibrary;
using System.Collections;
using UnityEngine;

namespace OverhaulAPI
{
    public class AddedWeaponModel : WeaponModel
    {
        public ModelOffset ModelOffset { get; internal set; }
        public MechBodyPartType BodyPartType { get; internal set; }

        internal void SetWeaponType(WeaponType type)
        {          
            if((int)type < WeaponsAdder.Start_WeaponType_Index)
            {
                API.ThrowException("ArgumentException: WeaponType index should be more than 15." + (int)type + " was given");
            }
            base.WeaponType = type;
        }
    }
}