namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectShowIf : PersonalizationEditorObjectComponentBase
    {
        [PersonalizationEditorObjectProperty]
        public ShowPersonalizationEditorObjectIf showIf
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return (ShowPersonalizationEditorObjectIf)ob.GetPropertyValue(nameof(showIf), 0);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(showIf), (int)value);
            }
        }

        [PersonalizationEditorObjectProperty]
        public bool greatSword
        {
            get
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                return ob.GetPropertyValue(nameof(greatSword), false);
            }
            set
            {
                PersonalizationEditorObjectBehaviour ob = objectBehaviour;
                ob.SetPropertyValue(nameof(greatSword), value);
            }
        }
    }
}
