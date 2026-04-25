using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MoveAssetBundlesToFolder : MonoBehaviour
{
    public List<string> AssetBundles;

    public List<string> Hashes;

    public string Destination;

    public void MoveUpdatedAssetBundles()
    {
        if (Hashes == null) Hashes = new List<string>();

        for (int i = 0; i < AssetBundles.Count; i++)
        {
            string assetBundle = AssetBundles[i];
            string file = Path.Combine(Application.streamingAssetsPath, assetBundle);
            if (hasBeenUpdated(i, assetBundle))
            {
                string dest = Path.Combine(Destination, assetBundle);
                if (File.Exists(dest)) File.Delete(dest);
                File.Copy(file, dest);
                Debug.Log($"Updated <color=#00FF00>{assetBundle}</color>!");
            }
        }
    }

    private bool hasBeenUpdated(int hashIndex, string bundleName)
    {
        bool result = false;

        string manifestString = string.Empty;
        int lineIndex = 0;
        foreach (string line in File.ReadLines(Path.Combine(Application.streamingAssetsPath, $"{bundleName}.manifest")))
        {
            if (lineIndex == 6)
            {
                manifestString = line.Trim().Substring(6);
            }
            lineIndex++;
        }
;
        if (Hashes[hashIndex] != manifestString)
        {
            Hashes[hashIndex] = manifestString;
#if UNITY_EDITOR
            EditorUtility.SetDirty(gameObject);
#endif
            result = true;
        }

        return result;
    }
}

# if UNITY_EDITOR
[CustomEditor(typeof(MoveAssetBundlesToFolder))]
public class MoveAssetBundlesToFolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Fill list"))
        {
            MoveAssetBundlesToFolder component = (MoveAssetBundlesToFolder)target;
            if (component.AssetBundles == null) component.AssetBundles = new List<string>();

            string[] names = AssetDatabase.GetAllAssetBundleNames();
            foreach (string name in names)
                if (!component.AssetBundles.Contains(name))
                    component.AssetBundles.Add(name);

            EditorUtility.SetDirty(component.gameObject);
        }
    }
}
#endif
