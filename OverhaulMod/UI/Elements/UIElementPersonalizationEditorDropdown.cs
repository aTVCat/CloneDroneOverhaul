using OverhaulMod.Content.Personalization;
using OverhaulMod.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPersonalizationEditorDropdown : OverhaulUIBehaviour
    {
        [UIElement("Button", false)]
        private readonly Button m_buttonPrefab;

        [UIElement("Separator", false)]
        private readonly GameObject m_separatorPrefab;

        private UIElementMouseEventsComponent m_mouseChecker;

        private List<GameObject> m_spawnedEntries;

        protected override void OnInitialized()
        {
            m_mouseChecker = base.gameObject.AddComponent<UIElementMouseEventsComponent>();
            m_spawnedEntries = new List<GameObject>();
        }

        public override void Update()
        {
            base.Update();
            if (Input.GetMouseButtonDown(0) && !m_mouseChecker.isMouseOverElement)
            {
                Hide();
            }
        }

        public void ShowWithOptions(List<OptionData> list, RectTransform rectTransform)
        {
            Show();
            SetPosition(rectTransform);
            Populate(list);

            base.transform.SetAsLastSibling();
        }

        public void SetPosition(RectTransform rectTransform)
        {
            Vector3 vector = base.transform.position;
            vector.x = rectTransform.position.x - 1.4f;
            base.transform.position = vector;
        }

        public void Populate(List<OptionData> list)
        {
            List<GameObject> entiriesList = m_spawnedEntries;
            foreach (GameObject entry in entiriesList)
            {
                Destroy(entry);
            }
            entiriesList.Clear();

            bool canVerifyItems = PersonalizationEditorManager.Instance.canVerifyItems;
            foreach (OptionData od in list)
            {
                if (od.DisplayedForVerifiers && !canVerifyItems)
                    continue;

                GameObject gameObject = null;
                if (od.IsToggleOption)
                {
                }
                else if (od.IsSeparator)
                {
                    gameObject = Instantiate(m_separatorPrefab, base.transform);
                }
                else
                {
                    Sprite sprite = null;
                    try
                    {
                        sprite = ModResources.Sprite(AssetBundleConstants.UI, od.ImageName);
                    }
                    catch { }

                    Button button = Instantiate(m_buttonPrefab, base.transform);
                    button.onClick.AddListener(od.Action);
                    ModdedObject moddedObject = button.GetComponent<ModdedObject>();
                    moddedObject.GetObject<Text>(0).text = od.text;
                    Image image = moddedObject.GetObject<Image>(1);
                    image.enabled = sprite;
                    image.sprite = sprite;

                    gameObject = button.gameObject;
                }

                if (gameObject)
                {
                    gameObject.SetActive(true);
                    entiriesList.Add(gameObject);
                }
            }
        }

        public class OptionData : Dropdown.OptionData
        {
            public bool IsToggleOption;

            public bool IsSeparator;

            public string ImageName;

            public bool DisplayedForVerifiers;

            public UnityAction Action;

            public UnityAction<bool> ActionBoolean;

            public OptionData()
            {

            }

            public OptionData(bool isSeparator)
            {
                this.IsSeparator = isSeparator;
            }

            public OptionData(string text, string sprite, UnityAction unityAction)
            {
                this.text = text;
                ImageName = sprite;
                Action = unityAction;
            }

            public OptionData(string text, string sprite, UnityAction<bool> unityAction)
            {
                this.text = text;
                ImageName = sprite;
                ActionBoolean = unityAction;
                IsToggleOption = true;
            }
        }
    }
}
