using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public PersonalizationCategory Category
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
