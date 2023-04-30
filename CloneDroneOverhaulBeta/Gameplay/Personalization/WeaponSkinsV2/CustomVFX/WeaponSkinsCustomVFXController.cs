using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using OverhaulAPI;

namespace CDOverhaul.Gameplay
{
    public static class WeaponSkinsCustomVFXController
    {
        private static Dictionary<WeaponSkinItemDefinitionV2, string> m_AllVFX = new Dictionary<WeaponSkinItemDefinitionV2, string>();

        public static void PrepareCustomVFXForSkin(WeaponSkinItemDefinitionV2 itemDefinition)
        {
            if (SkinHasCustomVFX(itemDefinition))
            {
                return;
            }

            if(AssetsController.TryGetAsset<GameObject>(itemDefinition.CollideWithEnvironmentVFXAssetName, itemDefinition.OverrideAssetBundle, out GameObject vfx))
            {
                if(vfx == null)
                {
                    return;
                }

                PooledPrefabController.TurnObjectIntoPooledPrefab<WeaponSkinCustomVFXInstance>(vfx.transform, 5, itemDefinition.CollideWithEnvironmentVFXAssetName);
                m_AllVFX.Add(itemDefinition, itemDefinition.CollideWithEnvironmentVFXAssetName);
            }
        }

        public static bool SkinHasCustomVFX(WeaponSkinItemDefinitionV2 itemDefinition)
        {
            return itemDefinition != null && m_AllVFX.ContainsKey(itemDefinition);
        }

        public static void SpawnVFX(Vector3 position, Vector3 eulerAngles, WeaponSkinItemDefinitionV2 itemDefinition)
        {
            if (!SkinHasCustomVFX(itemDefinition))
            {
                return;
            }

            Debug.Log((itemDefinition as IWeaponSkinItemDefinition).GetItemName() + " custom VFX!!");
            PooledPrefabController.SpawnObject<WeaponSkinCustomVFXInstance>(itemDefinition.CollideWithEnvironmentVFXAssetName, position, eulerAngles);
        }

        public static void RemoveAllVFX()
        {
            m_AllVFX.Clear();
        }
    }
}
