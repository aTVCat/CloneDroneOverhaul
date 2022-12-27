using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading;

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
            /// Create sprite from texture
            /// </summary>
            /// <param name="texture2D"></param>
            /// <returns></returns>
            public static Sprite FastSpriteCreate(in Texture2D texture2D)
            {
                return Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect);
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

        public static class CameraUtils
        {
            private static Dictionary<Camera, int> _cameraCullingMasks = new Dictionary<Camera, int>();
            private static List<AudioListener> _audioListeners = new List<AudioListener>();

            private const int DONT_RENDER_CULLING_MASK = 0;

            private static int _lastTargetFrame = -1;
            private static bool _renderingGame = true;
            private static bool _audioEnabled = true;

            public static void SetRendererEnabled(bool value, bool loweOrHighFramerate)
            {
                if (!value && _renderingGame)
                {
                    Camera[] _cameras = Modules.GameInformationManager.UnoptimizedThings.GetFPSLoweringStuff().AllCameras;
                    _cameraCullingMasks.Clear();
                    foreach (Camera cam in _cameras)
                    {
                        if (cam != null)
                        {
                            _cameraCullingMasks.Add(cam, cam.cullingMask);
                            cam.cullingMask = DONT_RENDER_CULLING_MASK;
                        }
                    }

                    if (loweOrHighFramerate)
                    {
                        SetTargetFramerate(30);
                    }
                }
                else if (value && !_renderingGame)
                {
                    foreach (Camera cam in _cameraCullingMasks.Keys)
                    {
                        cam.cullingMask = _cameraCullingMasks[cam];
                    }

                    if (loweOrHighFramerate)
                    {
                        SetTargetFramerate(_lastTargetFrame);
                    }
                }

                _renderingGame = value;
            }

            public static void SetAudioListenersEnabled(bool value)
            {
                if (_audioEnabled && !value)
                {
                    _audioListeners.Clear();
                    AudioListener[] listeners = UnityEngine.Object.FindObjectsOfType<AudioListener>();
                    foreach (AudioListener listener in listeners)
                    {
                        _audioListeners.Add(listener);
                        listener.enabled = value;
                    }
                }
                else if (!_audioEnabled && value)
                {
                    foreach (AudioListener listener in _audioListeners)
                    {
                        if (listener != null)
                        {
                            listener.enabled = value;
                        }
                    }
                }
                _audioEnabled = value;
            }

            public static void SetTargetFramerate(int value)
            {
                Application.targetFrameRate = value;
            }
        }

        public static class FileUtils
        {
            /// <summary>
            /// Get bytes in file
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static byte[] LoadBytes(string path)
            {
                byte[] result = null;

                if (File.Exists(path))
                {
                    ThreadStart start = delegate
                    {
                        result = File.ReadAllBytes(path);
                    };
                    Thread thread = new Thread(start);
                    thread.Start();
                }

                return result;
            }

            /// <summary>
            /// Async get bytes in file
            /// </summary>
            /// <param name="path"></param>
            /// <param name="onReadEnd"></param>
            public static void LoadBytesAsync(string path, Action<byte[]> onReadEnd)
            {
                if (File.Exists(path))
                {
                    ThreadStart start = delegate
                    {
                        StaticCoroutineRunner.StartStaticCoroutine(loadBytesAsync(path, onReadEnd));
                    };
                    Thread thread = new Thread(start);
                    thread.Start();
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
            public static string LoadString(in string path)
            {
                string result = null;

                if (File.Exists(path))
                {
                    result = System.Text.Encoding.UTF8.GetString(LoadBytes(path));
                }

                return result;
            }

            /// <summary>
            /// Async read string in file
            /// </summary>
            /// <param name="path"></param>
            /// <param name="onReadEnd"></param>
            public static void LoadStringAsync(string path, Action<string> onReadEnd)
            {
                if (File.Exists(path))
                {
                    ThreadStart start = delegate
                    {
                        StaticCoroutineRunner.StartStaticCoroutine(loadStringAsync(path, onReadEnd));
                    };
                    Thread thread = new Thread(start);
                    thread.Start();
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
    }
}
