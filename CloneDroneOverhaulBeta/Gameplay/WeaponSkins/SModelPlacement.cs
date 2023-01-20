using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public struct SModelPlacement
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }

        public SModelPlacement(in Vector3 pos, in Vector3 rot, in Vector3 sca)
        {
            Position = pos;
            Rotation = rot;
            Scale = sca;
        }
    }
}
