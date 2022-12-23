using UnityEngine;
using System.Collections.Generic;
using CloneDroneOverhaul.V3Tests.Base;
using ModLibrary;

namespace CloneDroneOverhaul.V3Tests.Gameplay
{
    public static class OverhaulGraphicsController
    {
        #region VFX

        public const string ID_VFX_SWORDBLOCK = "VFX_SwordBlock";
        public const string ID_VFX_SWORDBLOCK_FIRE = "VFX_FireSwordBlock";

        public const string ID_VFX_BLOCK_MINDSPACE = "VFX_MindspaceBlock";
        public const string ID_VFX_HIT_MINDSPACE = "VFX_MindspaceHit";

        /// <summary>
        /// Initialize class
        /// </summary>
        public static void Initialize()
        {
            setUpPooledPrefabs();
        }

        /// <summary>
        /// Spawn pooled prefabs
        /// </summary>
        private static void setUpPooledPrefabs()
        {
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_Block").transform, 5, ID_VFX_SWORDBLOCK);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_LongLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_FireBlock").transform, 50, ID_VFX_SWORDBLOCK_FIRE);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_LongLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_BlockMS").transform, 5, ID_VFX_BLOCK_MINDSPACE);
            PooledPrefabController.TurnObjectIntoPooledPrefab<PooledPrefab_VFXEffect_ShortLifeTime>(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_MSHit").transform, 5, ID_VFX_HIT_MINDSPACE);
        }

        /// <summary>
        /// Spawn VFX effect in world
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public static void UseVFX(in string id, in Vector3 position, in Vector3 rotation)
        {
            PooledPrefabController.SpawnObject<PooledPrefabInstanceBase>(id, position, rotation);
        }

        #endregion

        #region VFXSpawns

        public static void EmitVFX_SwordBlock(in Vector3 position, in bool isFire)
        {
            bool isMindspace = GameStatisticsController.GameStatistics.PlayerRobotInformation.IsFPMMindspace;

            Vector3 rot = Vector3.zero;

            if (isMindspace)
            {
                UseVFX(ID_VFX_BLOCK_MINDSPACE, position, rot);
                UseVFX(ID_VFX_HIT_MINDSPACE, position, rot);
                return;
            }
            else
            {
                if (isFire)
                {
                    UseVFX(ID_VFX_SWORDBLOCK_FIRE, position, rot);
                }
                else
                {
                    UseVFX(ID_VFX_SWORDBLOCK, position, rot);
                }
            }
        }

        #endregion
    }
}