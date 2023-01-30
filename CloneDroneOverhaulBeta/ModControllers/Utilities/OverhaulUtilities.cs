using ModLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

namespace CDOverhaul
{
    public static class OverhaulUtilities
    {
        public static Dictionary<UpgradeType, int> GetUpgradesDictionary(this UpgradeCollection collection)
        {
            return collection.GetPrivateField<Dictionary<UpgradeType, int>>("_upgradeLevels");
        }

        public static class TextureAndMaterialUtils
        {
            /// <summary>
            /// Create sprite from texture
            /// </summary>
            /// <param name="texture2D"></param>
            /// <returns></returns>
            public static Sprite FastSpriteCreate(in Texture2D texture2D)
            {
                if (texture2D == null)
                {
                    return null;
                }
                return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
            }

            /// <summary>
            /// Load a texture from file
            /// </summary>
            /// <param name="path"></param>
            public static Texture2D LoadTexture(in string path)
            {
                Texture2D result = new Texture2D(1, 1);

                byte[] content = FileUtils.LoadBytes(path);
                if (content != null)
                {
                    result.LoadImage(content, false);
                }

                return result;
            }

            /// <summary>
            /// Turn texture into byte array
            /// </summary>
            /// <param name="path"></param>
            public static byte[] TextureToByteArray(in Texture2D tex)
            {
                byte[] result;

                result = tex.EncodeToPNG();

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
                    if (array != null)
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
            /// Get bytes in file
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static byte[] LoadBytes(in string path, in string filename = "")
            {
                byte[] result = null;

                if (File.Exists(path + filename))
                {
                    result = File.ReadAllBytes(path + filename);
                }

                return result;
            }

            /// <summary>
            /// Save
            /// </summary>
            /// <param name="bytes"></param>
            /// <param name="path"></param>
            public static void SaveBytes(in byte[] bytes, in string path, in string filename = "")
            {
                File.WriteAllBytes(path + filename, bytes);
            }

            public static bool FileExists(in string path)
            {
                return File.Exists(path);
            }

            /// <summary>
            /// Async get bytes in file
            /// </summary>
            /// <param name="path"></param>
            /// <param name="onReadEnd"></param>
            public static void LoadBytesAsync(string path, Action<byte[]> onReadEnd, string filename = "")
            {
                if (File.Exists(path + filename))
                {
                    StaticCoroutineRunner.StartStaticCoroutine(loadBytesAsync(path + filename, onReadEnd));
                }
                else
                {
                    onReadEnd(null);
                }
            }

            private static IEnumerator loadBytesAsync(string filePath, Action<byte[]> onLoaded)
            {
                byte[] array = null;
                UnityWebRequest unityWebRequestAsyncOperation = UnityWebRequest.Get("file://" + filePath);
                yield return unityWebRequestAsyncOperation.SendWebRequest();
                array = unityWebRequestAsyncOperation.downloadHandler.data;
                unityWebRequestAsyncOperation.Dispose();
                onLoaded(array);
                yield break;
            }

            /// <summary>
            /// Load all text
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static string LoadString(in string path, in string filename = "")
            {
                string result = null;

                if (File.Exists(path + filename))
                {
                    result = File.ReadAllText(path + filename);
                }

                return result;
            }

            /// <summary>
            /// Async read string in file
            /// </summary>
            /// <param name="path"></param>
            /// <param name="onReadEnd"></param>
            public static void LoadStringAsync(string path, Action<string> onReadEnd, string filename = "")
            {
                if (File.Exists(path + filename))
                {
                    StaticCoroutineRunner.StartStaticCoroutine(loadStringAsync(path + filename, onReadEnd));
                }
                else
                {
                    onReadEnd(null);
                }
            }

            private static IEnumerator loadStringAsync(string filePath, Action<string> onLoaded)
            {
                string data = null;

                UnityWebRequest unityWebRequestAsyncOperation = UnityWebRequest.Get("file://" + filePath);

                yield return unityWebRequestAsyncOperation.SendWebRequest();

                data = unityWebRequestAsyncOperation.downloadHandler.text;
                unityWebRequestAsyncOperation.Dispose();

                onLoaded(data);

                yield break;
            }

            /// <summary>
            /// Creates a folder if one doesn't exist
            /// </summary>
            /// <param name="path"></param>
            public static void TryCreateFolder(in string path)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }

            /// <summary>
            /// Same as method above, but for multiple folders
            /// </summary>
            /// <param name="paths"></param>
            public static void TryCreateFolders(in string[] paths)
            {
                foreach (string path in paths)
                {
                    TryCreateFolder(path);
                }
            }
        }

        public static class ByteSaver
        {
            public static byte[] ObjectToByteArray(object obj)
            {
                bool flag = obj == null;
                byte[] result;
                if (flag)
                {
                    result = null;
                }
                else
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        binaryFormatter.Serialize(memoryStream, obj);
                        result = memoryStream.ToArray();
                    }
                }
                return result;
            }

            public static T FromByteArray<T>(byte[] data)
            {
                bool flag = data == null;
                T result;
                if (flag)
                {
                    result = default(T);
                }
                else
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    using (MemoryStream memoryStream = new MemoryStream(data))
                    {
                        object obj = binaryFormatter.Deserialize(memoryStream);
                        result = (T)obj;
                    }
                }
                return result;
            }
        }
    }
}
