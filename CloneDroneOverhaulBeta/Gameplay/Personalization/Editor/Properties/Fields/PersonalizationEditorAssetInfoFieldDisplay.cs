using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorAssetInfoFieldDisplay : PersonalizationEditorFieldDisplay
    {
        [ObjectReference("AssetPath")]
        private Text m_AssetPath;

        [ActionReference(nameof(editAssetInfo))]
        [ObjectReference("EditButton")]
        private Button m_EditButton;

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
            OverhaulAssetInfo assetInfo = FieldValue as OverhaulAssetInfo;
            if (assetInfo == null)
            {
                assetInfo = new OverhaulAssetInfo();
                EditingField.SetValue(TargetObject, assetInfo);
                EditorUI.SavePanel.NeedsToSave = true;
            }

            OverhaulAssetInfo info = FieldValue as OverhaulAssetInfo;

            string newText = string.Format("[{0}] {1}", new object[] { info.AssetBundle, info.AssetName });
            if(!string.IsNullOrEmpty(PreviousText) && newText != PreviousText)
            {
                EditorUI.LoadPanel.NeedToReload = true;
            }
            PreviousText = newText;
            m_AssetPath.text = string.Format("[{0}] {1}", new object[] { info.AssetBundle, info.AssetName });
        }

        private void editAssetInfo()
        {
            EditorUI.AssetInfoConfigMenu.Show(FieldValue as OverhaulAssetInfo, refresh);
        }
    }
}
