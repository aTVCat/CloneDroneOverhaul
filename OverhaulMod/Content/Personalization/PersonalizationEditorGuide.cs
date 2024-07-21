using System.Collections.Generic;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorGuide
    {
        public string Name;

        public List<PersonalizationEditorGuideStage> Stages;

        public bool IsTranslated;

        public PersonalizationEditorGuide()
        {

        }

        public PersonalizationEditorGuide(string name, List<PersonalizationEditorGuideStage> stages)
        {
            Name = name;
            Stages = stages;
        }
    }
}
