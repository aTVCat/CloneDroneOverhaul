using System;

namespace OverhaulMod.UI
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ButtonWithSoundAttribute : Attribute
    {
        public ButtonWithSound.SoundType SoundType;

        public ButtonWithSoundAttribute(ButtonWithSound.SoundType soundType)
        {
            SoundType = soundType;
        }
    }
}
