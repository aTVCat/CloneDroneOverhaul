using System.Collections.Generic;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorGuideManager : Singleton<PersonalizationEditorGuideManager>
    {
        private List<PersonalizationEditorGuide> m_guides;

        private void Start()
        {
            m_guides = new List<PersonalizationEditorGuide>()
            {
                new PersonalizationEditorGuide("getting_started", new List<PersonalizationEditorGuideStage>()
                {
                    new PersonalizationEditorGuideStage("Introduction", "This guide will help you in making a simple weapon skin.\n\n(Everything is still in development, so there might be mistakes)", null),
                    new PersonalizationEditorGuideStage("Introduction", "Click on \"File\" at the top left, then on \"Open\".\nClick on \"New\", name your item then click on \"Done\".", null),
                    new PersonalizationEditorGuideStage("Introduction", "Some windows will popup. You can drag, minimize/maximize and close them.\nTo display them again, click on \"Windows\" at top left.", null),
                    new PersonalizationEditorGuideStage("Introduction", "\"Edit item info\" is the panel you can change your item properties with.", null),
                    new PersonalizationEditorGuideStage("Introduction", "Scroll down a bit and look at \"Import files\" panel. This is where you can attach files to your item.", null),
                    new PersonalizationEditorGuideStage("Introduction", "Click on \"+ .vox\" button and import any voxel model you have on your device.", null),
                    new PersonalizationEditorGuideStage("Introduction", "Scroll down again. You'll see the \"Hierarchy\" panel. With this one you can make the stuff you imported visible.", null),
                    new PersonalizationEditorGuideStage("Introduction", "Click on \"+ New\" button and select \"Model Renderer (.vox)\" object. It will appear in the hierarchy panel. Click on the button with pen icon on it.", null),
                    new PersonalizationEditorGuideStage("Introduction", "Now, the \"Edit object\" panel. You can configure object's position, rotation, scale and properties with it.", null),
                    new PersonalizationEditorGuideStage("Introduction", "Almost every object has that \"Show if weapon variant is..\" setting. This one makes the object visible if the weapon configuration matches the option you've selected.", null),
                    new PersonalizationEditorGuideStage("Introduction", ".vox model renderers also have \"Hide if no preset\" option. This one disables the renderer if weapon doesn't match any of presets* this object has.", null),
                    new PersonalizationEditorGuideStage("Introduction", "Presets are configurations for each variant of weapon. Each config contains info about the model and its color settings.", null),
                    new PersonalizationEditorGuideStage("Introduction", "Click on \"New preset\" button. A small panel will show up. Press the small button with pen icon and choose the file you imported earlier. The voxel model should appear in the hands of player.", null),
                    new PersonalizationEditorGuideStage("Introduction", "Now you probably have to mess with object's position, rotation and scale to make it match the original weapon model", null),
                    new PersonalizationEditorGuideStage("Introduction", "You can also configure colors in every preset. Click on \"Edit colors\" button and try playing around with the colors your model has.", null),
                    new PersonalizationEditorGuideStage("Introduction", "In order to make your item look complete, make presets for every variant of the weapon.", null),
                    new PersonalizationEditorGuideStage("Weapon variants", "P.S. Each weapon might be set on fire or be replaced with its multiplayer model.", null),
                    new PersonalizationEditorGuideStage("Finishing", "After you consider your item finished, you can upload it by pressing \"File\" button and selecting \"Upload\"", null),
                    new PersonalizationEditorGuideStage("Finishing", "It will take a while until your item will be verified. You can check for customization assets updates that might contain your item verified so everyone will be able to use it.\nThis can be done via player customization menu (\"Updates\" button at bottom left)", null),
                })
                {
                    IsTranslated = true
                }
            };
        }

        public PersonalizationEditorGuide GetGuide(string name)
        {
            foreach (PersonalizationEditorGuide guide in m_guides)
                if (guide.Name == name)
                    return guide;

            return null;
        }
    }
}
