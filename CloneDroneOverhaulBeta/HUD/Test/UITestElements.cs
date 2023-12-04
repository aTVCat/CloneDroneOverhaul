namespace CDOverhaul.HUD
{
    public class UITestElements : UIController
    {
        [UIElementReference("UIElement_Dropdown")]
        private readonly UIElementDropdown m_Dropdown;

        protected override bool HideTitleScreen() => true;
        protected override bool WaitForEscapeKeyToHide() => true;

        public override void Initialize()
        {
            base.Initialize();
        }
    }
}
