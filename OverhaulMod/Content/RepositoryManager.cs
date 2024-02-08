using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OverhaulMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;

namespace OverhaulMod.Content
{
    public class RepositoryManager : Singleton<RepositoryManager>
    {
        private const string REPOSITORY_URL = "https://raw.githubusercontent.com/aTVCat/Overhaul-Mod-Content/main/";

        public void GetTextFile(string path, Action<string> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequest.Get(REPOSITORY_URL + path);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, true, delegate (object obj)
            {
                doneCallback?.Invoke((string)obj);
            }, errorCallback, timeOut));
        }

        public void GetFile(string path, Action<byte[]> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequest.Get(REPOSITORY_URL + path);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, false, delegate (object obj)
            {
                doneCallback?.Invoke((byte[])obj);
            }, errorCallback, timeOut));
        }

        public void GetTexture(string link, Action<Texture2D> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequestTexture.GetTexture(REPOSITORY_URL + link);
            _ = ModActionUtils.RunCoroutine(getTextureCoroutine(unityWebRequest, doneCallback, errorCallback, timeOut));
        }

        public void GetCustomTextFile(string link, Action<string> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequest.Get(link);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, true, delegate (object obj)
            {
                doneCallback?.Invoke((string)obj);
            }, errorCallback, timeOut));
        }

        public void GetCustomFile(string link, Action<byte[]> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequest.Get(link);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, false, delegate (object obj)
            {
                doneCallback?.Invoke((byte[])obj);
            }, errorCallback, timeOut));
        }

        public void GetCustomTexture(string link, Action<Texture2D> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest, int timeOut = 20)
        {
            unityWebRequest = UnityWebRequestTexture.GetTexture(link);
            _ = ModActionUtils.RunCoroutine(getTextureCoroutine(unityWebRequest, doneCallback, errorCallback, timeOut));
        }

        public void GetLocalTextFile(string path, Action<string> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest)
        {
            unityWebRequest = UnityWebRequest.Get("file://" + path);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, true, delegate (object obj)
            {
                doneCallback?.Invoke((string)obj);
            }, errorCallback, -1));
        }

        public void GetLocalFile(string path, Action<byte[]> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest)
        {
            unityWebRequest = UnityWebRequest.Get("file://" + path);
            _ = ModActionUtils.RunCoroutine(getFileCoroutine(unityWebRequest, false, delegate (object obj)
            {
                doneCallback?.Invoke((byte[])obj);
            }, errorCallback, -1));
        }

        public void GetLocalTexture(string path, Action<Texture2D> doneCallback, Action<string> errorCallback, out UnityWebRequest unityWebRequest)
        {
            unityWebRequest = UnityWebRequestTexture.GetTexture("file://" + path);
            _ = ModActionUtils.RunCoroutine(getTextureCoroutine(unityWebRequest, doneCallback, errorCallback, -1));
        }

        private IEnumerator getLFSFileCoroutine(UnityWebRequest webRequest, bool returnText, Action<object> doneCallback, Action<string> errorCallback, int timeOut)
        {
            if (timeOut != -1)
                webRequest.timeout = timeOut;

            yield return webRequest.SendWebRequest();

            try
            {
                if (!webRequest.isNetworkError && !webRequest.isHttpError)
                {
                    string lfsData = webRequest.downloadHandler.text;

                    Debug.Log($"LFS DATA: {lfsData}");

                    string version = null;
                    string sha = null;
                    string size = null;

                    string[] lfsDataSplit = lfsData.Split(new char[] { ' ', '\n', '\r' });
                    for (int i = 0; i < lfsDataSplit.Length; i++)
                    {
                        string str = lfsDataSplit[i];
                        Debug.Log(str);

                        if (str == "version")
                            version = lfsDataSplit[i + 1];

                        if (str == "oid")
                            sha = lfsDataSplit[i + 1].Replace("sha256:", string.Empty);

                        if (str == "size")
                            size = lfsDataSplit[i + 1];
                    }

                    Debug.Log(version);
                    Debug.Log(sha);
                    Debug.Log(size);

                    string[] transfersArray = new string[]
                    {
                        "basic"
                    };

                    Dictionary<string, object> objectsArray = new Dictionary<string, object>
                    {
                        { "oid", sha },
                        { "size", int.Parse(size) }
                    };

                    JObject json = new JObject
                    {
                        { "operation", "download" },
                        { "transfers", JToken.FromObject(transfersArray) },
                        //{ "ref", JToken.FromObject(new LFSRef() { name = "refs/heads/main" }) },
                        { "objects", JToken.FromObject(new Dictionary<string, object>[] { objectsArray }) },
                        //{ "hash_algo", "sha256" }
                    };

                    string data = JsonConvert.SerializeObject(json, Formatting.None);

                    Debug.Log($"Data: {data}");

                    /*
                    UnityWebRequest post = UnityWebRequest.Post("https://github.com/aTVCat/CloneDroneOverhaul.git/info/lfs/objects/batch", data);
                    post.SetRequestHeader("Accept", "application/vnd.git-lfs+json");
                    post.SetRequestHeader("Content-Type", "application/json");

                    yield return post.SendWebRequest();

                    Debug.Log("Error: " + post.error);
                    Debug.Log("RCode: " + post.responseCode);

                    Dictionary<string, string> response = post.GetResponseHeaders();
                    if (!response.IsNullOrEmpty())
                        foreach (KeyValuePair<string, string> keyValue in post.GetResponseHeaders())
                        {
                            Debug.Log($"Response: {keyValue.Key}:{keyValue.Value}");
                        }

                    Debug.Log(post.downloadHandler.text);*/

                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("Accept", "application/vnd.git-lfs+json");
                        client.Headers.Add("Content-Type", "application/json");
                        client.Headers.Add("Authorization", "Basic");
                        ModDataManager.Instance.WriteFile("LoadedFile.json", client.UploadString("https://github.com/aTVCat/CloneDroneOverhaul.git/info/lfs/objects/batch", "POST", data), true);
                    }
                }
                else
                {
                    errorCallback?.Invoke(webRequest.error);
                }
            }
            finally
            {
                webRequest.Dispose();
            }
            yield break;
        }

        private IEnumerator getFileCoroutine(UnityWebRequest webRequest, bool returnText, Action<object> doneCallback, Action<string> errorCallback, int timeOut)
        {
            if (timeOut != -1)
                webRequest.timeout = timeOut;

            yield return webRequest.SendWebRequest();

            try
            {
                if (!webRequest.isNetworkError && !webRequest.isHttpError)
                {
                    if (returnText)
                        doneCallback?.Invoke(webRequest.downloadHandler.text);
                    else
                        doneCallback?.Invoke(webRequest.downloadHandler.data);
                }
                else
                {
                    errorCallback?.Invoke(webRequest.error);
                }
            }
            finally
            {
                webRequest.Dispose();
            }
            yield break;
        }

        private IEnumerator getTextureCoroutine(UnityWebRequest webRequest, Action<Texture2D> doneCallback, Action<string> errorCallback, int timeOut)
        {
            if (timeOut != -1)
                webRequest.timeout = timeOut;

            yield return webRequest.SendWebRequest();

            try
            {
                if (!webRequest.isNetworkError && !webRequest.isHttpError)
                {
                    doneCallback?.Invoke((webRequest.downloadHandler as DownloadHandlerTexture).texture);
                }
                else
                {
                    errorCallback?.Invoke(webRequest.error);
                }
            }
            finally
            {
                webRequest.Dispose();
            }
            yield break;
        }

        public struct LFSRef
        {
            public string name;
        }
    }
}
