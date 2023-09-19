using System;
using System.Collections;
using UnityEngine;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorUI : UIController
    {
        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementReferenceAttribute("Shading")]
        public GameObject Shading;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new Type[] { typeof(PersonalizationEditorPropertiesWindow) })]
        [UIElementReferenceAttribute("PropertiesPanel")]
        public PersonalizationEditorPropertiesWindow PropertiesWindow;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new Type[] { typeof(PersonalizationEditorSavePanel) })]
        [UIElementReferenceAttribute("SavePanel")]
        public PersonalizationEditorSavePanel SavePanel;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new Type[] { typeof(PersonalizationEditorLoadPanel) })]
        [UIElementReferenceAttribute("LoadPanel")]
        public PersonalizationEditorLoadPanel LoadPanel;

        [UIElementComponents(new Type[] { typeof(PersonalizationEditorTypePanel) })]
        [UIElementReferenceAttribute("TypeSelectionPanel")]
        public PersonalizationEditorTypePanel TypesPanel;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new Type[] { typeof(PersonalizationEditorItemsBrowser) })]
        [UIElementReferenceAttribute("ItemsBrowser")]
        public PersonalizationEditorItemsBrowser ItemsBrowser;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new Type[] { typeof(PersonalizationPlayerInfoEditor) })]
        [UIElementReferenceAttribute("PlayerIDConfigMenu")]
        public PersonalizationPlayerInfoEditor PlayerInfoConfigMenu;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new Type[] { typeof(PersonalizationAssetInfoEditor) })]
        [UIElementReferenceAttribute("AssetInfoConfigMenu")]
        public PersonalizationAssetInfoEditor AssetInfoConfigMenu;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new Type[] { typeof(PersonalizationFileExplorer) })]
        [UIElementReferenceAttribute("FileExplorer")]
        public PersonalizationFileExplorer FileExplorerMenu;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementComponents(new Type[] { typeof(PersonalizationTupleEditor) })]
        [UIElementReferenceAttribute("TupleView")]
        public PersonalizationTupleEditor TupleEditor;

        public override void Initialize()
        {
            base.Initialize();
            AssignActionToButton(MyModdedObject, "CloseButton", OnCloseButtonClicked);
        }

        public override void Show()
        {
            base.Show();
            TypesPanel.StartTutorial();
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
