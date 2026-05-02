using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.UI.Windows
{
    public class WindowManager : Singleton<WindowManager>
    {
        public const string WINDOW_SELECTED_EVENT = "UIWindowSelected";

        private ModdedObject _windowPrefab;

        private Dictionary<WindowHandle, WindowBehaviour> _windows;

        private WindowHandle _selectedWindow;

        public override void Awake()
        {
            base.Awake();
            _windows = new Dictionary<WindowHandle, WindowBehaviour>();
            _windowPrefab = ModResources.Prefab(ModAssetBundles.UI, "WindowPrefab").GetComponent<ModdedObject>();
        }

        public WindowHandle NewWindow(Transform parent, RectTransform content, string title, WindowRectSettings rectSettings)
        {
            WindowHandle windowHandle = default;
            while (windowHandle.IsInvalid() || _windows.ContainsKey(windowHandle))
                windowHandle = new WindowHandle(UnityEngine.Random.Range(1, int.MaxValue));

            Rect rectToSet = new Rect();
            if (rectSettings.PreserveContentSize)
            {
                rectToSet.width = content.sizeDelta.x + 20f;
                rectToSet.height = content.sizeDelta.y + 45f;
            }
            else
            {
                rectToSet.width = rectSettings.Rect.width;
                rectToSet.height = rectSettings.Rect.height;
            }
            rectToSet.x = rectSettings.Rect.x;
            rectToSet.y = rectSettings.Rect.y;

            ModdedObject moddedObject = Instantiate(_windowPrefab, parent);
            moddedObject.gameObject.SetActive(true);
            moddedObject.gameObject.name = title;
            WindowBehaviour windowBehaviour = moddedObject.gameObject.AddComponent<WindowBehaviour>();
            windowBehaviour.InitializeAsElement();
            windowBehaviour.SetPivot(rectSettings.Pivot);
            windowBehaviour.SetRect(rectToSet);
            windowBehaviour.SetTitle(title);
            windowBehaviour.SetContents(content);
            windowBehaviour.Handle = windowHandle;
            _windows.Add(windowHandle, windowBehaviour);

            return windowHandle;
        }

        public WindowBehaviour GetWindow(WindowHandle handle)
        {
            if (_windows.TryGetValue(handle, out WindowBehaviour windowBehaviour) && windowBehaviour)
            {
                return windowBehaviour;
            }
            return null;
        }

        public void ShowWindow(WindowHandle handle)
        {
            WindowBehaviour windowBehaviour = GetWindow(handle);
            if (windowBehaviour) windowBehaviour.Show();
        }

        public void HideWindow(WindowHandle handle)
        {
            WindowBehaviour windowBehaviour = GetWindow(handle);
            if (windowBehaviour) windowBehaviour.Hide();
        }

        public bool IsWindowShown(WindowHandle handle)
        {
            WindowBehaviour windowBehaviour = GetWindow(handle);
            return windowBehaviour && windowBehaviour.IsVisible;
        }

        public void RemoveWindow(WindowHandle handle)
        {
            _ = _windows.Remove(handle);
        }

        public void SelectWindow(WindowHandle handle)
        {
            _selectedWindow = handle;
            GlobalEventManager.Instance.Dispatch(WINDOW_SELECTED_EVENT);
        }

        public WindowHandle GetSelectedWindowHandle() => _selectedWindow;

        public List<WindowHandle> GetWindowHandles() => new List<WindowHandle>(_windows.Keys);
    }
}