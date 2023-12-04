using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod
{
    public class ModLevelEditorManager : Singleton<ModLevelEditorManager>
    {
        private static readonly Dictionary<string, Texture2D> s_Textures = new Dictionary<string, Texture2D>();
        private static readonly Dictionary<string, Transform> s_Transforms = new Dictionary<string, Transform>();

        public void AddObject(string name, string folder, ref List<LevelObjectEntry> list, Transform transform, Texture2D texture = null)
        {
            if (list == null)
                throw new ArgumentNullException("LevelObjectEntry list");

            string path = "Prefabs/LevelObjects/" + folder + "/" + name;
            string preview = "Images/modded/" + folder + "/" + name;

            if (!s_Textures.ContainsKey(preview))
            {
                Texture2D textureToAdd = texture;
                if (!textureToAdd)
                    textureToAdd = new Texture2D(8, 8);

                s_Textures.Add(preview, textureToAdd);
            }

            if (!s_Transforms.ContainsKey(path))
            {
                s_Transforms.Add(path, transform);
            }

            foreach (LevelObjectEntry entry in list)
                if (entry.PathUnderResources == path)
                    return;

            list.Add(new LevelObjectEntry()
            {
                PathUnderResources = path,
                PreviewPathUnderResources = preview,
                DisplayName = name
            });
        }

        public bool HasTexture(string path)
        {
            return s_Textures.ContainsKey(path);
        }

        public Texture2D GetTexture(string path)
        {
            return s_Textures[path];
        }

        public bool HasTransform(string path)
        {
            return s_Transforms.ContainsKey(path);
        }

        public Transform GetTransform(string path)
        {
            return s_Transforms[path];
        }
    }
}
