using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CloneDroneOverhaul.LevelEditor
{
    public static class LevelEditorCustomObjectsManager
    {
        public const string PREFABPATHPREFIX = "Prefabs/LevelObjects/";
        public const string TEXTUREPATHPREFIX = "CloneDroneOverhaul/Textures/";

        static Dictionary<string, Transform> _moddedLevelObjectTransforms = new Dictionary<string, Transform>();
        static Dictionary<string, Texture2D> _moddedLevelObjectTextures = new Dictionary<string, Texture2D>();

        static List<Tuple<string, string>> _moddedLevelObjects = new List<Tuple<string, string>>();

        public static void AddObject(string folderName, string itemName, Transform transform, Texture2D image)
        {
            string fullPath = PREFABPATHPREFIX + folderName + "/" + itemName;
            string texturePath = TEXTUREPATHPREFIX + folderName + "/" + itemName;

            if (!_moddedLevelObjectTextures.ContainsKey(texturePath))
                _moddedLevelObjectTextures.Add(texturePath, image);

            if (!_moddedLevelObjectTransforms.ContainsKey(fullPath))
                _moddedLevelObjectTransforms.Add(fullPath, transform);

            Tuple<string, string> tuple = new Tuple<string, string>(fullPath, texturePath);

            if (!_moddedLevelObjects.Contains(tuple))
                _moddedLevelObjects.Add(tuple);
        }

        internal static UnityEngine.Object TryGetObject(string path)
        {
            UnityEngine.Object obj = GetGameObject(path);
            if (obj == null)
            {
                obj = GetPreviewTexture(path);
            }
            return obj;
        }

        internal static void OnLevelEditorStarted()
        {
            List<LevelObjectEntry> levelObjects = LevelObjectsLibraryManager.Instance.GetLevelObjectsInLibrary();
            List<LevelObjectEntry> visibleLevelObjects = LevelObjectsLibraryManager.Instance.GetVisibleLevelEditorObjects();

            foreach (Tuple<string, string> tuple in _moddedLevelObjects)
            {
                LevelObjectEntry entry = new LevelObjectEntry();
                string[] subStrings = tuple.Item1.Split("/".ToArray());
                entry.DisplayName = subStrings[subStrings.Length - 1];
                entry.PathUnderResources = tuple.Item1;
                entry.PreviewPathUnderResources = tuple.Item2;

                levelObjects.Add(entry);
                visibleLevelObjects.Add(entry);
            }

            GameUIRoot.Instance.LevelEditorUI.LibraryUI.Populate();
        }

        public static Texture2D GetPreviewTexture(string path)
        {
            if (HasTexture(path))
            {
                return _moddedLevelObjectTextures[path];
            }
            return null;
        }

        public static Transform GetGameObject(string path)
        {
            Transform transform = null;
            if (HasTransform(path))
            {
                transform = _moddedLevelObjectTransforms[path];
            }
            return transform;
        }

        public static bool HasTransform(string path)
        {
            return _moddedLevelObjectTransforms.ContainsKey(path);
        }

        public static bool HasTexture(string path)
        {
            return _moddedLevelObjectTextures.ContainsKey(path);
        }

    }
}
