using OverhaulMod.Utils;
using PicaVoxel;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectVolume : PersonalizationEditorObjectComponentBase
    {
        private bool m_hasAddedEventListeners;

        private Volume m_volume;
        public Volume volume
        {
            get
            {
                if (!m_volume)
                {
                    m_volume = base.GetComponent<Volume>();
                }
                return m_volume;
            }
        }

        [PersonalizationEditorObjectProperty(true)]
        public string voxFilePath
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(voxFilePath), string.Empty);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(voxFilePath), value);
                RefreshVolume();
            }
        }

        [PersonalizationEditorObjectProperty(0.001f, 0.1f)]
        public float voxelSize
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(voxelSize), 0.1f);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(voxelSize), value);
                RefreshVolume();
            }
        }

        [PersonalizationEditorObjectProperty]
        public bool centerPivot
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(centerPivot), true);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(centerPivot), value);
                RefreshVolume();
            }
        }

        public string colorReplacements
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(colorReplacements), string.Empty);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(colorReplacements), value);
                RefreshVolume();
            }
        }

        private void Start()
        {
            RefreshVolume();
            if (PersonalizationEditorManager.IsInEditor())
            {
                GlobalEventManager.Instance.AddEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshVolume);
                m_hasAddedEventListeners = true;
            }
        }

        private void OnDestroy()
        {
            if (m_hasAddedEventListeners)
            {
                m_hasAddedEventListeners = false;
                GlobalEventManager.Instance.RemoveEventListener(PersonalizationEditorManager.OBJECT_EDITED_EVENT, RefreshVolume);
            }
        }

        public void RefreshVolume()
        {
            Volume volumeComponent = volume;
            if (volumeComponent)
            {
                Vector3 vector = base.transform.localScale;
                base.transform.localScale = Vector3.one;
                string path = $"{ModCore.customizationFolder}{voxFilePath}";
                Debug.Log($"PATH: {path}");

                if (volumeComponent.NumFrames > 0)
                {
                    System.Collections.Generic.List<Frame> list = volumeComponent.Frames;
                    for (int i = 0; i < volumeComponent.NumFrames; i++)
                        Destroy(list[i].gameObject);

                    list.Clear();
                }

                if (!File.Exists(path))
                {
                    volumeComponent.GenerateBasic(FillMode.None);
                }
                else
                {
                    volumeComponent.AddFrame(0);
                    MagicaVoxelImporter.ImportModel(base.gameObject, path, "Import", voxelSize, centerPivot);

                    string cr = colorReplacements;
                    if (!cr.IsNullOrEmpty())
                    {
                        string[] split = cr.Split('|');
                        if (!split.IsNullOrEmpty())
                        {
                            foreach(var oldAndNewColorsString in split)
                            {
                                if (oldAndNewColorsString.IsNullOrEmpty())
                                    continue;

                                string[] oldAndNewColors = oldAndNewColorsString.Split('-');
                                if(oldAndNewColors.Length == 2)
                                {
                                    Color oldColor = ModParseUtils.TryParseToColor(oldAndNewColors[0], Color.white);
                                    Color newColor = ModParseUtils.TryParseToColor(oldAndNewColors[1], Color.white);
                                    ReplaceVoxelColor.ReplaceColors(volume, oldColor, newColor, true);
                                }
                            }
                        }
                    }
                }
                base.transform.localScale = vector;
            }
        }
    }
}
