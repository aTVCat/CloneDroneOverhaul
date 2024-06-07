using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Patches.Behaviours
{
    internal class SubtitleTextFieldPatchBehaviour : GamePatchBehaviour
    {
        private int m_initialSiblingIndex;

        private Transform m_targetTransform;
        private Transform targetTransform
        {
            get
            {
                if (!m_targetTransform)
                {
                    Transform guiRootTransform = ModCache.gameUIRoot.transform;
                    for (int i = 0; i < guiRootTransform.childCount; i++)
                    {
                        Transform child = guiRootTransform.GetChild(i);
                        if (child.name == "SpeechSubtitles")
                        {
                            m_targetTransform = child;
                            break;
                        }
                    }
                }
                return m_targetTransform;
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
                m_initialSiblingIndex = transform1.GetSiblingIndex();
                transform1.SetSiblingIndex(transform.GetSiblingIndex() + 1);
            }
        }

        public void ResetSiblingIndex()
        {
            Transform transform1 = targetTransform;
            if (transform1)
            {
                transform1.SetSiblingIndex(m_initialSiblingIndex);
            }
        }
    }
}
