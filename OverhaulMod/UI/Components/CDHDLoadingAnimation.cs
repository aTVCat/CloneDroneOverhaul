using OverhaulMod.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class CDHDLoadingAnimation : MonoBehaviour
    {
        private const int LOADING_ANIMATION_FRAME_COUNT = 38;

        private const int LOADING_ANIMATION_FRAMERATE = 16;

        private static Sprite[] _sprites;

        private Image _image;

        private float _timeLeftToSwapFrame;

        private int _frame;

        private void Start()
        {
            if (_sprites == null)
            {
                Sprite[] sprites = new Sprite[LOADING_ANIMATION_FRAME_COUNT];
                for (int i = 0; i < LOADING_ANIMATION_FRAME_COUNT; i++)
                {
                    string numString;
                    if (i < 10)
                    {
                        numString = $"0{i}";
                    }
                    else
                    {
                        numString = i.ToString();
                    }

                    sprites[i] = ModResources.Sprite(AssetBundleConstants.UI_TRANSTION, $"Loading.000{numString}");
                }
                _sprites = sprites;
            }

            _image.enabled = true;
            _timeLeftToSwapFrame = 1f / (float)LOADING_ANIMATION_FRAMERATE;
        }

        public void SetImage(Image image)
        {
            _image = image;
            _image.enabled = false;
        }

        private void Update()
        {
            if (!_image) return;

            float frameDuration = 1f / (float)LOADING_ANIMATION_FRAMERATE;
            _timeLeftToSwapFrame -= Time.unscaledDeltaTime;
            if (_timeLeftToSwapFrame <= 0f)
            {
                _frame += 1 + Mathf.FloorToInt(Mathf.Abs(_timeLeftToSwapFrame) / frameDuration);
                _frame = _frame % (LOADING_ANIMATION_FRAME_COUNT - 1);
                _timeLeftToSwapFrame = frameDuration - (Mathf.Abs(_timeLeftToSwapFrame) % frameDuration);
            }

            _image.sprite = _sprites[_frame];
        }
    }
}