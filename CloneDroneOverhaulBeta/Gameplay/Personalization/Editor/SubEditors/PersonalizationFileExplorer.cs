using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationFileExplorer : PersonalizationEditorUIElement
    {
        [UIElementDefaultVisibilityStateAttribute(false)]
        [UIElementReferenceAttribute("Shading")]
        private readonly GameObject m_Shading;

        [UIElementActionReference(nameof(OnCheckFileClicked))]
        [UIElementReferenceAttribute("CheckFile")]
        private readonly Button m_CheckFileButton;

        [UIElementReferenceAttribute("CheckFileStatusLabel")]
        private readonly Text m_CheckFileStatusLabel;

        [UIElementReferenceAttribute("FilePath")]
        private readonly InputField m_FilePath;

        private bool m_HasInitialized;

        public Action<string> Callback
        {
            get;
            set;
        }

        public bool IsCheckingFile
        {
            get;
            set;
        }

        protected override bool AssignVariablesAutomatically() => false;

        public void Show(Action<string> callBack, string initialText)
        {
            if (!m_HasInitialized)
            {
                UIController.AssignValues(this);
                UIController.AssignActionToButton(GetComponent<ModdedObject>(), "BackButton", Hide);
                UIController.AssignActionToButton(GetComponent<ModdedObject>(), "Done", OnDoneClicked);
                m_HasInitialized = true;
            }

            Callback = callBack;
            m_FilePath.text = initialText;

            base.gameObject.SetActive(true);
            m_Shading.SetActive(true);
        }

        public void Hide(bool applyChanges)
        {
            if (applyChanges)
            {
                Callback?.Invoke(m_FilePath.text);
            }
            Callback = null;

            base.gameObject.SetActive(false);
            m_Shading.SetActive(false);
        }

        public void Hide()
        {
            Hide(false);
        }

        public void OnDoneClicked()
        {
            Hide(true);
        }

        public void OnCheckFileClicked()
        {
            if (IsCheckingFile)
                return;

            _ = StaticCoroutineRunner.StartStaticCoroutine(checkFileCoroutine());
        }

        private IEnumerator checkFileCoroutine()
        {
            IsCheckingFile = true;
            m_CheckFileStatusLabel.text = "Checking...";
            yield return new WaitForSecondsRealtime(0.2f);
            if (File.Exists(OverhaulMod.Core.ModDirectory + m_FilePath.text))
            {
                m_CheckFileStatusLabel.text = "File found";
            }
            else
            {
                m_CheckFileButton.interactable = false;
                m_CheckFileStatusLabel.text = "File not found!";
                yield return new WaitForSecondsRealtime(1.5f);
                m_CheckFileStatusLabel.text = string.Empty;
                m_CheckFileButton.interactable = true;
            }
            IsCheckingFile = false;
            yield break;
        }
    }
}
