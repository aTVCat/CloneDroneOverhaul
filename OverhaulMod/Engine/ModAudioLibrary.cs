using OverhaulMod.Utils;

namespace OverhaulMod.Engine
{
    public class ModAudioLibrary : Singleton<ModAudioLibrary>
    {
        public AudioClipDefinition HyperdomeUIPause;

        public AudioClipDefinition HyperdomeUIResume;

        public AudioClipDefinition HyperdomeUIClick;

        public AudioClipDefinition HyperdomeUIChoose, HyperdomeUIChoose_NoEcho;

        public AudioClipDefinition HyperdomeUIBack, HyperdomeUIBack_NoEcho;

        private void Start()
        {
            HyperdomeUIPause = new AudioClipDefinition() { Clip = ModResources.AudioClip(AssetBundleConstants.SFX, "Hyperdome_Pause") };
            HyperdomeUIResume = new AudioClipDefinition() { Clip = ModResources.AudioClip(AssetBundleConstants.SFX, "Hyperdome_Unpause") };
            HyperdomeUIClick = new AudioClipDefinition() { Clip = ModResources.AudioClip(AssetBundleConstants.SFX, "Hyperdome_Click") };
            HyperdomeUIChoose = new AudioClipDefinition() { Clip = ModResources.AudioClip(AssetBundleConstants.SFX, "Hyperdome_Choose") };
            HyperdomeUIChoose_NoEcho = new AudioClipDefinition() { Clip = ModResources.AudioClip(AssetBundleConstants.SFX, "Hyperdome_Choose_NoEcho") };
            HyperdomeUIBack = new AudioClipDefinition() { Clip = ModResources.AudioClip(AssetBundleConstants.SFX, "Hyperdome_Back") };
            HyperdomeUIBack_NoEcho = new AudioClipDefinition() { Clip = ModResources.AudioClip(AssetBundleConstants.SFX, "Hyperdome_Back_NoEcho") };
        }
    }
}
