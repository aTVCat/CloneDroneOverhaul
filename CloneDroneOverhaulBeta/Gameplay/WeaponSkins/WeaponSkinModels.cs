using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinModels
    {
        public (GameObject, GameObject) Normal;
        public (GameObject, GameObject) Fire;
        public (GameObject, GameObject) EMP;

        public void SetNormalModel(in GameObject @object, in bool multiplayer)
        {
            if (multiplayer)
            {
                Normal.Item2 = @object;
                return;
            }
            Normal.Item1 = @object;
        }

        public void SetFireModel(in GameObject @object, in bool multiplayer)
        {
            if (multiplayer)
            {
                Fire.Item2 = @object;
                return;
            }
            Fire.Item1 = @object;
        }

        public void SetEMPModel(in GameObject @object, in bool multiplayer)
        {
            if (multiplayer)
            {
                EMP.Item2 = @object;
                return;
            }
            EMP.Item1 = @object;
        }
    }
}
