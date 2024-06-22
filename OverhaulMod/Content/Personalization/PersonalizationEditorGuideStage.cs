using System.Collections.Generic;
using UnityEngine.Events;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorGuideStage
    {
        public string Header, Description;

        public List<KeyValuePair<string, UnityAction>> Buttons;

        public PersonalizationEditorGuideStage()
        {

        }

        public PersonalizationEditorGuideStage(string header, string description, List<KeyValuePair<string, UnityAction>> buttons)
        {
            Header = header;
            Description = description;
            Buttons = buttons;
        }
    }
}
