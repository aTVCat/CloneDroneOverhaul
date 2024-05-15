using PicaVoxel;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectVolume : PersonalizationEditorObjectComponentBase
    {
        private Volume m_volume;

        [PersonalizationEditorObjectProperty]
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

        private void Start()
        {
            RefreshVolume();
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

                    foreach (var rvc in volumeComponent.GetComponents<ReplaceVoxelColor>())
                        Destroy(rvc);
                }
                base.transform.localScale = vector;
            }
        }
    }
}
