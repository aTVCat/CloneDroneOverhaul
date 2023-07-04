using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul
{
    public class OverhaulFullscreenDialogueWindow : OverhaulUI
    {
        public const float DefaultWidth = 300f;
        public const float DefaultHeight = 175f;

        public const float MainButtonPreferredWidth = 100f;

        public static OverhaulFullscreenDialogueWindow Instance;

        private RectTransform m_WindowTransform;

        private Text m_TitleLabel;
        private Text m_DescriptionLabel;
        private GameObject[] m_Icons;
        private PrefabAndContainer m_ButtonsContainer;

        public override void Initialize()
        {
            Instance = this;
            m_WindowTransform = MyModdedObject.GetObject<RectTransform>(0);
            m_TitleLabel = MyModdedObject.GetObject<Text>(1);
            m_DescriptionLabel = MyModdedObject.GetObject<Text>(2);
            m_Icons = new GameObject[]
            {
                 MyModdedObject.GetObject<Transform>(3).gameObject
            };
            m_ButtonsContainer = new PrefabAndContainer(MyModdedObject, 4, 5);
            Hide();
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
        }

        public void ResetContents()
        {
            SetIcon(IconType.None);
            SetWindowSize(DefaultWidth, DefaultHeight);
            m_ButtonsContainer.ClearContainer();
        }

        public void SetWindowSize(float width, float height)
        {
            Vector2 vector = new Vector2(width, height);
            m_WindowTransform.sizeDelta = vector;
        }

        public void SetTitle(string text)
        {
            m_TitleLabel.text = text;
        }

        public void SetDescription(string text)
        {
            m_DescriptionLabel.text = text;
        }

        public void SetIcon(IconType icon)
        {
            int iconIndex = (int)icon - 1;
            int i = 0;
            foreach(GameObject gameObject in m_Icons)
            {
                if (gameObject)
                {
                    gameObject.SetActive(i == iconIndex);
                }
                i++;
            }
        }

        public void AddButton(string text, Action onClickAction, string frameHexColor = "#FFFFFF", Func<bool> setInteractableIf = null, float preferredWidth = 0f)
        {
            ModdedObject moddedObject = m_ButtonsContainer.CreateNew();
            moddedObject.GetObject<Text>(0).text = text;
            moddedObject.GetObject<Image>(1).color = frameHexColor.ToColor();
            LayoutElement layoutElement = moddedObject.GetComponent<LayoutElement>();
            layoutElement.preferredWidth = preferredWidth;
            ActionButton actionButton = moddedObject.gameObject.AddComponent<ActionButton>();
            actionButton.ClickedAction = onClickAction;
            actionButton.BecomeInteractableIf = setInteractableIf;
        }

        public static void ShowOkWindow(string title, string desc, float width, float height, IconType icon)
        {
            Instance.ResetContents();
            Instance.SetTitle(title);
            Instance.SetDescription(desc);
            Instance.SetWindowSize(width, height);
            Instance.SetIcon(icon);
            Instance.AddButton("OK", delegate
            {
                Instance.Hide();
            });
            Instance.Show();
        }

        public static void ShowCancelOkWindow(string title, string desc, float width, float height, IconType icon, string okButtonText, Action okClicked)
        {
            Instance.ResetContents();
            Instance.SetTitle(title);
            Instance.SetDescription(desc);
            Instance.SetWindowSize(width, height);
            Instance.SetIcon(icon);
            Instance.AddButton("Cancel", delegate
            {
                Instance.Hide();
            });
            Instance.AddButton(okButtonText, delegate
            {
                if (okClicked != null)
                {
                    okClicked.Invoke();
                }
                Instance.Hide();
            }, "#FFFFFF", null, 100f);
            Instance.Show();
        }

        public class ActionButton : OverhaulBehaviour
        {
            private Button m_Button;

            public OverhaulFullscreenDialogueWindow DialogueWindow;
            public Action ClickedAction;
            public Func<bool> BecomeInteractableIf;

            public override void Start()
            {
                m_Button = base.GetComponent<Button>();
                m_Button.interactable = true;
                m_Button.onClick.AddListener(onClicked);
            }

            private void onClicked()
            {
                if(ClickedAction != null)
                {
                    ClickedAction.Invoke();
                }
            }

            private void LateUpdate()
            {
                if(BecomeInteractableIf != null)
                {
                    m_Button.interactable = BecomeInteractableIf();
                }
            }
        }

        public enum IconType
        {
            None,

            Warn
        }
    }
}
