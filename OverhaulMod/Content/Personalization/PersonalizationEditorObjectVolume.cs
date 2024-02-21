using PicaVoxel;
using System.IO;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectVolume : PersonalizationEditorObjectComponentBase
    {
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

        public string voxFilePath
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return (string)ob.GetPropertyValue(nameof(voxFilePath), string.Empty);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(voxFilePath), value);
                RefreshVolume();
            }
        }

        public float voxelSize
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return (float)ob.GetPropertyValue(nameof(voxelSize), 0.1f);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(voxelSize), value);
                RefreshVolume();
            }
        }

        public bool centerPivot
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return (bool)ob.GetPropertyValue(nameof(centerPivot), true);
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
                string path = ModCore.customizationFolder + voxFilePath;
                if (!File.Exists(path))
                {
                    volume.GenerateBasic(FillMode.None);
                }
                else
                {
                    MagicaVoxelImporter.ImportModel(base.gameObject, path, "Import", voxelSize, centerPivot);
                }
                base.transform.localScale = vector;
            }
        }
    }
}
