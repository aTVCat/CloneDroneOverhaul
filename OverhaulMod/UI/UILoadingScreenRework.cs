using OverhaulMod.Engine;
using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UILoadingScreenRework : OverhaulUIBehaviour
    {
        private ChapterLoadingScreen _chapterLoadingScreen;
        private LoadingScreen _loadingScreen;

        [UIElement("Description")]
        private readonly Text _stateText;

        [UIElement("ObjectProgress")]
        private readonly Text _objectProgressText;

        [UIElement("Fill")]
        private readonly Image _progressBarFill;

        [UIElement("ProgressBar")]
        private readonly GameObject _progressBarObject;

        public override bool CloseOnEscapeButtonPress => false;

        public override void Show()
        {
            base.Show();
            _progressBarObject.SetActive(false);
            _stateText.text = "Please wait";
            _objectProgressText.text = string.Empty;
            _progressBarFill.fillAmount = 0f;
            _chapterLoadingScreen = ModCache.gameUIRoot.ChapterLoadingScreen;
            _loadingScreen = ModCache.gameUIRoot.LoadingScreen;

            if (TransitionManager.TransitionSound)
                ModAudioManager.Instance.PlayTransitionSound((!GameModeManager.UsesAsyncLevelLoading() || GameModeManager.IsWorkshopChallenge()) ? 0.5f : 0f);
        }

        public override void Hide()
        {
            base.Hide();
            if (!TransitionManager.Instance.IsDoingTransition())
                ModAudioManager.Instance.StopTransitionSound();
        }

        public string GetStateString(ChapterLoadingScreenState chapterLoadingScreenState)
        {
            switch (chapterLoadingScreenState)
            {
                case ChapterLoadingScreenState.NOT_STARTED:
                    return "Please wait";
                case ChapterLoadingScreenState.LOADING_LEVEL:
                    return "Loading level data";
                case ChapterLoadingScreenState.LOADING_OBJECTS:
                    return "Spawning objects";
                case ChapterLoadingScreenState.STARTING_OBJECTS:
                    return "Starting objects";
                case ChapterLoadingScreenState.DONE:
                    return "Complete!";
            }
            return chapterLoadingScreenState.ToString();
        }

        public override void Update()
        {
            ErrorManager errorManager = ErrorManager.Instance;
            if (errorManager && errorManager.HasCrashed())
            {
                Hide();
            }

            ChapterLoadingScreen chapterLoadingScreen = _chapterLoadingScreen;
            if (chapterLoadingScreen)
            {
                if (Time.frameCount % 5 == 0)
                {
                    if (GameModeManager.Is(GameMode.Story))
                    {
                        _stateText.text = GetStateString(chapterLoadingScreen._screenState);
                        _progressBarObject.SetActive(chapterLoadingScreen.gameObject.activeInHierarchy);
                        _objectProgressText.enabled = true;
                        _objectProgressText.text = $"Instantiated: {chapterLoadingScreen._objectsToInstantiateTotal - chapterLoadingScreen._objectsToInstantiateRemaining}/{chapterLoadingScreen._objectsToInstantiateTotal}\nStarted: {chapterLoadingScreen._objectsToAwakenTotal - chapterLoadingScreen._objectstoAwakenRemaining}/{chapterLoadingScreen._objectsToAwakenTotal}";
                    }
                    else
                    {
                        _stateText.text = "Please wait";
                        _objectProgressText.enabled = false;
                        _progressBarObject.SetActive(false);
                    }
                }
                _progressBarFill.fillAmount = chapterLoadingScreen.getProgress();
            }
        }
    }
}
