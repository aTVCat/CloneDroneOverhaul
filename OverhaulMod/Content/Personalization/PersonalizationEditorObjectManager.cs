using OverhaulMod.Utils;
using PicaVoxel;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectManager : Singleton<PersonalizationEditorObjectManager>
    {
        private List<PersonalizationEditorObjectSpawnInfo> m_objectInfos;

        private Material m_volumeMaterial;

        private List<PersonalizationEditorObjectBehaviour> m_instantiatedObjects;

        public override void Awake()
        {
            base.Awake();

            m_instantiatedObjects = new List<PersonalizationEditorObjectBehaviour>();
            m_objectInfos = new List<PersonalizationEditorObjectSpawnInfo>();
            addObjectInfo("Empty object", "Empty", instantiateEmpty);
            addObjectInfo("Volume", "Volume", instantiateVolume);
        }

        private void addObjectInfo(string name, string path, Func<Transform, GameObject> func)
        {
            PersonalizationEditorObjectSpawnInfo personalizationEditorObjectInfo = new PersonalizationEditorObjectSpawnInfo()
            {
                Name = name,
                Path = path,
                InstantiateFunction = func
            };
            m_objectInfos.Add(personalizationEditorObjectInfo);
        }

        public List<PersonalizationEditorObjectSpawnInfo> GetObjectInfos()
        {
            return m_objectInfos;
        }

        public void AddInstantiatedObject(PersonalizationEditorObjectBehaviour behaviour)
        {
            if (!m_instantiatedObjects.Contains(behaviour))
                m_instantiatedObjects.Add(behaviour);
        }

        public void RemoveInstantiatedObject(PersonalizationEditorObjectBehaviour behaviour)
        {
            _ = m_instantiatedObjects.Remove(behaviour);
        }

        private Material getVolumeMaterial()
        {
            Material material = m_volumeMaterial;
            if (material)
                return material;

            foreach (Material m in Resources.FindObjectsOfTypeAll<Material>())
                if (m && m.shader && m.name == "PicaVoxel PBR OneMinus Alpha Emissive" && m.shader.name == "PicaVoxel/PicaVoxel PBR OneMinus Alpha Emissive")
                {
                    material = m;
                    break;
                }

            m_volumeMaterial = material;
            return material;
        }

        private GameObject instantiateVolume(Transform parent)
        {
            SeveredVolumeGenerator severedVolumeGenerator = SeveredVolumeGenerator.Instance;
            MechBodyPart prefab = severedVolumeGenerator.EmptyBodyPartPrefab;
            prefab.enabled = false;
            MechBodyPart bodyPart = Instantiate(prefab, parent);
            prefab.enabled = true;
            GameObject gameObject = bodyPart.gameObject;
            _ = gameObject.AddComponent<PersonalizationEditorObjectVolume>();
            Volume volume = gameObject.GetComponent<Volume>();
            volume.Material = getVolumeMaterial();
            Destroy(bodyPart);
            return gameObject;
        }

        private GameObject instantiateEmpty(Transform parent)
        {
            GameObject obj = new GameObject();
            obj.transform.SetParent(parent);
            return obj;
        }

        public PersonalizationEditorObjectSpawnInfo GetObjectInfo(string path)
        {
            foreach (PersonalizationEditorObjectSpawnInfo info in m_objectInfos)
                if (info.Path == path)
                    return info;

            return null;
        }

        public PersonalizationEditorObjectBehaviour PlaceObject(string path, Transform parent = null, bool assignEditableValues = true)
        {
            PersonalizationEditorObjectSpawnInfo objectInfo = GetObjectInfo(path);
            if (objectInfo == null)
                return null;

            GameObject gameObject = objectInfo.Instantiate(parent);
            gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
            PersonalizationEditorObjectBehaviour personalizationEditorObject = gameObject.AddComponent<PersonalizationEditorObjectBehaviour>();
            personalizationEditorObject.Path = objectInfo.Path;

            if (assignEditableValues)
            {
                personalizationEditorObject.Name = objectInfo.Name;
                personalizationEditorObject.PropertyValues = new Dictionary<string, object>();
                if (!objectInfo.Properties.IsNullOrEmpty())
                    foreach (PersonalizationEditorObjectPropertyInfo p in objectInfo.Properties)
                        personalizationEditorObject.PropertyValues.Add(p.PropertyName, default);
            }

            return personalizationEditorObject;
        }
    }
}
