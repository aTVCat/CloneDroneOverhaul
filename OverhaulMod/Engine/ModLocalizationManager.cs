using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Engine
{
    public class ModLocalizationManager : Singleton<ModLocalizationManager>
    {
        public void PopulateTranslationDictionary(ref Dictionary<string, string> keyValuePairs, string langId)
        {
            Debug.Log($"Language changed: {langId}");
        }
    }
}
