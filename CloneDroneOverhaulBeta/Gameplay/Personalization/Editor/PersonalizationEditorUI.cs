using System;
using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorUI : OverhaulUIController
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

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new Type[] { typeof(PersonalizationFileExplorer) })]
        [ObjectReference("FileExplorer")]
        public PersonalizationFileExplorer FileExplorerMenu;

        [ObjectDefaultVisibility(false)]
        [ObjectComponents(new Type[] { typeof(PersonalizationTupleEditor) })]
        [ObjectReference("TupleView")]
        public PersonalizationTupleEditor TupleEditor;

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
            _ = StaticCoroutineRunner.StartStaticCoroutine(refreshCoroutine());
        }

        private IEnumerator refreshCoroutine()
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            PropertiesWindow.Populate(PersonalizationEditor.EditingCategory);
            yield break;
        }

        public void OnCloseButtonClicked()
        {
            Hide();
        }
    }
}
