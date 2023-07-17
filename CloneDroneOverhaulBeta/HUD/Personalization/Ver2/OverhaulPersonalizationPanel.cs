using CDOverhaul.Gameplay;
using System.Collections;
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
        };

        public abstract PersonalizationCategory GetCategory();

        protected abstract void PopulateItems();
        protected abstract IEnumerator populateItemsCoroutine();

        protected virtual void SearchItems(string text, bool onlyExclusive, bool byAuthor) { }

        protected virtual void OnDoneButtonClicked()
        {
            Hide();
        }

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
