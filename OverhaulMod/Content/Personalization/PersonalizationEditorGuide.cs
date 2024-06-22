using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorGuide
    {
        public string Name;

        public List<PersonalizationEditorGuideStage> Stages;

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
