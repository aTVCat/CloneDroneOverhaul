using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CDOverhaul.Visuals
{
    public class DitheringUIEffect : OverhaulUIEffectBehaviour
    {
        [OverhaulSettingWithNotification(1)]
        [OverhaulSetting("Graphics.Post effects.Dithering", false)]
        public static bool DitheringEnabled;

        [OverhaulSettingSliderParameters(false, 0.024f, 0.044f)]
        [OverhaulSetting("Graphics.Post effects.Update interval", 0.034f, false, null, "Graphics.Post effects.Dithering")]
        public static float Interval;

        [OverhaulSettingSliderParameters(false, 0.15f, 0.35f)]
        [OverhaulSetting("Graphics.Post effects.Texture opaque", 0.2f, false, null, "Graphics.Post effects.Dithering")]
        public static float Alpha;

        private RawImage m_RawImage;

        private bool m_HasGeneratedTextures;
        private int m_TextureIndex;

        public Texture2D[] NoiseTextures
        {
            get;
            private set;
        }

        public override void Start()
        {
            base.Start();
            m_RawImage = base.GetComponent<RawImage>();
            m_RawImage.enabled = false;

            if (!DitheringEnabled)
                return;

            GenerateNoiseTextures(delegate (Texture2D[] textures)
            {
                m_HasGeneratedTextures = true;
                NoiseTextures = textures;
                StartCoroutine(ditheringLoopCoroutine());
            });
        }

        private IEnumerator ditheringLoopCoroutine()
        {
            while (true)
            {
                if (IsDisposedOrDestroyed())
                    yield break;

                bool render = HasToRenderDithering() && m_HasGeneratedTextures;
                m_RawImage.enabled = render;
                if (render)
                {
                    m_RawImage.texture = NoiseTextures[m_TextureIndex];
                    Color color = m_RawImage.color;
                    color.a = Alpha;
                    m_RawImage.color = color;
                    m_TextureIndex++;
                    if (m_TextureIndex > 2)
                        m_TextureIndex = 0;
                }
                yield return new WaitForSecondsRealtime(Interval);
            }
            yield break;
        }

        protected override void OnDisposed()
        {
            if (m_RawImage)
            {
                m_RawImage.enabled = false;
                m_RawImage = null;
            }
            if (!NoiseTextures.IsNullOrEmpty())
            {
                foreach(Texture2D texture2D in NoiseTextures)
                {
                    Destroy(texture2D);
                }
            }
            NoiseTextures = null;
        }

        public bool HasToRenderDithering()
        {
            OverhaulCameraManager cameraManager = CameraManager;
            if (!cameraManager)
                return false;

            Camera camera = CameraManager.mainCamera;
            return camera /*&& camera.enabled && camera.gameObject.activeInHierarchy */&& CameraManager.mainCamera.name != "UICamera";
        }

        public void GenerateNoiseTextures(Action<Texture2D[]> callback)
        {
            StartCoroutine(generateNoiseTexturesCoroutine(callback));
        }

        private IEnumerator generateNoiseTexturesCoroutine(Action<Texture2D[]> callback)
        {
            Texture2D[] result = new Texture2D[3];
            yield return StaticCoroutineRunner.StartStaticCoroutine(GeneratePerlinNoiseCoroutine(1280, 720, delegate (Texture2D texture)
            {
                result[0] = texture;
            }));
            yield return StaticCoroutineRunner.StartStaticCoroutine(GeneratePerlinNoiseCoroutine(1280, 720, delegate (Texture2D texture)
            {
                result[1] = texture;
            }));
            yield return StaticCoroutineRunner.StartStaticCoroutine(GeneratePerlinNoiseCoroutine(1280, 720, delegate (Texture2D texture)
            {
                result[2] = texture;
            }));
            callback?.Invoke(result);
            yield break;
        }

        public static IEnumerator GeneratePerlinNoiseCoroutine(int width, int height, Action<Texture2D> callback)
        {
            Texture2D texture2D = new Texture2D(width, height);
            Color[] array = new Color[width * height];
            float num = 0f;
            float num2 = 0f;
            for (float num3 = 0f; num3 < texture2D.height; num3++)
            {
                for (float num4 = 0f; num4 < texture2D.width; num4++)
                {
                    float x = num + (num4 / texture2D.width);
                    float y = num2 + (num3 / texture2D.height);
                    float num5 = Mathf.PerlinNoise(x, y) + UnityEngine.Random.Range(-0.2f, 0.4f);
                    array[((int)num3 * texture2D.width) + (int)num4] = new Color(num5, num5, num5, 0.1f);

                    if (num3 + num4 % 100 == 0)
                        yield return null;
                }
            }
            for (int i = 0; i < array.Length; i++)
            {
                float num6 = UnityEngine.Random.Range(-0.2f, 0.4f);
                array[i] = new Color(array[i].r + num6, array[i].g + num6, array[i].b + num6, 0.1f);
            }
            texture2D.SetPixels(array);
            texture2D.Apply();
            callback?.Invoke(texture2D);
            yield break;
        }
    }
}
