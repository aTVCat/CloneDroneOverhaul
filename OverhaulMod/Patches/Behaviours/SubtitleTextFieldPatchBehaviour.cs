using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches.Behaviours
{
    internal class SubtitleTextFieldPatchBehaviour : GamePatchBehaviour
    {
        private int _initialSiblingIndex;

        private Transform _targetTransform;
        private Transform targetTransform
        {
            get
            {
                if (!_targetTransform)
                {
                    Transform guiRootTransform = ModCache.gameUIRoot.transform;
                    for (int i = 0; i < guiRootTransform.childCount; i++)
                    {
                        Transform child = guiRootTransform.GetChild(i);
                        if (child.name == "SpeechSubtitles")
                        {
                            _targetTransform = child;
                            break;
                        }
                    }
                }
                return _targetTransform;
            }
        }

        public override void Patch()
        {
            Transform transform = targetTransform;
            if (transform && !transform.GetComponent<CanvasGroup>())
            {
                CanvasGroup canvasGroup = transform.gameObject.AddComponent<CanvasGroup>();
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        public override void UnPatch()
        {
            Transform transform = targetTransform;
            if (transform)
            {
                CanvasGroup canvasGroup = transform.GetComponent<CanvasGroup>();
                if (canvasGroup)
                {
                    Destroy(canvasGroup);
                }
            }
        }

        public void SetSiblingIndex(Transform transform)
        {
            Transform transform1 = targetTransform;
            if (transform && transform1)
            {
                _initialSiblingIndex = transform1.GetSiblingIndex();
                transform1.SetSiblingIndex(transform.GetSiblingIndex() + 1);
            }
        }

        public void ResetSiblingIndex()
        {
            Transform transform1 = targetTransform;
            if (transform1)
            {
                transform1.SetSiblingIndex(_initialSiblingIndex);
            }
        }
    }
}
