using OverhaulMod.Utils;
using System;
using System.Collections;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationEditorScreenshotManager : Singleton<PersonalizationEditorScreenshotManager>
    {
        private GameObject m_stageObject;

        private Transform m_holder;

        private GameObject m_camerasObject;

        private Camera m_blackCamera;

        private Camera m_whiteCamera;

        private PersonalizationEditorCamera m_screenshotCameraController;

        private void instatiateStageIfHavent()
        {
            if (m_stageObject) return;

            GameObject stageObject = Instantiate(ModResources.Prefab(AssetBundleConstants.MISCELLANEOUS, "PersonalizationItemScreenshotStage"));
            stageObject.transform.position = Vector3.up * 1000f;

            ModdedObject moddedObject = stageObject.GetComponent<ModdedObject>();
            m_holder = moddedObject.GetObject<Transform>(0);
            m_blackCamera = moddedObject.GetObject<Camera>(1);
            m_whiteCamera = moddedObject.GetObject<Camera>(2);
            m_camerasObject = moddedObject.GetObject<GameObject>(3);
            m_screenshotCameraController = m_camerasObject.AddComponent<PersonalizationEditorCamera>();
            m_screenshotCameraController.IsScreenshotStageCamera = true;

            m_stageObject = stageObject;
        }

        public GameObject GetStage()
        {
            instatiateStageIfHavent();
            return m_stageObject;
        }

        public PersonalizationEditorCamera GetCameraController()
        {
            return m_screenshotCameraController;
        }

        public void SpawnItemInHolder(PersonalizationItemInfo personalizationItemInfo)
        {
            instatiateStageIfHavent();

            if (m_holder.childCount != 0)
                TransformUtils.DestroyAllChildren(m_holder);

            personalizationItemInfo.RootObject.Deserialize(m_holder, null);
        }

        public Texture2D TakeScreenshotOfObject(int width, int height)
        {
            instatiateStageIfHavent();

            Texture2D whiteTexture = takeScreenshotOfCameraView(m_whiteCamera, width, height);
            Texture2D blackTexture = takeScreenshotOfCameraView(m_blackCamera, width, height);

            Texture2D outputTexture = calculateOutputTexture(whiteTexture, blackTexture, width, height);

            DestroyImmediate(whiteTexture);
            DestroyImmediate(blackTexture);

            return outputTexture;
        }

        private static Texture2D takeScreenshotOfCameraView(Camera cameraToRender, int width, int height)
        {
            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            cameraToRender.targetTexture = renderTexture;

            cameraToRender.Render();
            RenderTexture.active = renderTexture;

            Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
            texture2D.ReadPixels(new Rect(0f, 0f, width, height), 0, 0);

            cameraToRender.targetTexture = null;
            RenderTexture.active = null;

            DestroyImmediate(renderTexture);

            return texture2D;
        }

        private Texture2D calculateOutputTexture(Texture2D whiteTexture, Texture2D blackTexture, int width, int height)
        {
            Texture2D texture2D = new Texture2D(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixelA = whiteTexture.GetPixel(x, y);
                    float pixelAMagnitude = pixelA.r + pixelA.g + pixelA.b;

                    Color pixelB = blackTexture.GetPixel(x, y);
                    float pixelBMagnitude = pixelB.r + pixelB.g + pixelB.b;

                    float difference = pixelAMagnitude - pixelBMagnitude;
                    difference = 1f - difference;

                    Color color;
                    if (Mathf.Abs(difference) < 0.01f)
                    {
                        color = Color.clear;
                    }
                    else
                    {
                        color = blackTexture.GetPixel(x, y) / difference;
                    }
                    color.a = difference;

                    texture2D.SetPixel(x, y, color);
                }
            }
            texture2D.Apply();
            return texture2D;
        }
    }
}
