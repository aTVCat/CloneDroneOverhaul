using OverhaulMod.Utils;
using PicaVoxel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectManager : Singleton<PersonalizationEditorObjectManager>
    {
        public const string OBJECT_SELECTION_CHANGED_EVENT = "PersonalizationEditorObjectSelectionChanged";

        private List<PersonalizationEditorObjectSpawnInfo> _objectInfos;

        private Material _volumeMaterial;

        private List<PersonalizationEditorObjectBehaviour> _instantiatedObjects;

        private int _nextUniqueIndex;

        public override void Awake()
        {
            base.Awake();

            _instantiatedObjects = new List<PersonalizationEditorObjectBehaviour>();
            _objectInfos = new List<PersonalizationEditorObjectSpawnInfo>();
            addObjectInfo("Empty object", "Empty", instantiateEmpty);
            addObjectInfo("Model Renderer (.vox)", "Volume", instantiateVolume);
            addObjectInfo("Model Renderer (.cvm)", "CvmModel", instantiateCvmModel);
            addObjectInfo("Fire particles (Sword)", "FireParticles_Sword", instantiateSwordFireParticles);
            addObjectInfo("Fire particles (Hammer)", "FireParticles_Hammer", instantiateHammerFireParticles);
            addObjectInfo("Fire particles (Spear)", "FireParticles_Spear", instantiateSpearFireParticles);
            addObjectInfo("Arrow spawn point", "ArrowSpawnPoint", instantiateArrowSpawnPoint);
        }

        private void addObjectInfo(string name, string path, Func<Transform, GameObject> func)
        {
            PersonalizationEditorObjectSpawnInfo personalizationEditorObjectInfo = new PersonalizationEditorObjectSpawnInfo()
            {
                Name = name,
                Path = path,
                InstantiateFunction = func
            };
            _objectInfos.Add(personalizationEditorObjectInfo);
        }

        public List<PersonalizationEditorObjectSpawnInfo> GetObjectInfos()
        {
            return _objectInfos;
        }

        public int GetMaxUniqueIndex()
        {
            int result = 0;
            foreach (var instantiatedObject in _instantiatedObjects)
                result = Mathf.Max(result, instantiatedObject.UniqueIndex);

            return result;
        }

        public void AddInstantiatedObject(PersonalizationEditorObjectBehaviour behaviour)
        {
            if (!_instantiatedObjects.Contains(behaviour)) _instantiatedObjects.Add(behaviour);
        }

        public void RemoveInstantiatedObject(PersonalizationEditorObjectBehaviour behaviour)
        {
            _ = _instantiatedObjects.Remove(behaviour);
        }

        public PersonalizationEditorObjectBehaviour GetInstantiatedObject(int uniqueIndex)
        {
            List<PersonalizationEditorObjectBehaviour> list = _instantiatedObjects;
            if (list.IsNullOrEmpty())
                return null;

            int i = 0;
            do
            {
                PersonalizationEditorObjectBehaviour obj = list[i];
                if (obj && obj.UniqueIndex == uniqueIndex)
                    return obj;

                i++;
            } while (i < list.Count);

            return null;
        }

        public void SetCurrentRootNextUniqueIndex()
        {
            SetCurrentRootNextUniqueIndex(GetMaxUniqueIndex());
        }

        public void SetCurrentRootNextUniqueIndex(int value)
        {
            _nextUniqueIndex = value;
        }

        public int GetNextUniqueIndex()
        {
            _nextUniqueIndex++;
            return _nextUniqueIndex;
        }

        public int GetCurrentUniqueIndex()
        {
            return _nextUniqueIndex;
        }

        private Material getVolumeMaterial()
        {
            Material material = _volumeMaterial;
            if (material)
                return material;

            foreach (Material m in Resources.FindObjectsOfTypeAll<Material>())
                if (m && m.shader && m.name == "PicaVoxel PBR OneMinus Alpha Emissive" && m.shader.name == "PicaVoxel/PicaVoxel PBR OneMinus Alpha Emissive")
                {
                    material = m;
                    break;
                }

            _volumeMaterial = material;
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
            Transform t = gameObject.transform;
            t.localPosition = Vector3.zero;
            t.localEulerAngles = Vector3.zero;
            t.localScale = Vector3.one;
            _ = gameObject.AddComponent<PersonalizationEditorObjectVolume>();
            _ = gameObject.AddComponent<PersonalizationEditorObjectVisibilityController>();
            Volume volume = gameObject.GetComponent<Volume>();
            volume.Material = getVolumeMaterial();
            Destroy(bodyPart);
            return gameObject;
        }

        private GameObject instantiateCvmModel(Transform parent)
        {
            GameObject obj = new GameObject();
            Transform t = obj.transform;
            t.SetParent(parent);
            t.localPosition = Vector3.zero;
            t.localEulerAngles = Vector3.zero;
            t.localScale = Vector3.one;
            _ = obj.AddComponent<PersonalizationEditorObjectCVMModel>();
            _ = obj.AddComponent<PersonalizationEditorObjectVisibilityController>();
            return obj;
        }

        private GameObject instantiateSwordFireParticles(Transform parent)
        {
            Transform fireParticles = Instantiate(TransformUtils.FindChildRecursive(WeaponManager.Instance.FireSwordModelPrefab, "SwordFireVFX"), parent);
            Transform t = fireParticles.transform;
            t.localPosition = Vector3.zero;
            t.localEulerAngles = Vector3.zero;
            t.localScale = Vector3.one * 0.01f;
            _ = fireParticles.gameObject.AddComponent<PersonalizationEditorObjectVisibilityController>();
            _ = fireParticles.gameObject.AddComponent<PersonalizationEditorObjectFireParticles>();
            return fireParticles.gameObject;
        }

        private GameObject instantiateSpearFireParticles(Transform parent)
        {
            Transform fireParticles = Instantiate(TransformUtils.FindChildRecursive(WeaponManager.Instance.FireSpearModelPrefab, "SwordFireVFX (1)"), parent);
            Transform t = fireParticles.transform;
            t.localPosition = Vector3.zero;
            t.localEulerAngles = Vector3.zero;
            t.localScale = Vector3.one * 0.01f;
            _ = fireParticles.gameObject.AddComponent<PersonalizationEditorObjectVisibilityController>();
            _ = fireParticles.gameObject.AddComponent<PersonalizationEditorObjectFireParticles>();
            return fireParticles.gameObject;
        }

        private GameObject instantiateHammerFireParticles(Transform parent)
        {
            Transform fireParticles = Instantiate(TransformUtils.FindChildRecursive(WeaponManager.Instance.FireHammerModelPrefab, "FireVFX (1)"), parent);
            Transform t = fireParticles.transform;
            t.localPosition = Vector3.zero;
            t.localEulerAngles = Vector3.zero;
            t.localScale = Vector3.one * 0.01f;
            _ = fireParticles.gameObject.AddComponent<PersonalizationEditorObjectVisibilityController>();
            _ = fireParticles.gameObject.AddComponent<PersonalizationEditorObjectFireParticles>();
            return fireParticles.gameObject;
        }

        private GameObject instantiateEmpty(Transform parent)
        {
            GameObject obj = new GameObject();
            Transform t = obj.transform;
            t.SetParent(parent);
            t.localPosition = Vector3.zero;
            t.localEulerAngles = Vector3.zero;
            t.localScale = Vector3.one;
            return obj;
        }

        private GameObject instantiateArrowSpawnPoint(Transform parent)
        {
            GameObject obj = new GameObject("Arrow Spawn Point");
            Transform t = obj.transform;
            t.SetParent(parent);
            t.localPosition = Vector3.zero;
            t.localEulerAngles = Vector3.zero;
            t.localScale = Vector3.one;
            _ = obj.gameObject.AddComponent<PersonalizationEditorObjectArrowSpawnPoint>();
            return obj;
        }

        public PersonalizationEditorObjectSpawnInfo GetObjectInfo(string path)
        {
            foreach (PersonalizationEditorObjectSpawnInfo info in _objectInfos)
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
            }

            return personalizationEditorObject;
        }

        public void DeleteObject(PersonalizationEditorObjectBehaviour objectBehaviour)
        {
            StartCoroutine(deleteObjectCoroutine(objectBehaviour.gameObject));
        }

        private IEnumerator deleteObjectCoroutine(GameObject gameObject)
        {
            Destroy(gameObject);
            yield return new WaitForEndOfFrame();
            yield return null;
            PersonalizationEditorManager.Instance.SerializeRoot();
            yield break;
        }
    }
}
