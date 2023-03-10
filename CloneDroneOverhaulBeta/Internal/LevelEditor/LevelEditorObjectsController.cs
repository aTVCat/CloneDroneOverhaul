using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CDOverhaul.LevelEditor
{
    public static class LevelEditorObjectsController
    {
        public const string ObjectPathPrefix = "Prefabs/LevelObjects/OverhaulMod";
        public const string PreviewPathPrefix = "Previews/LevelObjects/OverhaulMod";

        private static readonly Dictionary<string, UnityEngine.Object> _objects = new Dictionary<string, UnityEngine.Object>();

        private static bool _hasAddedAssets;

        internal static void Initialize()
        {
            if (!_hasAddedAssets)
            {
                GameObject obj1 = AssetController.GetAsset("TextBlock", OverhaulAssetsPart.Objects);
                _ = obj1.AddComponent<LevelEditorTextBlock>();
                AddObject(obj1.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", OverhaulAssetsPart.Objects), "TextBlock");

                GameObject obj2 = AssetController.GetAsset("ImageBlock", OverhaulAssetsPart.Objects);
                obj2.AddComponent<LevelEditorComponentDescription>().Description = "For internal use only!";
                _ = obj2.AddComponent<LevelEditorImageBlock>();
                AddObject(obj2.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", OverhaulAssetsPart.Objects), "ImageBlock");

                GameObject obj3 = AssetController.GetAsset("GamemodeSubstateChanger", OverhaulAssetsPart.Objects);
                _ = obj3.AddComponent<LevelEditorGamemodeSubstateChanger>();
                AddObject(obj3.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", OverhaulAssetsPart.Objects), "G_SChanger");

                GameObject obj4 = AssetController.GetAsset("SkinEdit_SkinSpawnpoint", OverhaulAssetsPart.Objects);
                _ = obj4.AddComponent<LevelEditorSkinSpawnpoint>();
                AddObject(obj4.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", OverhaulAssetsPart.Objects), "SkinSpawnpoint");

                GameObject obj6 = AssetController.GetAsset("GamemodeSubstateChanger", OverhaulAssetsPart.Objects);
                _ = obj6.AddComponent<LevelEditorGiveAllWeapons>();
                AddObject(obj6.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", OverhaulAssetsPart.Objects), "GiveAllWeapons");

                GameObject obj5 = AssetController.GetAsset("EnemiesLeftPanelPoser", OverhaulAssetsPart.Objects);
                _ = obj5.AddComponent<LevelEditorArenaEnemiesCounterPoser>();
                AddObject(obj5.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", OverhaulAssetsPart.Objects), "AR_EL_Mover");
                _hasAddedAssets = true;

                GameObject obj7 = AssetController.GetAsset("GarbageBotSpawnPoint", OverhaulAssetsPart.Objects);
                _ = obj7.AddComponent<LevelEditorGarbageBotSpawnpoint>();
                AddObject(obj7.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", OverhaulAssetsPart.Objects), "GarbageBotSpawnpoint");

                GameObject obj8 = AssetController.GetAsset("GarbageDropPoint", OverhaulAssetsPart.Objects);
                _ = obj8.AddComponent<LevelEditorGarbageDropPoint>();
                AddObject(obj8.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", OverhaulAssetsPart.Objects), "GarbageDropPoint");
                
                GameObject obj9 = AssetController.GetAsset("GarbageCustomForcefield", OverhaulAssetsPart.Objects);
                _ = obj9.AddComponent<LevelEditorGarbageCustomForcefield>();
                AddObject(obj9.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", OverhaulAssetsPart.Objects), "GarbageCustomForcefield");

                GameObject obj10 = AssetController.GetAsset("PickableWeapon", OverhaulAssetsPart.Objects);
                _ = obj10.AddComponent<LevelEditorPickableWeapon>();
                _ = obj10.AddComponent<LevelEditorTriggerPlayAnimation>();
                AddObject(obj10.transform, AssetController.GetAsset<Texture2D>("HologrammIco16x16", OverhaulAssetsPart.Objects), "PickableWeapon");

                _hasAddedAssets = true;
            }

            _ = OverhaulEventManager.AddEventListener(GlobalEvents.LevelEditorStarted, UpdateLevelEditor, true);
        }

        internal static void UpdateLevelEditor()
        {
            //List<LevelObjectEntry> levelObjects = LevelObjectsLibraryManager.Instance.GetLevelObjectsInLibrary();
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

                    //levelObjects.Add(entry);
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
            string objectKey = ObjectPathPrefix + "/" + displayName;
            string previewKey = PreviewPathPrefix + "/" + displayName;
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
