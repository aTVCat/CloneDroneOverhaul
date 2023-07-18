using CDOverhaul.Gameplay;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD
{
    /// <summary>
    /// An updated version of <see cref="PersonalizationMenu"/> to be implemented in 0.3 updates/0.4
    /// </summary>
    public abstract class OverhaulPersonalizationPanel : OverhaulUI
    {
        public bool IsPopulatingItems
        {
            get;
            protected set;
        }

        protected Button DoneButton;
        protected PrefabAndContainer MainItemContainer;

        public override void Initialize()
        {
            int[] array = GetObjectIndexes();
            DoneButton = MyModdedObject.GetObject<Button>(array[0]);
            DoneButton.onClick.AddListener(OnDoneButtonClicked);
            MainItemContainer = new PrefabAndContainer(MyModdedObject, array[1], array[2]);

            GameObject panelBG = MyModdedObject.GetObject<Transform>(array[6]).gameObject;
            OverhaulUIAnchoredPanelSlider slider = panelBG.AddComponent<OverhaulUIAnchoredPanelSlider>();
            slider.StartPosition = new Vector3(-300f, 0f, 0f);
            slider.TargetPosition = GetTargetPosition();

            Hide();
        }

        public override void OnEnable()
        {
            PopulateItems();
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
            ShowCursor = true;
        }

        public void Hide()
        {
            base.gameObject.SetActive(false);
            ShowCursor = false;
        }

        /// <summary>
        /// Get object indexes for <see cref="ModdedObject"/><br></br>
        /// 0 - Done Button<br></br>
        /// 1 - Main item prefab<br></br>
        /// 2 - Main item container<br></br>
        /// 3 - Search box
        /// 4 - Search exclusive button-toggle
        /// 5 - Search by dropdown
        /// 6 - Panel BG
        /// </summary>
        /// <returns></returns>
        public virtual int[] GetObjectIndexes() => new int[]
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
        };

        public abstract PersonalizationCategory GetCategory();

        protected abstract void PopulateItems();
        protected abstract IEnumerator populateItemsCoroutine();

        protected virtual void SearchItems(string text, bool onlyExclusive, bool byAuthor) { }

        protected virtual void OnDoneButtonClicked()
        {
            Hide();
        }

        protected virtual Vector3 GetTargetPosition() => new Vector3(25f, 0f, 0f);

        public static OverhaulPersonalizationPanel GetPanel(PersonalizationCategory category)
        {
            OverhaulPersonalizationPanel[] panels = GetControllers<OverhaulPersonalizationPanel>();
            if (panels.IsNullOrEmpty())
                return null;

            foreach (OverhaulPersonalizationPanel panel in panels)
                if (panel.GetCategory() == category)
                    return panel;

            return null;
        }
    }
}
