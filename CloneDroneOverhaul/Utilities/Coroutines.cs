using UnityEngine;
using System.Collections;
using System;

namespace CloneDroneOverhaul.Utilities
{
    public static class Coroutines
    {
		public static void LoadWorkshopImage(string url, Action<Sprite> action)
		{
			StaticCoroutineRunner.StartStaticCoroutine(loadWorkshopImage(url, action));
		}

		private static IEnumerator loadWorkshopImage(string url, Action<Sprite> action)
		{
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
			Sprite sprite = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f));
			action(sprite);
			yield break;
		}
	}
}
