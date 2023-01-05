using System;
using System.Collections;
using UnityEngine;

namespace CloneDroneOverhaul.Utilities
{
    public static class Coroutines
    {
        public static void LoadWorkshopImage(string url, string id, Action<Sprite> action)
        {
            StaticCoroutineRunner.StartStaticCoroutine(loadWorkshopImage(url, id, action));
        }
        public static void WaitForCharacterInitAndCall(Character character, Action callback)
        {
            StaticCoroutineRunner.StartStaticCoroutine(waitForCharacterInitAndCall(character, callback));
        }

        private static IEnumerator waitForCharacterInitAndCall(Character character, Action callback)
        {
            yield return new WaitUntil(() => character.IsInitialized() == true);
            callback();
            yield break;
        }
        private static IEnumerator loadWorkshopImage(string url, string id, Action<Sprite> action)
        {
            /*
            if (V3Tests.Base.ModDataController.HasTempAsset(id + ".png"))
            {
                V3Tests.Base.ModDataController.LoadTempAssetAsync<Texture2D>(id + ".png", V3Tests.Base.ModAssetFileType.Image, delegate (Texture2D tex)
                {
                    Sprite sprite1 = Sprite.Create(tex, new Rect(0f, 0f, (float)tex.width, (float)tex.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
                    action(sprite1);
                });
                yield break;
            }*/

            WWW www = new WWW(url);
            yield return www;
            bool flag = !string.IsNullOrEmpty(www.error);
            if (flag)
            {
                action(null);
                yield break;
            }
            Texture2D texture2D = new Texture2D(32, 32, TextureFormat.ARGB32, false);
            www.LoadImageIntoTexture(texture2D);
            texture2D.filterMode = FilterMode.Point;
            texture2D.Apply();
            Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);

            /*OverhaulCacheAndGarbageController.SaveImageOnDisk(texture2D, id + ".png");*/

            //OverhaulCacheManager.AddTemporalObject<Sprite>(sprite, UI.NewWorkshopBrowserUI.TEMPORAL_PREFIX + "Image_" + url); //Memory issues

            action(sprite);

            yield break;
        }

        public static void LerpImageColor(UnityEngine.UI.Image img, Color targetColor, float multipler, Action delegateOnEnd = null)
        {
            StaticCoroutineRunner.StartStaticCoroutine(lerpImageColor(img, targetColor, multipler, delegateOnEnd));
        }

        private static IEnumerator lerpImageColor(UnityEngine.UI.Image img, Color targetColor, float duration, Action delegateOnEnd)
        {
            float elapsed = 0.0f;
            Color colStart = img.color;

            while (elapsed < duration)
            {
                Color col = img.color;
                col.r = Mathf.Lerp(colStart.r, targetColor.r, elapsed / duration);
                col.g = Mathf.Lerp(colStart.g, targetColor.g, elapsed / duration);
                col.b = Mathf.Lerp(colStart.b, targetColor.b, elapsed / duration);
                col.a = Mathf.Lerp(colStart.a, targetColor.a, elapsed / duration);
                img.color = col;
                elapsed += Time.deltaTime;

                yield return null;
            }
            img.color = targetColor;
            if (delegateOnEnd != null)
            {
                delegateOnEnd();
            }
            yield break;
        }

        public static IEnumerator SpawnCurrentLevel_EndlessOverMode(bool isAsync = false, string overrideLevelID = null, Action completeCallback = null)
        {
            V3Tests.Gameplay.LevelConstructor.BuildALevel(new V3Tests.Gameplay.LevelConstructor.LevelSettings(), true);
            yield break;
        }
    }
}
