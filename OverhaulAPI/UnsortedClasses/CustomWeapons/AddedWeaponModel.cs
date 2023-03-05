using ModLibrary;
using System.Collections;
using UnityEngine;

namespace OverhaulAPI
{
    public class AddedWeaponModel : WeaponModel
    {
        public ModelOffset ModelOffset { get; internal set; }
        public MechBodyPartType BodyPartType { get; internal set; }
    }
}