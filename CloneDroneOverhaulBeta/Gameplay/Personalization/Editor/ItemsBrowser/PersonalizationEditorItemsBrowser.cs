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
        [ObjectDefaultVisibility(false)]
        [ObjectReference("Shading")]
        private readonly GameObject m_Shading;

        [ObjectDefaultVisibility(false)]
        [ObjectReference("ItemPrefab")]
        private readonly ModdedObject m_Item;

        [ObjectDefaultVisibility(false)]
        [ObjectReference("CreateNewPrefab")]
        private readonly Button m_CreateItem;

        [ObjectReference("Content")]
        private readonly Transform m_Container;

        private bool m_HasInitialized;

        protected override bool AssignVariablesAutomatically() => false;

        public void Show()
        {
            if (!m_HasInitialized)
            {
                OverhaulUIController.AssignValues(this);
                OverhaulUIController.AssignActionToButton(GetComponent<ModdedObject>(), "BackButton", Hide);
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
            PersonalizationItemsController itemsController = null;
            switch (PersonalizationEditor.EditingCategory)
            {
                case PersonalizationCategory.WeaponSkins:
                    itemsController = OverhaulController.Get<WeaponSkins.WeaponSkinsController>();
                    newItem = new WeaponSkinItem();
                    break;
                case PersonalizationCategory.Outfits:
                    itemsController = OverhaulController.Get<Outfits.OutfitsController>();
                    newItem = new OutfitItem();
                    break;
                case PersonalizationCategory.Pets:
                    itemsController = OverhaulController.Get<Pets.PetsController>();
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

            if (PersonalizationEditor.EditingCategory == PersonalizationCategory.None)
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
