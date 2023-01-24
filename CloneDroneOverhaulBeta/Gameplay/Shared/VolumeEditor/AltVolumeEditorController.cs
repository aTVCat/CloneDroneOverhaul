using System.Collections;
using UnityEngine;

namespace CDOverhaul.Shared
{
    /// <summary>
    /// The 2D way of voxel editing
    /// </summary>
    public class AltVolumeEditorController : ModController
    {
        public override void Initialize()
        {
            HasAddedEventListeners = true;
            IsInitialized = true;
        }

        public static Texture2D CreatePalette(int width, int height)
        {
            Color[] colors = CreateColorArray(width, height);

            Texture2D tex = new Texture2D(width, height);
            tex.name = "WeaponPalette";
            tex.SetPixels(colors);

            return tex;
        }

        public static Color[] CreateColorArray(int width, int height)
        {
            Color[] colors = new Color[width * height];
            for (int i = 0; i < colors.Length; i++)
            {
                colors[i] = Color.white;
            }
            return colors;
        }
    }
}