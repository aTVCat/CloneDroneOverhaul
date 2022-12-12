using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace CloneDroneOverhaul.V3Tests.Utilities
{
    public static class OverhaulUtilities
    {
        public static class TextureAndMaterialUtils
        {
            /// <summary>
            /// Set material texture just without a need to enter texture name
            /// </summary>
            /// <param name="material"></param>
            /// <param name="textureType"></param>
            /// <param name="texture"></param>
            public static void SetMaterialTexture(in Material material, in MaterialTextureType textureType, in Texture texture)
            {
                throw new NotImplementedException();
            }

            /// <summary>
            /// Load a texture from file
            /// </summary>
            /// <param name="path"></param>
            public static Texture2D LoadTexture(in string path)
            {
                Texture2D result = new Texture2D(1,1);

                byte[] content = FileUtils.LoadBytes(path);
                if(content != null)
                {
                    result.LoadImage(content, false);
                }

                return result;
            }

            /// <summary>
            /// Async load texture from file
            /// </summary>
            /// <param name="path"></param>
            /// <param name="onLoaded"></param>
            public static void LoadTextureAsync(string path, Action<Texture2D> onLoaded)
            {
                Texture2D result = new Texture2D(1, 1);
                FileUtils.LoadBytesAsync(path, delegate (byte[] array)
                {
                    if(array != null)
                    {
                        result.LoadImage(array);
                        result.Apply();
                        onLoaded(result);
                    }
                    else
                    {
                        onLoaded(null);
                    }
                });
            }
        }

        public static class FileUtils
        {
            /// <summary>
            /// Load bytes from file
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static byte[] LoadBytes(in string path)
            {
                byte[] result = null;

                if (File.Exists(path))
                {
                    result = File.ReadAllBytes(path);
                }

                return result;
            }

            /// <summary>
            /// Async load bytes from file
            /// </summary>
            /// <param name="path"></param>
            /// <param name="onReadEnd"></param>
            public static void LoadBytesAsync(string path, Action<byte[]> onReadEnd)
            {
                if (File.Exists(path))
                {
                    StaticCoroutineRunner.StartStaticCoroutine(loadBytesAsync(path, onReadEnd));
                }
                else
                {
                    onReadEnd(null);
                }
            }
            static IEnumerator loadBytesAsync(string filePath, Action<byte[]> onLoaded)
            {
                byte[] array = null;

                UnityWebRequest unityWebRequestAsyncOperation = UnityWebRequest.Get("file://" + filePath);

                yield return unityWebRequestAsyncOperation.SendWebRequest();

                array = unityWebRequestAsyncOperation.downloadHandler.data;
                unityWebRequestAsyncOperation.Dispose();

                onLoaded(array);

                yield break;
            }
        }
    }
}
