using UnityEngine;

namespace CDOverhaul.HUD
{
    public class PrefabContainer
    {
        public ModdedObject Prefab;
        public Transform Container;

        public PrefabContainer(ModdedObject moddedObject, int indexOfPrefab, int indexOfContainer)
        {
            if (moddedObject == null)
                throw new System.ArgumentNullException();

            setUp(moddedObject.GetObject<ModdedObject>(indexOfPrefab), moddedObject.GetObject<Transform>(indexOfContainer));
        }

        public PrefabContainer(ModdedObject prefab, Transform container)
        {
            setUp(prefab, container);
        }

        private void setUp(ModdedObject prefab, Transform container)
        {
            if (!prefab && !container)
                throw new System.ArgumentNullException("prefab and container are NULL");
            if (!prefab)
                throw new System.ArgumentNullException("prefab is NULL");
            if (!container)
                throw new System.ArgumentNullException("container is NULL");

            Prefab = prefab;
            Prefab.gameObject.SetActive(false);
            Prefab.transform.SetParent(container.parent);
            Container = container;
        }

        public ModdedObject InstantiateEntry(bool startActive = true)
        {
            if (!Prefab || !Container)
                return null;

            ModdedObject result = UnityEngine.Object.Instantiate(Prefab, Container);
            result.gameObject.SetActive(startActive);
            result.gameObject.name = result.gameObject.name.Replace("(Clone)", string.Empty);
            return result;
        }

        public void Clear()
        {
            if (!Container)
                return;

            TransformUtils.DestroyAllChildren(Container);
        }
    }
}
