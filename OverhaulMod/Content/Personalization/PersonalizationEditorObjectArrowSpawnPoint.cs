using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorObjectArrowSpawnPoint : PersonalizationEditorObjectComponentBase
    {
        private static readonly Vector3 s_targetVector = Vector3.one;

        private GameObject _preview;

        private Transform _transform;

        private void Start()
        {
            _transform = base.transform;

            if (!PersonalizationEditorManager.IsInEditor()) return;

            GameObject previewModel = Instantiate(ModResources.Prefab(AssetBundleConstants.MODELS, "ArrowSpawnPoint"), base.transform, false);
            _preview = previewModel;

            Transform previewTransform = previewModel.transform;
            previewTransform.localPosition = new Vector3(-0.013f, 0.013f, -1.25f);
            previewTransform.localEulerAngles = new Vector3(-180f, 0f, 0f);
            previewTransform.localScale = Vector3.one * 0.4f;
        }

        private void Update()
        {
            if (_transform.localScale != s_targetVector) _transform.localScale = s_targetVector;
        }

        private void OnDisable()
        {
            if (!PersonalizationEditorManager.IsInEditor()) return;

            GameObject gm = base.gameObject;
            ModActionUtils.DoInFrame(delegate
            {
                if (gm) gm.SetActive(true);
            });
        }
    }
}