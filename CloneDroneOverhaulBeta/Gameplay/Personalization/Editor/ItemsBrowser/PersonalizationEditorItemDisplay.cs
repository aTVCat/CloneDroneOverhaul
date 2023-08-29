using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationEditorItemDisplay : PersonalizationEditorUIElement
    {
        public PersonalizationItem Item
        {
            get;
            set;
        }

        public EPersonalizationCategory Category
        {
            get;
            set;
        }

        public override void Start()
        {
            base.Start();

            Button itemButton = base.GetComponent<Button>();
            itemButton.AddOnClickListener(OnClicked);
        }

        public void OnClicked()
        {
            EditorUI.ItemsBrowser.EditItem(Item);
        }
    }
}
