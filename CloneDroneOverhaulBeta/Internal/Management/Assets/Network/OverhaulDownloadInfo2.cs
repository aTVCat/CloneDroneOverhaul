using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace CDOverhaul.NetworkAssets
{
    public class OverhaulDownloadInfo : OverhaulDisposable
    {
        public OverhaulDownloadInfo(Action<OverhaulDownloadInfo> doneAction)
        {
            DoneAction = doneAction;
            DownloadResult = new Result();
        }

        public UnityWebRequest WebRequest;
        public float Progress => WebRequest != null ? WebRequest.downloadProgress : 0f;

        public bool Error;
        public string ErrorString;
        public bool DownloadError => Error || DownloadResult == null;

        public Result DownloadResult;
        public Action<OverhaulDownloadInfo> DoneAction;

        public void OnDownloadComplete()
        {
            DoneAction?.Invoke(this);
            Dispose();
        }

        protected override void OnDisposed()
        {
            if (DownloadResult != null && !DownloadResult.IsDisposed)
                DownloadResult.Dispose();

            base.OnDisposed();
        }

        public class Result : OverhaulDisposable
        {
            public string Text;
            public byte[] ByteArray;

            public Texture Texture;
        }
    }
}
