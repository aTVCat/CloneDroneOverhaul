using System.Reflection;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorAssetInfoFieldDisplay : PersonalizationEditorFieldDisplay
    {
        [UIElementReferenceAttribute("AssetPath")]
        private readonly Text m_AssetPath;

        [UIElementActionReference(nameof(editAssetInfo))]
        [UIElementReferenceAttribute("EditButton")]
        private readonly Button m_EditButton;

        public string PreviousText
        {
            get;
            set;
        }

        public override void Initialize(FieldInfo fieldToEdit, object targetObject)
        {
            base.Initialize(fieldToEdit, targetObject);
            refresh();
        }

        private void refresh()
        {
            if (!(FieldValue is OverhaulAssetInfo assetInfo))
            {
                assetInfo = new OverhaulAssetInfo();
                FieldValue = assetInfo;
            }

            string newText = string.Format("[{0}] {1}", new object[] { assetInfo.AssetBundle, assetInfo.AssetName });
            if (!string.IsNullOrEmpty(PreviousText) && newText != PreviousText)
            {
                EditorUI.LoadPanel.NeedsToReload = true;
            }
            PreviousText = newText;
            m_AssetPath.text = newText;
        }

        private void editAssetInfo()
        {
            EditorUI.AssetInfoConfigMenu.Show(FieldValue as OverhaulAssetInfo, refresh);
        }
    }
}
