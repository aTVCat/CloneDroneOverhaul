using System;
using System.Collections;
using UnityEngine;

namespace CDOverhaul.Graphics
{
    public class OverhaulDitheringEffect : OverhaulUI
    {
        private static Texture2D[] m_Textures_HighQuality;
        private static Texture2D[] m_Textures_LowQuality;

        public static bool CanDither => m_GeneratedDithering;
        private static bool m_GeneratedDithering;

        private static IEnumerator generateNoiseTexture(int width, int height, int iterations, Action<Texture2D[]> onComplete)
        {
            yield return new WaitForEndOfFrame();

            Texture2D[] textures = new Texture2D[iterations];
            for(int i = 0; i < iterations; i++)
            {
                Color[] pixels = new Color[width * height];
                Texture2D newTex = new Texture2D(width, height);
                for(int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        pixels[x+y] = Color.white * ((float)m_Random.Next(20, 100) / 100);
                    }
                }
                newTex.SetPixels(pixels);
                newTex.Apply();

                textures[i] = newTex;
            }

            onComplete(textures);
            yield break;
        }

        private static void onGeneratedHighQuality(Texture2D[] array)
        {
            m_Textures_HighQuality = array;
        }
        private static void onGeneratedLowQuality(Texture2D[] array)
        {
            m_Textures_LowQuality = array;
            m_GeneratedDithering = true;
        }

        public override void Initialize()
        {
            if (!OverhaulSessionController.GetKey<bool>("GeneratedDithering"))
            {
                OverhaulSessionController.SetKey("GeneratedDithering", true);

                _ = StaticCoroutineRunner.StartStaticCoroutine(generateNoiseTexture(1600, 900, 3, onGeneratedHighQuality));
                _ = StaticCoroutineRunner.StartStaticCoroutine(generateNoiseTexture(640, 360, 3, onGeneratedLowQuality));
            }
        }
    }
}