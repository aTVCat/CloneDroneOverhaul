using UnityEngine;

namespace CDOverhaul.Patches
{
    public class NewAudioController : ReplacementBase
    {
        public override void Replace()
        {
            base.Replace();

            SuccessfullyPatched = true;
        }

        public static void TakeControlOverAudioSource(in AudioSource source)
        {

        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
