using System;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorUI : OverhaulUIVer2
    {
        [ObjectComponents(new Type[] { typeof(PersonalizationEditorPropertiesWindow) })]
        [ObjectReference("PropertiesPanel")]
        public PersonalizationEditorPropertiesWindow PropertiesWindow;

        [ObjectComponents(new Type[] { typeof(PersonalizationEditorSavePanel) })]
        [ObjectReference("SavePanel")]
        public PersonalizationEditorSavePanel SavePanel;

        [ObjectComponents(new Type[] { typeof(PersonalizationEditorLoadPanel) })]
        [ObjectReference("LoadPanel")]
        public PersonalizationEditorLoadPanel LoadPanel;

        [ObjectComponents(new Type[] { typeof(PersonalizationEditorTypePanel) })]
        [ObjectReference("TypeSelectionPanel")]
        public PersonalizationEditorTypePanel TypesPanel;

        public override void Initialize()
        {
            base.Initialize();

            AssignActionToButton(MyModdedObject, "CloseButton", OnCloseButtonClicked);
        }

        public override void Show()
        {
            base.Show();
            ShowCursor = true;
        }

        public override void Hide()
        {
            base.Hide();
            ShowCursor = false;
        }

        public void OnCloseButtonClicked()
        {
            Hide();
        }
    }
}
