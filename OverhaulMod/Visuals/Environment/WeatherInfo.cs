using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Visuals.Environment
{
    public class WeatherInfo
    {
        public string Name;

        public bool IsDefault;

        public float EmissionRate;

        public KeyValuePair<string, string> BundleAndAsset;

        public GameObject GetVFXPrefab()
        {
            if (IsDefault)
                return null;

            return ModResources.Load<GameObject>(BundleAndAsset.Key, BundleAndAsset.Value);
        }
    }
}
