using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class WeaponSkinModels
    {
        /// <summary>
        /// Directory where all previews are stored
        /// </summary>
        public static string SkinPreviewDirectory = OverhaulMod.Core.ModFolder + "Skins/Previews/";

        public (GameObject, GameObject) Normal;
        public (GameObject, GameObject) Fire;
        public (GameObject, GameObject) EMP;

        /// <summary>
        /// The path where you can find weapon skin preview
        /// </summary>
        public string SkinPreviewPath { get; set; }

        /// <summary>
        /// The skin name
        /// </summary>
        public string SkinName { get; set; }

        /// <summary>
        /// The weapon the skin is made for
        /// </summary>
        public WeaponType WeaponType { get; set; }

        public virtual void SetNormalModel(in GameObject @object, in bool multiplayer)
        {
            if (multiplayer)
            {
                Normal.Item2 = @object;
                return;
            }
            Normal.Item1 = @object;
        }

        public virtual void SetFireModel(in GameObject @object, in bool multiplayer)
        {
            if (multiplayer)
            {
                Fire.Item2 = @object;
                return;
            }
            Fire.Item1 = @object;
        }

        public virtual void SetEMPModel(in GameObject @object, in bool multiplayer)
        {
            if (multiplayer)
            {
                EMP.Item2 = @object;
                return;
            }
            EMP.Item1 = @object;
        }

        public virtual GameObject GetNormalModel(in bool multiplayer)
        {
            if (multiplayer)
            {
                if (Normal.Item2 == null)
                {
                    return Normal.Item1;
                }
                return Normal.Item2;
            }
            return Normal.Item1;
        }

        public virtual GameObject GetFireModel(in bool multiplayer)
        {
            if (multiplayer)
            {
                if (Fire.Item2 == null)
                {
                    if (Fire.Item1 == null)
                    {
                        return GetNormalModel(multiplayer);
                    }
                    return Fire.Item1;
                }
                return Fire.Item2;
            }
            if (Fire.Item1 == null)
            {
                return GetNormalModel(multiplayer);
            }
            return Fire.Item1;
        }

        public virtual GameObject GetEMPModel(in bool multiplayer)
        {
            if (multiplayer)
            {
                if (Normal.Item2 == null)
                {
                    return Normal.Item1;
                }
                return Normal.Item2;
            }
            return Normal.Item1;
        }

        public virtual GameObject GetModel(in bool fire, in bool multiplayer)
        {
            if (fire)
            {
                return GetFireModel(multiplayer);
            }
            return GetNormalModel(multiplayer);
        }

        public virtual WeaponSkinPlacement GetPlacement()
        {
            return MainGameplayController.Instance.WeaponSkins.GetSkinPlacement(this);
        }
    }
}
