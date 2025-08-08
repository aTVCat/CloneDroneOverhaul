using System.Collections.Generic;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorScreenshotCameraAnglesInfo
    {
        public List<PersonalizationEditorScreenshotCameraAngle> Angles;

        public void FixValues()
        {
            if (Angles == null)
                Angles = new List<PersonalizationEditorScreenshotCameraAngle>();
        }

        public PersonalizationEditorScreenshotCameraAngle GetAngle(string name)
        {
            List<PersonalizationEditorScreenshotCameraAngle> list = Angles;
            if (list == null || list.Count == 0) return null;

            for (int i = 0; i < list.Count; i++)
            {
                PersonalizationEditorScreenshotCameraAngle angle = list[i];
                if (angle.Name == name)
                {
                    return angle;
                }
            }
            return null;
        }
    }
}
