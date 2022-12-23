using System.Collections.Generic;
using UnityEngine;

namespace CloneDroneOverhaul.PooledPrefabs
{
    public class SimplePooledPrefab
    {
        public SimplePooledPrefab(Transform prefab, int maxCount, string name, float timeToDestroy, string tag)
        {
            Prefab = prefab;
            MaxCount = maxCount;
            Name = name;
            timeToHide = timeToDestroy;
            this.tag = tag;
            InitializeContainer();
        }

        private void InitializeContainer()
        {
            Transform transform = new GameObject(Name).transform;
            for (int i = 0; i < MaxCount; i++)
            {
                Transform transform2 = UnityEngine.Object.Instantiate<Transform>(Prefab, transform);
                transform2.gameObject.name = Name + "_" + i.ToString();
                transform2.gameObject.SetActive(false);
                AvailableObjects.Add(transform2);
            }
        }

        public Transform SpawnObject(Vector3 position, Vector3 rotation, Color col, Transform transformToFollow = null, bool usedForVoxels = false)
        {
            Transform nextObject = GetNextObject();
            bool flag = nextObject != null;
            Transform result;
            if (flag)
            {
                nextObject.gameObject.SetActive(true);
                nextObject.position = position;
                nextObject.eulerAngles = rotation;
                nextObject.gameObject.AddComponent<SimplePooledPrefabInstance>().Initialize(this, transformToFollow, position, usedForVoxels, col);
                ActiveObjects.Add(nextObject);
                result = nextObject;
            }
            else
            {
                result = null;
            }
            return result;
        }

        private Transform GetNextObject()
        {
            foreach (Transform transform in AvailableObjects)
            {
                bool flag = !ActiveObjects.Contains(transform);
                if (flag)
                {
                    return transform;
                }
            }
            return null;
        }

        internal void ReturnToPool(SimplePooledPrefabInstance instance, bool dontDisable)
        {
            bool flag = ActiveObjects.Contains(instance.transform);
            if (flag)
            {
                bool flag2 = !dontDisable;
                if (flag2)
                {
                    instance.gameObject.SetActive(false);
                }
                ActiveObjects.Remove(instance.transform);
                UnityEngine.Object.Destroy(instance);
            }
        }

        public float GetLifeTime()
        {
            return timeToHide;
        }

        public string GetTag()
        {
            return tag;
        }

        private List<Transform> AvailableObjects = new List<Transform>();

        private List<Transform> ActiveObjects = new List<Transform>();

        private int MaxCount;

        private float timeToHide;

        private string Name;

        private string tag;

        public Transform Prefab;
    }

    public class SimplePooledPrefabInstance : ManagedBehaviour
    {
        // Token: 0x060000FD RID: 253 RVA: 0x00008BBC File Offset: 0x00006DBC
        public void Initialize(SimplePooledPrefab og, Transform transformToFollow, Vector3 initPos, bool voxelFix, Color lightColor)
        {
            bool flag = og.GetLifeTime() > 0f;
            if (flag)
            {
                TimeToHide = Time.time + og.GetLifeTime();
            }
            orig = og;
            Follow = transformToFollow;
            bool flag2 = Follow != null;
            if (flag2)
            {
                UsesFollow = true;
                InitPosDifference = transformToFollow.position - initPos;
                if (voxelFix)
                {
                    InitPosDifference *= 0.5f;
                }
            }
            bool flag3 = orig.GetTag() == SimplePooledPrefabInstance.ParticleSystemTag;
            if (flag3)
            {
                base.GetComponent<ParticleSystem>().Play();
            }
            bool flag5 = orig.GetTag() == SimplePooledPrefabInstance.LightTag;
            if (flag5)
            {
                bool flag6 = lightColor != Color.clear;
                if (flag6)
                {
                    base.GetComponent<Light>().color = lightColor;
                }
                base.GetComponent<Animator>().Play("LightStarted");
            }
            bool flag61 = orig.GetTag() == SimplePooledPrefabInstance.LongLiveLightTag;
            if (flag61)
            {
                bool flag7 = lightColor != Color.clear;
                if (flag7)
                {
                    base.GetComponent<Light>().color = lightColor;
                }
                base.GetComponent<Animator>().speed = 0.3f;
                base.GetComponent<Animator>().Play("LightStarted");
            }
        }

        // Token: 0x060000FE RID: 254 RVA: 0x00008CE8 File Offset: 0x00006EE8
        public override void UpdateMe()
        {
            bool flag = TimeToHide != -1f && Time.time >= TimeToHide && !isEnded;
            if (flag)
            {
                isEnded = true;
                bool flag2 = orig.GetTag() == SimplePooledPrefabInstance.ParticleSystemTag;
                if (flag2)
                {
                    base.GetComponent<ParticleSystem>().Stop();
                    ReturnToPool(true);
                    return;
                }
                ReturnToPool(false);
            }
            bool flag3 = InitPosDifference != Vector3.zero && Follow != null;
            if (flag3)
            {
                base.gameObject.transform.position = Follow.transform.position + InitPosDifference;
            }
            bool flag4 = UsesFollow && Follow == null;
            if (flag4)
            {
                ReturnToPool(true);
            }
        }

        // Token: 0x060000FF RID: 255 RVA: 0x00008DF6 File Offset: 0x00006FF6
        public void ReturnToPool(bool dontDisableGameObject)
        {
            orig.ReturnToPool(this, dontDisableGameObject);
        }

        // Token: 0x04000099 RID: 153
        private float TimeToHide = -1f;

        // Token: 0x0400009A RID: 154
        private bool isEnded;

        // Token: 0x0400009B RID: 155
        private SimplePooledPrefab orig;

        // Token: 0x0400009C RID: 156
        private bool UsesFollow;

        // Token: 0x0400009D RID: 157
        private Transform Follow;

        // Token: 0x0400009E RID: 158
        private Vector3 InitPosDifference = Vector3.zero;

        // Token: 0x0400009F RID: 159
        public static string ParticleSystemTag = "OnlyParticles";

        // Token: 0x040000A0 RID: 160
        public static string OnlyParticleEmitSystemTag = "OnlyEmit";

        // Token: 0x040000A1 RID: 161
        public static string LightTag = "EmitLight";

        public static string LongLiveLightTag = "EmitLongLiveLight";
    }
}
