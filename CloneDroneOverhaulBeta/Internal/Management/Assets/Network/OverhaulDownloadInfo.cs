using System;
using UnityEngine;
using UnityEngine.Networking;

namespace CDOverhaul.NetworkAssets
{
    public class OverhaulDownloadInfo
    {
        public UnityWebRequest WebRequest;
        public float Progress => WebRequest == null ? 1f : WebRequest.downloadProgress;

        public bool Error;
        public string ErrorString;

        public byte[] DownloadedData;
        public string DownloadedText;
        public Texture DownloadedTexture;

        public Action DoneAction;

        public void OnDownloadComplete()
        {
            DoneAction?.Invoke();
            OverhaulDisposable.AssignNullToAllVars(this);
        }

        public void CancelDownload()
        {
            if (WebRequest != null && !WebRequest.isDone)
                WebRequest.Abort();

            Error = true;
            OnDownloadComplete();
        }
    }
}