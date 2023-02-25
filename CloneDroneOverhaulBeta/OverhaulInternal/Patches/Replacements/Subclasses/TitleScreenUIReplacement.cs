using CDOverhaul.Gameplay;
using CDOverhaul.HUD;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Patches
{
    public class TitleScreenUIReplacement : ReplacementBase
    {
        private Transform _buttonsTransform;
        private Transform _spawnedPanel;

        public override void Replace()
        {
            base.Replace();
            TitleScreenUI target = GameUIRoot.Instance.TitleScreenUI;

            _buttonsTransform = TransformUtils.FindChildRecursive(target.transform, "BottomButtons");
            if (_buttonsTransform == null)
            {
                SuccessfullyPatched = false;
                return;
            }

            GameObject panel = OverhaulMod.Core.HUDController.GetHUDPrefab("TitleScreenUI_Buttons");
            if (panel == null)
            {
                SuccessfullyPatched = false;
                return;
            }
            _spawnedPanel = GameObject.Instantiate(panel, _buttonsTransform).transform;
            _spawnedPanel.SetAsFirstSibling();
            _spawnedPanel.gameObject.SetActive(true);

            ModdedObject moddedObject = _spawnedPanel.GetComponent<ModdedObject>();
            moddedObject.GetObject<Button>(1).onClick.AddListener(OverhaulController.GetController<UISettingsMenu>().Show);

            _buttonsTransform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            _buttonsTransform.localPosition = new Vector3(0, -170f, 0);

            SuccessfullyPatched = true;
        }

        public override void Cancel()
        {
            base.Cancel();
            if (SuccessfullyPatched)
            {
                _buttonsTransform.localScale = Vector3.one;
                _buttonsTransform.localPosition = new Vector3(0, -195.5f, 0);

                if (_spawnedPanel != null)
                {
                    GameObject.Destroy(_spawnedPanel.gameObject);
                }
            }
        }
    }
}
