using OverhaulMod.Engine;
using OverhaulMod.Utils;
using PicaVoxel;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectVolume : PersonalizationEditorObjectComponentBase
    {
        private bool m_hasAddedEventListeners;

        private int m_prevFileImportVersion;

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

        [PersonalizationEditorObjectProperty]
        public int fileImportVersion
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(fileImportVersion), 0);
            }
            set
            {
                m_prevFileImportVersion = fileImportVersion;
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(fileImportVersion), value);
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
                if (value != voxFilePath)
                {
                    fileImportVersion++;
                }

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
            m_prevFileImportVersion = fileImportVersion;
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

                    int fiv = fileImportVersion;

                    if (cr.IsNullOrEmpty() || m_prevFileImportVersion != fiv)
                    {
                        m_prevFileImportVersion = fiv;

                        List<Color32> colors = new List<Color32>();
                        Frame frame = volumeComponent.GetCurrentFrame();
                        int i = 0;
                        do
                        {
                            Voxel voxel = frame.Voxels[i];
                            Color32 color32 = voxel.Color;
                            if (voxel.Active && !colors.Contains(color32))
                            {
                                colors.Add(color32);
                            }
                            i++;
                        } while (i < frame.Voxels.Length);

                        List<ColorPairFloat> colorPairs = new List<ColorPairFloat>();
                        foreach (Color32 color in colors)
                            colorPairs.Add(new ColorPairFloat(color, color));

                        colorReplacements = PersonalizationEditorManager.Instance.GetStringFromColorPairs(colorPairs);
                    }
                    else
                    {
                        List<ColorPairFloat> list = PersonalizationEditorManager.Instance.GetColorPairsFromString(cr);
                        if (!list.IsNullOrEmpty())
                        {
                            foreach (ColorPairFloat cp in list)
                                ReplaceVoxelColor.ReplaceColors(volumeComponent, cp.ColorA, cp.ColorB, false);
                        }
                    }
                }
                base.transform.localScale = vector;
            }
        }
    }
}
