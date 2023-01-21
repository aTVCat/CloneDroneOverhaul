using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    public static class LevelEditorObjectsController
    {
        public const string ObjectPathPrefix = "Prefabs/LevelObjects/OverhaulMod";
        public const string PreviewPathPrefix = "Previews/LevelObjects/OverhaulMod";

        private static Dictionary<string, UnityEngine.Object> _objects = new Dictionary<string, UnityEngine.Object>();

        private static bool _hasAddedAssets;

        internal static void Initialize()
        {
            if (!_hasAddedAssets)
            {
                GameObject obj1 = AssetController.GetAsset("TextBlock", Enumerators.EModAssetBundlePart.Objects);
                obj1.AddComponent<LevelEditorTextBlock>();
                AddObject(obj1.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", Enumerators.EModAssetBundlePart.Objects), "TextBlock");

                GameObject obj2 = AssetController.GetAsset("ImageBlock", Enumerators.EModAssetBundlePart.Objects);
                obj2.AddComponent<LevelEditorComponentDescription>().Description = "For internal use only!";
                obj2.AddComponent<LevelEditorImageBlock>();
                AddObject(obj2.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", Enumerators.EModAssetBundlePart.Objects), "ImageBlock");

                GameObject obj3 = AssetController.GetAsset("GamemodeSubstateChanger", Enumerators.EModAssetBundlePart.Objects);
                obj3.AddComponent<LevelEditorGamemodeSubstateChanger>();
                AddObject(obj3.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", Enumerators.EModAssetBundlePart.Objects), "G_SChanger");
            }

            OverhaulEventManager.AddEventListener(GlobalEvents.LevelEditorStarted, UpdateLevelEditor, true);

            _hasAddedAssets = true;
        }

        internal static void UpdateLevelEditor()
        {
            List<LevelObjectEntry> levelObjects = LevelObjectsLibraryManager.Instance.GetLevelObjectsInLibrary();
            List<LevelObjectEntry> visibleLevelObjects = LevelObjectsLibraryManager.Instance.GetVisibleLevelEditorObjects();

            foreach (string @string in _objects.Keys)
            {
                if (@string.Contains(ObjectPathPrefix))
                {
                    LevelObjectEntry entry = new LevelObjectEntry();
                    string[] subStrings = @string.Split("/".ToArray());
                    entry.DisplayName = subStrings[subStrings.Length - 1];
                    entry.PathUnderResources = @string;
                    entry.PreviewPathUnderResources = @string.Replace(ObjectPathPrefix, PreviewPathPrefix);

                    levelObjects.Add(entry);
                    visibleLevelObjects.Add(entry);
                }
            }

            GameUIRoot.Instance.LevelEditorUI.LibraryUI.Populate();
        }

        /// <summary>
        /// Add object to level editor
        /// </summary>
        /// <param name="theObject"></param>
        /// <param name="thePreview"></param>
        /// <param name="path"></param>
        /// <param name="displayName"></param>
        public static void AddObject(in Transform theObject, in Texture2D thePreview, in string displayName)
        {
            string objectKey = null;
            string previewKey = null;

            objectKey = ObjectPathPrefix + "/" + displayName;
            previewKey = PreviewPathPrefix + "/" + displayName;

            if (!_objects.ContainsKey(objectKey))
            {
                _objects.Add(objectKey, theObject);
            }

            if (!_objects.ContainsKey(previewKey))
            {
                _objects.Add(previewKey, thePreview);
            }
        }

        /// <summary>
        /// Returns <see cref="Transform"/> or <see cref="Texture2D"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static UnityEngine.Object GetObject(in string path)
        {
            if (_objects.ContainsKey(path))
            {
                return _objects[path];
            }
            return null;
        }
    }
}
