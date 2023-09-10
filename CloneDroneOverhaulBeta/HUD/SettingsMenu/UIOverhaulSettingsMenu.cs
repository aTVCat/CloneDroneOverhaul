using System.Collections.Generic;

namespace CDOverhaul.HUD
{
    public class UIOverhaulSettingsMenu : UIController
    {
        [PrefabContainer(0, 2)]
        private readonly PrefabContainer m_CategoriesContainer;

        [PrefabContainer(1, 2)]
        private readonly PrefabContainer m_SeparatorsContainer;

        private bool m_HasPopulatedCategories;

        public string selectedCategoryId { get; set; }

        public UIElementSettingsMenuCategoryButton recentSelectedCategoryButton
        {
            get;
            set;
        }

        protected override bool HideTitleScreen() => true;
        protected override bool WaitForEscapeKeyToHide() => true;

        public override void Show()
        {
            base.Show();
            PopulateCategories();
        }

        public void PopulateCategories(bool force = false)
        {
            if (m_HasPopulatedCategories && !force)
                return;

            selectedCategoryId = "Home";
            m_CategoriesContainer.Clear();

            recentSelectedCategoryButton = AddCategory("Home");
            _ = m_SeparatorsContainer.InstantiateEntry();

            List<string> categories = OverhaulSettingsManager.reference.GetCategories();
            foreach (string category in categories)
            {
                _ = AddCategory(category);
            }

            m_HasPopulatedCategories = true;
        }

        public UIElementSettingsMenuCategoryButton AddCategory(string category)
        {
            ModdedObject moddedObject = m_CategoriesContainer.InstantiateEntry();
            UIElementSettingsMenuCategoryButton categoryButton = moddedObject.gameObject.AddComponent<UIElementSettingsMenuCategoryButton>();
            categoryButton.categoryId = category;
            return categoryButton;
        }
    }
}
