using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulDialogues : OverhaulUI
    {
        public static readonly Vector2 DefaultSize = new Vector2(245, 105);
        private static OverhaulDialogues m_Instance;
        public static bool IsInitialized { get; private set; }

        private GameObject m_DialoguePrefab;
        private Transform m_DialoguesContainer;

        public override void Initialize()
        {
            m_DialoguePrefab = MyModdedObject.GetObject<Transform>(0).gameObject;
            m_DialoguePrefab.SetActive(false);
            m_DialoguesContainer = MyModdedObject.GetObject<Transform>(1);
            m_DialoguesContainer.gameObject.SetActive(true);

            m_Instance = this;
            IsInitialized = m_DialoguePrefab && m_DialoguesContainer;
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();

            m_DialoguePrefab = null;
            m_DialoguesContainer = null;
            m_Instance = null;
            IsInitialized = false;
        }

        internal void CreateDialogueInstance(string title, string description, float additionalTime, Vector2? size, Button[] buttons)
        {
            Vector2? theSize = size;
            if (theSize == null)
            {
                theSize = DefaultSize;
            }

            ModdedObject moddedObject = Instantiate(m_DialoguePrefab, m_DialoguesContainer).GetComponent<ModdedObject>();
            moddedObject.GetObject<Text>(0).text = title;
            moddedObject.GetObject<Text>(2).text = description;
            moddedObject.GetObject<Transform>(3).gameObject.SetActive(false);
            moddedObject.GetObject<Transform>(4).gameObject.SetActive(true);
            (moddedObject.transform as RectTransform).sizeDelta = theSize.Value;

            moddedObject.gameObject.SetActive(true);
            moddedObject.gameObject.AddComponent<OverhaulDialogueInstance>().Initialize(additionalTime, buttons);
        }


        public static void CreateDialogue(string title, string description, float additionalTime, Vector2? size, Button[] buttons)
        {
            if (m_Instance == null || m_Instance.IsDisposedOrDestroyed() || !IsInitialized)
            {
                return;
            }
            m_Instance.CreateDialogueInstance(title, description, additionalTime, size, buttons);
        }

        public static void Create2BDialogue(string title, string description, float additionalTime, string b1Text, UnityAction b1Act, string b2Text, UnityAction b2Act, Vector2? size)
        {
            if (m_Instance == null || m_Instance.IsDisposedOrDestroyed() || !IsInitialized)
            {
                return;
            }

            Button[] buttons = new Button[2];
            buttons[0] = new Button() { Title = b1Text, Action = b1Act };
            buttons[1] = new Button() { Title = b2Text, Action = b2Act };
            m_Instance.CreateDialogueInstance(title, description, additionalTime, size, buttons);
        }

        public static void CreateDialogueFromPreset(OverhaulDialoguePresetType presetType)
        {
            switch (presetType)
            {
                case OverhaulDialoguePresetType.UnsupportedGameVersion:
                    OverhaulDialogues.Create2BDialogue("Unsupported Clone Drone version!", "Current Overhaul mod version is made for version " + OverhaulVersion.GameTargetVersion + " of the game.\nThis may result bugs and crashes.\nYou may continue using the mod or delete one. It is better to update the mod", 24f, "Ok", null, "Visit site", delegate
                    {
                        UnityEngine.Application.OpenURL("https://modbot.org/modPreview.html?modID=rAnDomPaTcHeS1");
                    }, new Vector2(330, 145));
                    break;
            }
        }

        public class Button
        {
            public string Title;
            public UnityAction Action;
        }
    }
}