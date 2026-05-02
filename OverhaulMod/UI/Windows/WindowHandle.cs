namespace OverhaulMod.UI.Windows
{
    public struct WindowHandle
    {
        public int WindowID;

        public WindowHandle(int id)
        {
            WindowID = id;
        }

        public bool IsInvalid() => WindowID == 0;
    }
}