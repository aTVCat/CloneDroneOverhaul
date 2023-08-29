using System.Reflection;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorVoxAssetInfoFieldDisplay : PersonalizationEditorFieldDisplay
    {
        [UIElementActionReference(nameof(editAssetInfo))]
        [UIElementReferenceAttribute("EditButton")]
        private readonly Button m_EditButton;

        [UIElementReferenceAttribute("FilePath")]
        private readonly Text m_FilePath;

        [UIElementActionReference(nameof(onVolumeNameChanged))]
        [UIElementReferenceAttribute("VolumeNameField")]
        private readonly InputField m_VolumeNameField;

        [UIElementActionReference(nameof(onVoxelSizeChanged))]
        [UIElementReferenceAttribute("VoxelSizeField")]
        private readonly InputField m_VoxelSizeField;

        [UIElementActionReference(nameof(onCenterPivotValueChanged))]
        [UIElementReferenceAttribute("CenterPivot")]
        private readonly Toggle m_CenterPivot;

        private bool m_IsInitializing;

        public string PreviousText
        {
            get;
            set;
        }

        public override void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            m_IsInitializing = true;
            base.Initialize(fieldToEdit, targetObject);
            refresh();
            m_IsInitializing = false;
        }

        private void refresh()
        {
            if (!(FieldValue is OverhaulVoxAssetInfo assetInfo))
            {
                assetInfo = new OverhaulVoxAssetInfo();
                FieldValue = assetInfo;
            }

            string newText = assetInfo.PathUnderModFolder;
            if (!string.IsNullOrEmpty(PreviousText) && newText != PreviousText)
            {
                EditorUI.LoadPanel.NeedsToReload = true;
            }
            PreviousText = newText;

            m_FilePath.text = newText;
            m_VolumeNameField.text = assetInfo.VolumeName;
            m_VoxelSizeField.text = assetInfo.VoxelSize.ToString().Replace(',', '.');
            m_CenterPivot.isOn = assetInfo.CenterPivot;
        }

        private void editAssetInfo()
        {
            EditorUI.FileExplorerMenu.Show(onEndedEditingFilePath, m_FilePath.text);
        }

        private void onVolumeNameChanged(string newValue)
        {
            if (!(FieldValue is OverhaulVoxAssetInfo assetInfo))
            {
                m_VolumeNameField.text = "ERROR";
                return;
            }

            if (m_IsInitializing)
                return;

            assetInfo.VolumeName = m_VolumeNameField.text;
            OnFieldValueChanged();
        }

        private void onVoxelSizeChanged(string newValue)
        {
            if (!(FieldValue is OverhaulVoxAssetInfo assetInfo))
            {
                m_VolumeNameField.text = "ERROR";
                return;
            }

            newValue = newValue.Replace('.', ',').Replace(" ", string.Empty);
            if (!float.TryParse(newValue, out float result))
            {
                m_VolumeNameField.text = "0.1";
                return;
            }

            if (m_IsInitializing)
                return;

            assetInfo.VoxelSize = result;
            EditorUI.LoadPanel.NeedsToReload = true;
            OnFieldValueChanged();
        }

        private void onCenterPivotValueChanged(bool newValue)
        {
            if (!(FieldValue is OverhaulVoxAssetInfo assetInfo))
            {
                m_CenterPivot.isOn = true;
                return;
            }

            if (m_IsInitializing)
                return;

            assetInfo.CenterPivot = newValue;
            EditorUI.LoadPanel.NeedsToReload = true;
            OnFieldValueChanged();
        }

        private void onEndedEditingFilePath(string newPath)
        {
            if (!(FieldValue is OverhaulVoxAssetInfo assetInfo))
                return;

            assetInfo.PathUnderModFolder = newPath;
            OnFieldValueChanged();
            refresh();
        }
    }
}
