using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CloneDroneOverhaul.PooledPrefabs
{
    public class SimplePooledPrefab
    {
        // Token: 0x060000F6 RID: 246 RVA: 0x00008980 File Offset: 0x00006B80
        public SimplePooledPrefab(Transform prefab, int maxCount, string name, float timeToDestroy, string tag)
        {
            this.Prefab = prefab;
            this.MaxCount = maxCount;
            this.Name = name;
            this.timeToHide = timeToDestroy;
            this.tag = tag;
            this.InitializeContainer();
        }

        // Token: 0x060000F7 RID: 247 RVA: 0x000089D8 File Offset: 0x00006BD8
        private void InitializeContainer()
        {
            Transform transform = new GameObject(this.Name).transform;
            for (int i = 0; i < this.MaxCount; i++)
            {
                Transform transform2 = UnityEngine.Object.Instantiate<Transform>(this.Prefab, transform);
                transform2.gameObject.name = this.Name + "_" + i.ToString();
                transform2.gameObject.SetActive(false);
                this.AvailableObjects.Add(transform2);
            }
        }

        // Token: 0x060000F8 RID: 248 RVA: 0x00008A58 File Offset: 0x00006C58
        public Transform SpawnObject(Vector3 position, Vector3 rotation, Color col, Transform transformToFollow = null, bool usedForVoxels = false)
        {
            Transform nextObject = this.GetNextObject();
            bool flag = nextObject != null;
            Transform result;
            if (flag)
            {
                nextObject.gameObject.SetActive(true);
                nextObject.position = position;
                nextObject.eulerAngles = rotation;
                nextObject.gameObject.AddComponent<SimplePooledPrefabInstance>().Initialize(this, transformToFollow, position, usedForVoxels, col);
                this.ActiveObjects.Add(nextObject);
                result = nextObject;
            }
            else
            {
                result = null;
            }
            return result;
        }

        // Token: 0x060000F9 RID: 249 RVA: 0x00008AC4 File Offset: 0x00006CC4
        private Transform GetNextObject()
        {
            foreach (Transform transform in this.AvailableObjects)
            {
                bool flag = !this.ActiveObjects.Contains(transform);
                if (flag)
                {
                    return transform;
                }
            }
            return null;
        }

        // Token: 0x060000FA RID: 250 RVA: 0x00008B34 File Offset: 0x00006D34
        internal void ReturnToPool(SimplePooledPrefabInstance instance, bool dontDisable)
        {
            bool flag = this.ActiveObjects.Contains(instance.transform);
            if (flag)
            {
                bool flag2 = !dontDisable;
                if (flag2)
                {
                    instance.gameObject.SetActive(false);
                }
                this.ActiveObjects.Remove(instance.transform);
                UnityEngine.Object.Destroy(instance);
            }
        }

        // Token: 0x060000FB RID: 251 RVA: 0x00008B8C File Offset: 0x00006D8C
        public float GetLifeTime()
        {
            return this.timeToHide;
        }

        // Token: 0x060000FC RID: 252 RVA: 0x00008BA4 File Offset: 0x00006DA4
        public string GetTag()
        {
            return this.tag;
        }

        // Token: 0x04000092 RID: 146
        private List<Transform> AvailableObjects = new List<Transform>();

        // Token: 0x04000093 RID: 147
        private List<Transform> ActiveObjects = new List<Transform>();

        // Token: 0x04000094 RID: 148
        private int MaxCount;

        // Token: 0x04000095 RID: 149
        private float timeToHide;

        // Token: 0x04000096 RID: 150
        private string Name;

        // Token: 0x04000097 RID: 151
        private string tag;

        // Token: 0x04000098 RID: 152
        private Transform Prefab;
    }

    public class SimplePooledPrefabInstance : ManagedBehaviour
    {
        // Token: 0x060000FD RID: 253 RVA: 0x00008BBC File Offset: 0x00006DBC
        public void Initialize(SimplePooledPrefab og, Transform transformToFollow, Vector3 initPos, bool voxelFix, Color lightColor)
        {
            bool flag = og.GetLifeTime() > 0f;
            if (flag)
            {
                this.TimeToHide = Time.time + og.GetLifeTime();
            }
            this.orig = og;
            this.Follow = transformToFollow;
            bool flag2 = this.Follow != null;
            if (flag2)
            {
                this.UsesFollow = true;
                this.InitPosDifference = transformToFollow.position - initPos;
                if (voxelFix)
                {
                    this.InitPosDifference *= 0.5f;
                }
            }
            bool flag3 = this.orig.GetTag() == SimplePooledPrefabInstance.ParticleSystemTag;
            if (flag3)
            {
                base.GetComponent<ParticleSystem>().Play();
            }
        }

        // Token: 0x060000FE RID: 254 RVA: 0x00008CE8 File Offset: 0x00006EE8
        public override void UpdateMe()
        {
            bool flag = this.TimeToHide != -1f && Time.time >= this.TimeToHide && !this.isEnded;
            if (flag)
            {
                this.isEnded = true;
                bool flag2 = this.orig.GetTag() == SimplePooledPrefabInstance.ParticleSystemTag;
                if (flag2)
                {
                    base.GetComponent<ParticleSystem>().Stop();
                    this.ReturnToPool(true);
                    return;
                }
                this.ReturnToPool(false);
            }
            bool flag3 = this.InitPosDifference != Vector3.zero && this.Follow != null;
            if (flag3)
            {
                base.gameObject.transform.position = this.Follow.transform.position + this.InitPosDifference;
            }
            bool flag4 = this.UsesFollow && this.Follow == null;
            if (flag4)
            {
                this.ReturnToPool(true);
            }
        }

        // Token: 0x060000FF RID: 255 RVA: 0x00008DF6 File Offset: 0x00006FF6
        public void ReturnToPool(bool dontDisableGameObject)
        {
            this.orig.ReturnToPool(this, dontDisableGameObject);
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
    }
}
