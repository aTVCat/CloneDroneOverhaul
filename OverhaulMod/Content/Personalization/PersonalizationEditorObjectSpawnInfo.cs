using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectSpawnInfo
    {
        public string Name, Path;

        public Func<Transform, GameObject> InstantiateFunction;

        public GameObject Instantiate(Transform parent)
        {
            return InstantiateFunction(parent);
        }
    }
}
