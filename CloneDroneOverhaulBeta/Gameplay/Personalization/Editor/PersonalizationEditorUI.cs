using System;
using UnityEngine;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorUI : OverhaulUIVer2
    {
        [ObjectDefaultVisibility(false)]
        [ObjectReference("Shading")]
        public GameObject Shading;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new Type[] { typeof(PersonalizationEditorPropertiesWindow) })]
        [ObjectReference("PropertiesPanel")]
        public PersonalizationEditorPropertiesWindow PropertiesWindow;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new Type[] { typeof(PersonalizationEditorSavePanel) })]
        [ObjectReference("SavePanel")]
        public PersonalizationEditorSavePanel SavePanel;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new Type[] { typeof(PersonalizationEditorLoadPanel) })]
        [ObjectReference("LoadPanel")]
        public PersonalizationEditorLoadPanel LoadPanel;

        [ObjectComponents(new Type[] { typeof(PersonalizationEditorTypePanel) })]
        [ObjectReference("TypeSelectionPanel")]
        public PersonalizationEditorTypePanel TypesPanel;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new Type[] { typeof(PersonalizationEditorItemsBrowser) })]
        [ObjectReference("ItemsBrowser")]
        public PersonalizationEditorItemsBrowser ItemsBrowser;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new Type[] { typeof(PersonalizationPlayerInfoEditor) })]
        [ObjectReference("PlayerIDConfigMenu")]
        public PersonalizationPlayerInfoEditor PlayerInfoConfigMenu;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new Type[] { typeof(PersonalizationAssetInfoEditor) })]
        [ObjectReference("AssetInfoConfigMenu")]
        public PersonalizationAssetInfoEditor AssetInfoConfigMenu;

        public override void Initialize()
        {
            base.Initialize();
            AssignActionToButton(MyModdedObject, "CloseButton", OnCloseButtonClicked);
        }

        public override void Show()
        {
            base.Show();
            ShowCursor = true;

            TypesPanel.StartTutorial();
        }

        public override void Hide()
        {
            base.Hide();
            ShowCursor = false;
        }

        public void Refresh()
        {
            SavePanel.gameObject.SetActive(true);
            LoadPanel.gameObject.SetActive(true);
            TypesPanel.gameObject.SetActive(true);
            PropertiesWindow.gameObject.SetActive(true);
            PropertiesWindow.Populate(PersonalizationEditor.EditingCategory);
        }

        public void OnCloseButtonClicked()
        {
            Hide();
        }
    }
}
