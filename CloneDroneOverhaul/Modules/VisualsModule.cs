using System;
using System.Collections.Generic;
using UnityEngine;
using ModLibrary;
using UnityEngine.Rendering;
using CloneDroneOverhaul.Utilities;
using CloneDroneOverhaul.PooledPrefabs;

namespace CloneDroneOverhaul.Modules
{
    public class VisualsModule : ModuleBase
    {
        private bool isInitialized;

        private ReflectionProbe probe;
        ParticleSystem worldDustMS1;
        ParticleSystem worldDustMS0;
        ParticleSystem worldDustNormal;

        SimplePooledPrefab swordBlockPooled;
        SimplePooledPrefab swordFireBlockPooled;
        SimplePooledPrefab swordBlockMSPooled;
        SimplePooledPrefab msBodyPartDamagedVFX;

        SimplePooledPrefab bodyPartDamagedVFX;
        SimplePooledPrefab bodyPartDamagedWithFireVFX;
        SimplePooledPrefab bodyPartBurning;

        private bool isWaitingNextFrame;

        public override bool ShouldWork()
        {
            return isInitialized; // SETTINGS!!
        }
        public override void OnActivated()
        {
            GameObject gameObject = new GameObject("CDO_Visuals");
            this.probe = gameObject.AddComponent<ReflectionProbe>();
            this.probe.size = new Vector3(4096f, 4096f, 4096f);
            this.probe.resolution = 64;
            this.probe.shadowDistance = 1024f;
            this.probe.intensity = 0.75f;
            this.probe.nearClipPlane = 0.01f;
            this.probe.nearClipPlane = 0.3f;
            this.probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
            this.probe.mode = ReflectionProbeMode.Realtime;
            this.probe.timeSlicingMode = ReflectionProbeTimeSlicingMode.NoTimeSlicing;
            Singleton<GlobalEventManager>.Instance.AddEventListener(GlobalEvents.SectionVisibilityChanged, this.renderReflections);

            worldDustMS1 = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustM1")).GetComponent<ParticleSystem>();
            worldDustMS0 = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustM0")).GetComponent<ParticleSystem>();
            worldDustNormal = UnityEngine.Object.Instantiate(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "WorldDustNormal")).GetComponent<ParticleSystem>();

            swordBlockPooled = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_Block").transform, 10, "VFX_SwordBlock", 0.15f, SimplePooledPrefabInstance.ParticleSystemTag);
            swordFireBlockPooled = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_FireBlock").transform, 50, "VFX_SwordFireBlock", 0.15f, SimplePooledPrefabInstance.ParticleSystemTag);
            swordBlockMSPooled = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_BlockMS").transform, 10, "VFX_SwordBlockMS", 0.15f, SimplePooledPrefabInstance.ParticleSystemTag);
            msBodyPartDamagedVFX = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_CutMS").transform, 10, "VFX_MSCut", 0.15f, SimplePooledPrefabInstance.ParticleSystemTag);
            bodyPartDamagedVFX = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_Cut_Normal").transform, 15, "VFX_Cut", 0.15f, SimplePooledPrefabInstance.ParticleSystemTag);
            bodyPartDamagedWithFireVFX = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_Cut_Fire").transform, 15, "VFX_FireCut", 0.15f, SimplePooledPrefabInstance.ParticleSystemTag);
            bodyPartBurning = new SimplePooledPrefab(AssetLoader.GetObjectFromFile("cdo_rw_stuff", "VFX_FireBurn").transform, 15, "VFX_Burning", 0.25f, SimplePooledPrefabInstance.ParticleSystemTag);

            RefreshDustMaterials();

            isInitialized = true;
        }

        private void renderReflections()
        {
            isWaitingNextFrame = true;
        }

        public override void OnNewFrame()
        {
            if (isWaitingNextFrame)
            {
                isWaitingNextFrame = false;
                this.probe.RenderProbe();
            }
        }
        public override void OnManagedUpdate()
        {
            if (Camera.main != null)
            {
                this.probe.gameObject.transform.position = Camera.main.transform.position;
                this.worldDustMS1.gameObject.transform.position = Camera.main.transform.position;
                this.worldDustMS0.gameObject.transform.position = Camera.main.transform.position;
                this.worldDustNormal.gameObject.transform.position = Camera.main.transform.position;
            }
            else
            {
                this.probe.gameObject.transform.position = Vector3.zero;
                this.worldDustMS1.gameObject.transform.position = Vector3.zero;
                this.worldDustMS0.gameObject.transform.position = Vector3.zero;
                this.worldDustNormal.gameObject.transform.position = Vector3.zero;
            }
        }
        public override void OnSecond(float time)
        {
            if (time == 0.5f)
            {
                RefreshDustMaterials();
            }
        }

        private void RefreshDustMaterials()
        {
            RobotShortInformation info = PlayerUtilities.GetPlayerRobotInfo();
            bool useMindspace = false;
            if (!info.IsNull)
            {
                useMindspace = info.IsFPMMindspace;
            }

            worldDustNormal.Stop();
            worldDustMS0.Stop();
            worldDustMS1.Stop();

            if (useMindspace)
            {
                worldDustMS0.Play();
                worldDustMS1.Play();
                worldDustNormal.Stop();
            }
            else
            {
                worldDustMS0.Stop();
                worldDustMS1.Stop();
                worldDustNormal.Play();
            }
        }

        public void EmitSwordBlockVFX(Vector3 pos, bool isFire = false)
        {
            RobotShortInformation info = PlayerUtilities.GetPlayerRobotInfo();
            bool useMindspace = false;
            if (!info.IsNull)
            {
                useMindspace = info.IsFPMMindspace;
            }

            if (useMindspace)
            {
                swordBlockMSPooled.SpawnObject(pos, Vector3.zero, Color.clear);
            }
            else
            {
                if (isFire)
                {
                    swordFireBlockPooled.SpawnObject(pos, Vector3.zero, Color.clear);
                    return;
                }
                swordBlockPooled.SpawnObject(pos, Vector3.zero, Color.clear);
            }
        }

        public void EmitMSBodyPartDamage(Vector3 pos)
        {
            msBodyPartDamagedVFX.SpawnObject(pos, Vector3.zero, Color.clear);
        }

        public void EmitBodyPartCutVFX(Vector3 pos, bool isFire)
        {
            if (isFire)
            {
                bodyPartDamagedWithFireVFX.SpawnObject(pos, Vector3.zero, Color.clear);
            }
            else
            {
                bodyPartDamagedVFX.SpawnObject(pos, Vector3.zero, Color.clear);
            }
        }

        public void EmitBurningVFX(Vector3 pos)
        {
            bodyPartBurning.SpawnObject(pos, Vector3.zero, Color.clear);
        }
    }
}
