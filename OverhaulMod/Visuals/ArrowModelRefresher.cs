using OverhaulMod.Engine;
using OverhaulMod.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace OverhaulMod.Visuals
{
    public class ArrowModelRefresher : ModBehaviour
    {
        [ModSetting(ModSettingsConstants.ENABLE_ARROW_REWORK, true)]
        public static bool EnableArrowRework;

        private ArrowProjectile _arrowProjectile;

        private Transform _normalVisualsTransform;
        private GameObject[] _normalVisuals;

        private Transform _fireVisualsTransform;
        private GameObject[] _fireVisuals;

        private Transform _newNormalModelTransform;
        private Transform _newFireModelTransform;

        private bool _hasStarted;

        public override void Start()
        {
            _arrowProjectile = base.GetComponent<ArrowProjectile>();

            Transform normalVisualsObject = TransformUtils.FindChildRecursive(base.transform, "NormalVisuals");
            if (normalVisualsObject && normalVisualsObject.childCount != 0)
            {
                GameObject[] gameObjects = new GameObject[normalVisualsObject.childCount];
                for (int i = 0; i < normalVisualsObject.childCount; i++)
                {
                    gameObjects[i] = normalVisualsObject.GetChild(i).gameObject;
                }
                _normalVisuals = gameObjects;
                _normalVisualsTransform = normalVisualsObject;
            }
            else
                _normalVisuals = Array.Empty<GameObject>();

            Transform fireVisualsObject = TransformUtils.FindChildRecursive(base.transform, "FlamingVisuals");
            if (fireVisualsObject && fireVisualsObject.childCount != 0)
            {
                GameObject[] gameObjects = new GameObject[fireVisualsObject.childCount];
                for (int i = 0; i < fireVisualsObject.childCount; i++)
                {
                    gameObjects[i] = fireVisualsObject.GetChild(i).gameObject;
                }
                _fireVisuals = gameObjects;
                _fireVisualsTransform = fireVisualsObject;
            }
            else
                _fireVisuals = Array.Empty<GameObject>();

            InstantiateNewModels();
            _hasStarted = true;
        }

        public override void OnEnable()
        {
            if (!ModCore.IsActive())
            {
                Destroy(this);
                return;
            }
            _ = ModActionUtils.RunCoroutine(waitThenRefreshAllVisuals());
        }

        public void RefreshAllVisuals()
        {
            if (!_arrowProjectile || !_hasStarted)
                return;

            bool featureEnabled = ModCore.IsActive() && EnableArrowRework;
            if (!featureEnabled)
            {
                SetDefaultVisuals(true);
                return;
            }
            SetDefaultVisuals(false);

            Transform newNormalModelTransform = _newNormalModelTransform;
            Transform newFireModelTransform = _newFireModelTransform;

            Transform[] transforms = _arrowProjectile.BladeScaleTransforms;
            if (transforms != null && transforms.Length != 0)
            {
                Transform transform = transforms[0];
                if (transform && newNormalModelTransform)
                {
                    Vector3 vector = newNormalModelTransform.localScale;
                    vector.x = Mathf.Max(0.4f, transform.localScale.x / 4f);

                    newNormalModelTransform.localScale = vector;
                    if (newFireModelTransform)
                        newFireModelTransform.localScale = vector;
                }
            }
        }

        public void SetDefaultVisuals(bool visible)
        {
            if (_fireVisuals != null)
            {
                foreach (GameObject gameObject in _fireVisuals)
                    gameObject.SetActive(visible);
            }

            if (_normalVisuals != null)
            {
                foreach (GameObject gameObject in _normalVisuals)
                    gameObject.SetActive(visible);
            }

            if (_newNormalModelTransform)
                _newNormalModelTransform.gameObject.SetActive(!visible);

            if (_newFireModelTransform)
                _newFireModelTransform.gameObject.SetActive(!visible);
        }

        public void InstantiateNewModels()
        {
            bool featureEnabled = ModCore.IsActive();
            if (!featureEnabled)
                return;

            if (!_newNormalModelTransform)
            {
                Transform arrowModel = Instantiate(ModResources.Prefab(AssetBundleConstants.MODELS, "OverhaulVRArrowModel")).transform;
                arrowModel.SetParent(_normalVisualsTransform);
                arrowModel.localPosition = new Vector3(0.025f, -0.025f, -0.6f);
                arrowModel.localEulerAngles = new Vector3(0f, 180f, 0f);
                arrowModel.localScale = Vector3.one * 0.4f;
                MeshRenderer meshRenderer = arrowModel.GetComponent<MeshRenderer>();
                if (meshRenderer)
                {
                    meshRenderer.material.shader = Shader.Find("Standard");
                    meshRenderer.material.SetColor("_EmissionColor", new Color(0.7f, 1.5f, 3f) * 2f);
                }
                _newNormalModelTransform = arrowModel;
            }

            if (!_newFireModelTransform)
            {
                Transform arrowModel = Instantiate(ModResources.Prefab(AssetBundleConstants.MODELS, "OverhaulVRArrowModel")).transform;
                arrowModel.SetParent(_fireVisualsTransform);
                arrowModel.localPosition = new Vector3(0.025f, -0.025f, -0.6f);
                arrowModel.localEulerAngles = new Vector3(0f, 180f, 0f);
                arrowModel.localScale = Vector3.one * 0.4f;
                MeshRenderer meshRenderer = arrowModel.GetComponent<MeshRenderer>();
                if (meshRenderer)
                {
                    meshRenderer.material.shader = Shader.Find("Standard");
                    meshRenderer.material.SetColor("_EmissionColor", WeaponManager.Instance.FireSpearModelPrefab.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor"));
                }

                Transform ogVfx = TransformUtils.FindChildRecursive(_fireVisualsTransform, "FireVFX");
                if (ogVfx)
                {
                    Transform newVfx = Instantiate(ogVfx, arrowModel);
                    newVfx.localPosition = new Vector3(0.05f, 0f, -1.4f);
                    newVfx.localEulerAngles = new Vector3(270f, 180f, 0f);
                    newVfx.localScale = Vector3.one * 0.03f;
                }

                _newFireModelTransform = arrowModel;
            }
        }

        private IEnumerator waitThenRefreshAllVisuals()
        {
            if (!_hasStarted)
            {
                yield return null;
            }
            yield return null;
            RefreshAllVisuals();
            yield break;
        }
    }
}
