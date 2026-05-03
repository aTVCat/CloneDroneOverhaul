using UnityEngine;

namespace OverhaulMod.Content.Personalization.Objects
{
    public class PersonalizationEditorRotator : PersonalizationEditorComponent
    {
        public Vector3 Direction
        {
            get => GetPropertyValue(nameof(PersonalizationEditorRotator), nameof(Direction), Vector3.zero);
            set => SetPropertyValue(nameof(PersonalizationEditorRotator), nameof(Direction), value);
        }

        public float Speed
        {
            get => GetPropertyValue(nameof(PersonalizationEditorRotator), nameof(Speed), 0f);
            set => SetPropertyValue(nameof(PersonalizationEditorRotator), nameof(Speed), value);
        }

        public bool PreviewInEditor;
    }
}
