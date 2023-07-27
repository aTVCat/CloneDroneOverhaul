using System;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorUI : OverhaulUIVer2
    {
        [ObjectComponents(new Type[] { typeof(PersonalizationEditorPropertiesWindow) })]
        [ObjectReference("PropertiesPanel")]
        public PersonalizationEditorPropertiesWindow PropertiesWindow;

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
