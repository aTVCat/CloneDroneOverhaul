using System;
using UnityEngine.Networking;

namespace CDOverhaul.NetworkAssets
{
    public class OverhaulNetworkDownloadHandler
    {
        public UnityWebRequest WebRequest;
        public float DonePercentage => WebRequest == null ? 1f : WebRequest.downloadProgress;

        public bool Error;
        public string ErrorString;

        public Action DoneAction;

        public byte[] DownloadedData;
        public string DownloadedText;
    }
}