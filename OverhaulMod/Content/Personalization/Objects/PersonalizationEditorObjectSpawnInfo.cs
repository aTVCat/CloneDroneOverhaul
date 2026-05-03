using System;
using UnityEngine;

namespace OverhaulMod.Content.Personalization.Objects
{
    public class PersonalizationEditorObjectSpawnInfo
    {
        public string DisplayName, Path;

        public Func<Transform, GameObject> InstantiateFunction;

        public GameObject Instantiate(Transform parent) => InstantiateFunction(parent);
    }
}
