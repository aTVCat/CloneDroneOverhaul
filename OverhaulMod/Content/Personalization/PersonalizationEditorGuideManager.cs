using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorGuideManager : Singleton<PersonalizationEditorGuideManager>
    {
        private List<PersonalizationEditorGuide> m_guides;

        private void Start()
        {
            m_guides = new List<PersonalizationEditorGuide>()
            {
                new PersonalizationEditorGuide("test", new List<PersonalizationEditorGuideStage>()
                {
                    new PersonalizationEditorGuideStage("How to become a sigma?", "This guide will help you to become a sigma", new List<KeyValuePair<string, UnityEngine.Events.UnityAction>>()
                    {
                        new KeyValuePair<string, UnityEngine.Events.UnityAction>("No way", null)
                    }),
                    new PersonalizationEditorGuideStage("How to become a sigma?", "Stage 1 - Get real", new List<KeyValuePair<string, UnityEngine.Events.UnityAction>>()
                    {
                        new KeyValuePair<string, UnityEngine.Events.UnityAction>("I can't :sob:", null)
                    }),
                    new PersonalizationEditorGuideStage("How to become a sigma?", "Stage 2 - Quit Clone Drone", new List<KeyValuePair<string, UnityEngine.Events.UnityAction>>()
                    {
                        new KeyValuePair<string, UnityEngine.Events.UnityAction>("No!!!11!", null)
                    }),
                    new PersonalizationEditorGuideStage("How to become a sigma?", "Stage 3 - Touch grass.\nThis is easier than you think", null),
                })
            };
        }

        public PersonalizationEditorGuide GetGuide(string name)
        {
            foreach (var guide in m_guides)
                if (guide.Name == name)
                    return guide;

            return null;
        }
    }
}
