using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.HUD.Tutorial
{
    public class OverhaulTutorialUI : OverhaulUI
    {
        public static OverhaulTutorialUI Instance;

        private Transform m_UITaskTransform;
        private Image m_UITaskShading;

        private Transform m_GameplayTaskTransform;
        private Text m_GameplayTaskTitle;
        private Text m_GameplayTaskDescription;
        private CanvasGroup m_GameplayTaskCanvasGroup;

        public bool IsGameplayTaskActive
        {
            get;
            private set;
        }

        public bool IsUITaskActive
        {
            get;
            private set;
        }

        public override void Initialize()
        {
            Instance = this;

            m_UITaskTransform = MyModdedObject.GetObject<Transform>(0);
            m_UITaskShading = MyModdedObject.GetObject<Image>(0);
            m_UITaskShading.color = Color.clear;

            m_GameplayTaskTransform = MyModdedObject.GetObject<Transform>(1);
            m_GameplayTaskTitle = MyModdedObject.GetObject<Text>(2);
            m_GameplayTaskDescription = MyModdedObject.GetObject<Text>(3);
            m_GameplayTaskCanvasGroup = MyModdedObject.GetObject<CanvasGroup>(1);
            m_GameplayTaskCanvasGroup.alpha = 0f;
        }

        public void SetUITaskActive(bool value)
        {
            m_UITaskTransform.gameObject.SetActive(value);
        }

        public void SetGameplayTaskActive(bool value)
        {
            m_GameplayTaskTransform.gameObject.SetActive(value);
        }

        public void ParentTransformToUITask(Transform transform)
        {
            transform.SetParent(m_UITaskTransform, true);
        }

        public void SetGameplayTaskContext(string title, string description)
        {
            m_GameplayTaskTitle.text = title;
            m_GameplayTaskDescription.text = description;
        }

        private void Update()
        {
            float deltaTime = Time.unscaledDeltaTime * 10f;
            m_GameplayTaskCanvasGroup.alpha = Mathf.Lerp(m_GameplayTaskCanvasGroup.alpha, IsGameplayTaskActive ? 1f : 0f, deltaTime);
            m_GameplayTaskTransform.gameObject.SetActive(IsGameplayTaskActive || m_GameplayTaskCanvasGroup.alpha > 0f);

            Color color = m_UITaskShading.color;
            color.a = Mathf.Lerp(color.a, IsUITaskActive ? 0.35f : 0f, deltaTime);
            m_UITaskShading.color = color;
            m_UITaskTransform.gameObject.SetActive(IsUITaskActive || m_UITaskShading.color.a > 0f);
        }
    }
}
