using CDOverhaul.Gameplay.Outfits;
using CDOverhaul.Gameplay.Pets;
using CDOverhaul.Gameplay.WeaponSkins;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorItemsBrowser : PersonalizationEditorUIElement
    {
        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementReferenceAttribute("Shading")]
        private readonly GameObject m_Shading;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementReferenceAttribute("ItemPrefab")]
        private readonly ModdedObject m_Item;

        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementReferenceAttribute("CreateNewPrefab")]
        private readonly Button m_CreateItem;

        [UIElementReferenceAttribute("Content")]
        private readonly Transform m_Container;

        private bool m_HasInitialized;

        protected override bool AssignVariablesAutomatically() => false;

        public void Show()
        {
            if (!m_HasInitialized)
            {
                UIController.AssignVariables(this);
                UIController.AssignActionToButton(GetComponent<ModdedObject>(), "BackButton", Hide);
                m_HasInitialized = true;
            }

            Populate();
            base.gameObject.SetActive(true);
            m_Shading.SetActive(true);
        }

        public void Hide()
        {
            TransformUtils.DestroyAllChildren(m_Container);
            base.gameObject.SetActive(false);
            m_Shading.SetActive(false);
        }

        public void OnCreateNewClicked()
        {
            PersonalizationItem newItem = null;
            PersonalizationItemsSystemBase itemsController = null;
            switch (PersonalizationEditor.EditingCategory)
            {
                case EPersonalizationCategory.WeaponSkins:
                    itemsController = PersonalizationManager.reference?.weaponSkins;
                    newItem = new WeaponSkinItem();
                    break;
                case EPersonalizationCategory.Outfits:
                    itemsController = PersonalizationManager.reference?.outfits;
                    newItem = new OutfitItem();
                    break;
                case EPersonalizationCategory.Pets:
                    itemsController = PersonalizationManager.reference?.pets;
                    newItem = new PetItem();
                    break;
            }

            List<PersonalizationItem> list = PersonalizationEditor.GetPersonalizationCategorySavedItems(PersonalizationEditor.EditingCategory);
            if (list == null)
                itemsController.ItemsData = new PersonalizationItemsData() { Items = new List<PersonalizationItem>() { newItem } };
            else
                list.Add(newItem);

            PersonalizationEditor.SaveEditingItem();
            EditItem(newItem);
        }

        public void EditItem(PersonalizationItem item)
        {
            PersonalizationEditor.EditingItem = item;
            EditorUI.Refresh();
            Hide();
        }

        public void Populate()
        {
            if (!m_Container)
                return;

            if (m_Container.childCount != 0)
                TransformUtils.DestroyAllChildren(m_Container);

            if (PersonalizationEditor.EditingCategory == EPersonalizationCategory.None)
                return;

            List<PersonalizationItem> list = PersonalizationEditor.GetPersonalizationCategorySavedItems(PersonalizationEditor.EditingCategory);
            if (!list.IsNullOrEmpty())
            {
                foreach (PersonalizationItem item in list)
                {
                    ModdedObject itemModdedObject = Instantiate(m_Item, m_Container);
                    itemModdedObject.gameObject.SetActive(true);
                    itemModdedObject.GetObject<Text>(0).text = item.Name;
                    PersonalizationEditorItemDisplay display = itemModdedObject.gameObject.AddComponent<PersonalizationEditorItemDisplay>();
                    display.Item = item;
                    display.Category = PersonalizationEditor.EditingCategory;
                }
            }

            Button createNewButton = Instantiate(m_CreateItem, m_Container);
            createNewButton.gameObject.SetActive(true);
            createNewButton.AddOnClickListener(OnCreateNewClicked);
        }
    }
}
