using OverhaulMod.Utils;
using UnityEngine;

namespace OverhaulMod.Content.Personalization.Objects
{
    public class PersonalizationEditorArrowSpawnPoint : PersonalizationEditorComponent
    {
        private static readonly Vector3 s_scale = Vector3.one;

        private Transform _transform;

        private void Start()
        {
            _transform = base.transform;

            if (!PersonalizationEditorManager.IsInEditorMode() || PersonalizationEditorManager.Instance.IsInScreenshotMode()) return;

            GameObject previewModel = Instantiate(ModResources.Prefab(ModAssetBundles.MODELS, "ArrowSpawnPoint"), base.transform, false);
            Transform previewTransform = previewModel.transform;
            previewTransform.localPosition = new Vector3(-0.013f, 0.013f, -1.25f);
            previewTransform.localEulerAngles = new Vector3(-180f, 0f, 0f);
            previewTransform.localScale = Vector3.one * 0.4f;
        }

        private void Update()
        {
            if (_transform.localScale != s_scale) _transform.localScale = s_scale;
        }

        private void OnDisable()
        {
            if (!PersonalizationEditorManager.IsInEditorMode()) return;

            GameObject gm = base.gameObject;
            ModActionUtils.DoInFrame(delegate
            {
                if (gm) gm.SetActive(true);
            });
        }

        public override string GetDisplayName() => "Arrow spawn point settings";
    }
}