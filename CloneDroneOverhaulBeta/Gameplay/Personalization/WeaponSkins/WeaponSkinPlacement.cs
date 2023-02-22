using UnityEngine;

namespace CDOverhaul.Gameplay
{
    /// <summary>
    /// The target position of weapon skin
    /// </summary>
    public class WeaponSkinPlacement
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;

        public WeaponSkinPlacement(in Vector3 pos, in Vector3 rot, in Vector3 sca)
        {
            Position = pos;
            Rotation = rot;
            Scale = sca;
        }
    }
}
