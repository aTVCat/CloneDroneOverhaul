using OverhaulMod.Utils;
using OverhaulMod.Visuals;
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
                    for(int i = 0; i < guiRootTransform.childCount; i++)
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
