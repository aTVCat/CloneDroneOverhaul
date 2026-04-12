using OverhaulMod.Combat;
using OverhaulMod.UI;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorScreenshotManager : Singleton<PersonalizationEditorScreenshotManager>
    {
        public const string CAMERA_ANGLES_FILE = "cessCameraAngles.json"; // customization editor screenshot stage camera angles

        public float TonemapFixSaturationMultiplier = 1.5f;
        public float TonemapFixBrightnessMultiplier = 2f;
        public float TonemapFixAlphaMultiplier = 3.5f;

        private GameObject _stageObject;

        private Transform _holder;

        private GameObject _camerasObject;

        private Camera _blackCamera;

        private Camera _whiteCamera;

        private PersonalizationEditorCamera _screenshotCameraController;

        private PersonalizationEditorScreenshotCameraAnglesInfo _anglesInfo;

        private PersonalizationEditorScreenshotOverlay _screenshotOverlay;

        private Vector3 _defaultCameraPosition, _defaultCameraRotation;

        private bool _isScreenshotting, _stopScreenshotting;

        private void Start()
        {
            readCameraAnglesFromDisk();
        }

        public void ShowOverlay()
        {
            if (!_screenshotOverlay)
            {
                GameObject gameObject = Instantiate(ModResources.Prefab(AssetBundleConstants.UI, "PersonalizationItemScreenshotOverlay"), null, false);
                _screenshotOverlay = gameObject.AddComponent<PersonalizationEditorScreenshotOverlay>();
            }
            _screenshotOverlay.Show();
        }

        public void HideOverlay()
        {
            if (_screenshotOverlay) _screenshotOverlay.Hide();
        }

        public void ShowStage()
        {
            if (_stageObject) _stageObject.SetActive(true);
        }

        public void HideStage()
        {
            StopTakingScreenshots();
            if (_stageObject) _stageObject.SetActive(false);
        }

        public void DestroyStageItem()
        {
            if (_holder.childCount != 0) TransformUtils.DestroyAllChildren(_holder);
        }

        public void ResetCameraPosition()
        {
            _camerasObject.transform.position = _defaultCameraPosition;
            _camerasObject.transform.eulerAngles = _defaultCameraRotation;
        }

        public void AdjustCameraPositionForCurrentItem()
        {
            PersonalizationItemInfo item = PersonalizationEditorManager.Instance.EditingItemInfo;
            if (item == null)
            {
                ResetCameraPosition();
                return;
            }

            PersonalizationEditorScreenshotCameraAngle angle = _anglesInfo.GetAngle(item.Weapon == ModWeaponsManager.SCYTHE_TYPE ? "Scythe" : item.Weapon.ToString());
            angle.ApplyToTransform(_camerasObject.transform);
        }

        private void readCameraAnglesFromDisk()
        {
            string path = Path.Combine(ModCore.DataFolder, CAMERA_ANGLES_FILE);

            PersonalizationEditorScreenshotCameraAnglesInfo info;
            try
            {
                info = ModJsonUtils.DeserializeStream<PersonalizationEditorScreenshotCameraAnglesInfo>(path);
            }
            catch
            {
                info = new PersonalizationEditorScreenshotCameraAnglesInfo();
            }
            info.FixValues();
            _anglesInfo = info;
        }

        private void saveCameraAnglesToDisk()
        {
            string path = Path.Combine(ModCore.DataFolder, CAMERA_ANGLES_FILE);
            ModJsonUtils.WriteStream(path, _anglesInfo);
        }

        public PersonalizationEditorScreenshotCameraAnglesInfo GetCameraAnglesInfo()
        {
            return _anglesInfo;
        }

        public void SaveCameraAnglesInfo()
        {
            saveCameraAnglesToDisk();
        }

        public void SaveAngle()
        {
            ModUIUtils.InputFieldWindow("Type angle name", "bbb", string.Empty, 0, 125f, delegate (string str)
            {
                PersonalizationEditorScreenshotManager manager = PersonalizationEditorScreenshotManager.Instance;

                PersonalizationEditorScreenshotCameraAngle angle = manager.GetCameraAnglesInfo().GetAngle(str);
                if (angle == null)
                {
                    angle = new PersonalizationEditorScreenshotCameraAngle()
                    {
                        Name = str,
                    };
                }
                angle.SetAngleFromTransform(manager.GetCameraController().transform);

                PersonalizationEditorScreenshotCameraAnglesInfo anglesInfo = manager.GetCameraAnglesInfo();
                anglesInfo.Angles.Add(angle);

                manager.SaveCameraAnglesInfo();
            });
        }

        private void instatiateStageIfHavent()
        {
            if (_stageObject) return;

            GameObject stageObject = Instantiate(ModResources.Prefab(AssetBundleConstants.MISCELLANEOUS, "PersonalizationItemScreenshotStage"));
            stageObject.transform.position = Vector3.up * 1000f;

            ModdedObject moddedObject = stageObject.GetComponent<ModdedObject>();
            _holder = moddedObject.GetObject<Transform>(0);
            _camerasObject = moddedObject.GetObject<GameObject>(1);
            _blackCamera = createCamera(_camerasObject.transform, false);
            _whiteCamera = createCamera(_camerasObject.transform, true);
            _whiteCamera.enabled = false;
            _screenshotCameraController = _camerasObject.AddComponent<PersonalizationEditorCamera>();
            _screenshotCameraController.IsScreenshotStageCamera = true;

            _defaultCameraPosition = _camerasObject.transform.position;
            _defaultCameraRotation = _camerasObject.transform.eulerAngles;

            _stageObject = stageObject;
        }

        private Camera createCamera(Transform parent, bool white)
        {
            Camera camera = Instantiate(PlayerCameraManager.Instance.DefaultGameCameraPrefab, parent, false);
            Transform cameraTransform = camera.transform;
            cameraTransform.localPosition = Vector3.zero;
            cameraTransform.localEulerAngles = Vector3.zero;
            cameraTransform.localScale = Vector3.one;

            camera.farClipPlane = 100f;
            camera.fieldOfView = 30f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = white ? Color.white : Color.black;
            camera.allowMSAA = true;
            return camera;
        }

        public PersonalizationEditorCamera GetCameraController()
        {
            return _screenshotCameraController;
        }

        public Camera GetCamera(bool white)
        {
            if (white)
            {
                return _whiteCamera;
            }
            return _blackCamera;
        }

        public void SpawnItemInHolder(PersonalizationItemInfo personalizationItemInfo)
        {
            instatiateStageIfHavent();

            if (_holder.childCount != 0) TransformUtils.DestroyAllChildren(_holder);
            if (personalizationItemInfo != null)
            {
                PersonalizationEditorManager.Instance.EditingItemInfo = personalizationItemInfo;
                personalizationItemInfo.RootObject.Deserialize(_holder, null);
            }
        }

        public void StopTakingScreenshots()
        {
            _stopScreenshotting = _isScreenshotting;
        }

        public bool IsTakingScreenshots()
        {
            return _isScreenshotting;
        }

        public void TakeScreenshotsOfWeaponSkins(bool onlyNew, Action<int, int> callback)
        {
            if (_isScreenshotting) return;

            List<PersonalizationItemInfo> items = PersonalizationManager.Instance.itemList.GetItems(PersonalizationCategory.WeaponSkins);
            takeScreenshotsOfItemsCoroutine(items, onlyNew, callback).Run();
        }

        private IEnumerator takeScreenshotsOfItemsCoroutine(List<PersonalizationItemInfo> items, bool onlyNew, Action<int, int> callback)
        {
            _isScreenshotting = true;

            yield return null;

            for (int i = 0; i < items.Count; i++)
            {
                PersonalizationItemInfo item = items[i];

                if (onlyNew && File.Exists(Path.Combine(item.FolderPath, "preview.png"))) continue;

                UIPersonalizationEditor.instance.Utilities.SetRandomFavoriteColor();

                SpawnItemInHolder(item);
                AdjustCameraPositionForCurrentItem();

                for (int j = 0; j < 30; j++) yield return null;

                if (_stopScreenshotting)
                {
                    _isScreenshotting = false;
                    if (callback != null) callback(items.Count, items.Count);
                    ModUIUtils.MessagePopupOK("Stopped taking screenshots", "not b", true);
                    yield break;
                }

                TakeAndSaveScreenshot();
                if (callback != null) callback(i, items.Count);

                yield return null;
                yield return null;
            }

            _isScreenshotting = false;
            if (callback != null) callback(items.Count, items.Count);

            ModUIUtils.MessagePopupOK("Done!", "bbb", true);

            yield break;
        }

        public void TakeAndSaveScreenshot()
        {
            if (_screenshotOverlay && _screenshotOverlay.isActiveAndEnabled)
            {
                TakeAndSaveScreenshot(false, out Texture2D resultImage);
                _screenshotOverlay.ShowResultImage(resultImage);

                AudioManager.Instance.PlayClipGlobal(AudioLibrary.Instance.DogVoteUpZap, 0f, false, 1.3f);
            }
            else
            {
                TakeAndSaveScreenshot(true, out _);
            }
        }

        public void TakeAndSaveScreenshot(bool destroyTexture, out Texture2D texture)
        {
            int antiAliasingBefore = QualitySettings.antiAliasing;
            QualitySettings.antiAliasing = 8;
            texture = TakeScreenshot(128, 128, 1, false);
            QualitySettings.antiAliasing = antiAliasingBefore;

            string folderPath = PersonalizationEditorManager.Instance.EditingItemFolder;
            string path = Path.Combine(folderPath, "preview.png");
            ModFileUtils.WriteBytes(texture.EncodeToPNG(), path);

            if (destroyTexture)
            {
                DestroyImmediate(texture);
                texture = null;
            }
        }

        public Texture2D TakeScreenshot(int width, int height, int resizeAmount, bool altMethod)
        {
            instatiateStageIfHavent();

            if (altMethod)
            {
                Bloom bloom = _whiteCamera.GetComponent<Bloom>();
                Color bgColorBefore = _whiteCamera.backgroundColor;
                float bloomSpreadBefore = bloom.sepBlurSpread;

                _whiteCamera.backgroundColor = Color.clear;
                bloom.sepBlurSpread = 20f;

                Texture2D texture = takeScreenshotOfCameraView(_whiteCamera, width, height);
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        Color pixel = texture.GetPixel(x, y);
                        pixel.a = 1f;
                        texture.SetPixel(x, y, pixel);
                    }
                }
                texture.Apply();

                _whiteCamera.backgroundColor = bgColorBefore;
                bloom.sepBlurSpread = bloomSpreadBefore;

                return texture;
            }

            Texture2D whiteTexture = takeScreenshotOfCameraView(_whiteCamera, width, height);
            Texture2D blackTexture = takeScreenshotOfCameraView(_blackCamera, width, height);

            Texture2D outputTexture = calculateOutputTexture(whiteTexture, blackTexture, width, height, resizeAmount);

            DestroyImmediate(whiteTexture);
            DestroyImmediate(blackTexture);

            return outputTexture;
        }

        private static Texture2D takeScreenshotOfCameraView(Camera cameraToRender, int width, int height)
        {
            RenderTexture renderTexture = new RenderTexture(width, height, 24, RenderTextureFormat.DefaultHDR);
            cameraToRender.targetTexture = renderTexture;

            cameraToRender.Render();
            RenderTexture.active = renderTexture;

            Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);

            cameraToRender.targetTexture = null;
            RenderTexture.active = null;

            DestroyImmediate(renderTexture);

            return texture2D;
        }

        private Texture2D calculateOutputTexture(Texture2D whiteTexture, Texture2D blackTexture, int width, int height, int resizeAmount)
        {
            Texture2D texture2D = new Texture2D(width / resizeAmount, height / resizeAmount);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelA = whiteTexture.GetPixel(x, y);
                    float pixelAMagnitude = (pixelA.r + pixelA.g + pixelA.b) / 3f;

                    Color pixelB = blackTexture.GetPixel(x, y);
                    float pixelBMagnitude = (pixelB.r + pixelB.g + pixelB.b) / 3f;

                    float difference = 1f - (pixelAMagnitude - pixelBMagnitude);

                    Color color;
                    if (Mathf.Abs(difference) <= 0f)
                    {
                        color = Color.clear;
                    }
                    else
                    {
                        color = blackTexture.GetPixel(x, y) / difference;
                    }
                    color.a = difference;

                    Color finalColor = fixTonemapping(tonemapPixel(color));
                    texture2D.SetPixel(Mathf.FloorToInt(x / (float)resizeAmount), Mathf.FloorToInt(y / (float)resizeAmount), finalColor);
                }
            }
            texture2D.Apply();
            return texture2D;
        }

        private Color tonemapPixel(Color color)
        {
            return new Color(color.r / (color.r + 1f), color.g / (color.g + 1f), color.b / (color.b + 1f), color.a / (color.a + 1f));
        }

        private Color fixTonemapping(Color color)
        {
            HSBColor hsbColor = new HSBColor(color);
            hsbColor.s *= TonemapFixSaturationMultiplier;
            hsbColor.b *= TonemapFixBrightnessMultiplier;
            hsbColor.a *= TonemapFixAlphaMultiplier;
            return hsbColor.ToColor();
        }
    }
}
