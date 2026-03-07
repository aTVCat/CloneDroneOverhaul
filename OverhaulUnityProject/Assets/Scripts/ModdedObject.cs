using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public class ModdedObject : MonoBehaviour
{
    public string ID;

    public List<Object> objects;
}

# if UNITY_EDITOR
[CustomEditor(typeof(ModdedObject))]
public class SceneImporterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Remove missing entries"))
        {
            ModdedObject moddedObject = (ModdedObject)target;
            for (int i = moddedObject.objects.Count - 1; i >= 0; i--)
            {
                if (!moddedObject.objects[i]) moddedObject.objects.RemoveAt(i);
            }

            EditorUtility.SetDirty(moddedObject.gameObject);
        }
    }
}
#endif