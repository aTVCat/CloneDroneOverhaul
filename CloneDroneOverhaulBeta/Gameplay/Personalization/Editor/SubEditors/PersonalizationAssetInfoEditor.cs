using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Gameplay.Editors.Personalization
{
    public class PersonalizationAssetInfoEditor : PersonalizationEditorUIElement
    {
        [ObjectDefaultVisibility(false)]
        [ObjectReference("Shading")]
        private readonly GameObject m_Shading;

        [ObjectReference("AssetBundle")]
        private readonly InputField m_AssetBundleField;

        [ObjectReference("AssetName")]
        private readonly InputField m_AssetField;

        [ObjectReference("FixMaterialsToggle")]
        private readonly Toggle m_FixMaterialsToggle;

        [ObjectReference("CheckAsset")]
        private readonly Button m_CheckAssetButton;

        [ObjectReference("CheckAssetStateLabel")]
        private readonly Text m_CheckAssetStateLabel;

        private bool m_HasInitialized;

        public bool IsCheckingAsset
        {
            get;
            set;
        }

        public OverhaulAssetInfo EditingInfo
        {
            get;
            set;
        }

        public Action CallBack
        {
            get;
            set;
        }

        protected override bool AssignVariablesAutomatically() => false;

        public void Show(OverhaulAssetInfo assetInfo, Action callBack)
        {
            if (!m_HasInitialized)
            {
                OverhaulUIVer2.AssignValues(this);
                OverhaulUIVer2.AssignActionToButton(GetComponent<ModdedObject>(), "BackButton", Hide);
                OverhaulUIVer2.AssignActionToButton(GetComponent<ModdedObject>(), "Done", OnDoneClicked);
                OverhaulUIVer2.AssignActionToButton(GetComponent<ModdedObject>(), "CheckAsset", OnCheckAssetClicked);
                m_CheckAssetStateLabel.text = string.Empty;
                m_HasInitialized = true;
            }

            EditingInfo = assetInfo;
            CallBack = callBack;

            m_AssetBundleField.text = assetInfo.AssetBundle;
            m_AssetField.text = assetInfo.AssetName;
            m_FixMaterialsToggle.isOn = assetInfo.FixMaterials;

            base.gameObject.SetActive(true);
            m_Shading.SetActive(true);
        }

        public void Hide(bool applyChanges)
        {
            if(EditingInfo != null)
            {
                if (applyChanges)
                {
                    EditingInfo.AssetBundle = m_AssetBundleField.text;
                    EditingInfo.AssetName = m_AssetField.text;
                    EditingInfo.FixMaterials = m_FixMaterialsToggle.isOn;
                }
                EditingInfo = null;
            }
            if(CallBack != null)
            {
                CallBack();
                CallBack = null;
            }

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

        public void OnCheckAssetClicked()
        {
            if (IsCheckingAsset)
                return;

            StaticCoroutineRunner.StartStaticCoroutine(checkAssetCoroutine());
        }

        private IEnumerator checkAssetCoroutine()
        {
            IsCheckingAsset = true;
            m_CheckAssetStateLabel.text = "Checking...";
            yield return new WaitForSecondsRealtime(0.2f);
            if(OverhaulAssetsController.TryGetAsset(m_AssetField.text, m_AssetBundleField.text, out UnityEngine.Object asset, m_FixMaterialsToggle.isOn))
            {
                m_CheckAssetStateLabel.text = "Asset found";
            }
            else
            {
                m_CheckAssetButton.interactable = false;
                m_CheckAssetStateLabel.text = "Asset not found!";
                yield return new WaitForSecondsRealtime(1.5f);
                m_CheckAssetStateLabel.text = string.Empty;
                m_CheckAssetButton.interactable = true;
            }
            IsCheckingAsset = false;
            yield break;
        }
    }
}
