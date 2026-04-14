using OverhaulMod.Content;
using OverhaulMod.Utils;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace OverhaulMod.UI
{
    public class UIElementPlayerInfoDisplay : OverhaulUIBehaviour
    {
        [UIElement("PlayerIcon")]
        public RawImage _icon;

        private bool _showIcon;

        private UnityWebRequest _webRequest;

        private Texture2D _texture;

        protected override void OnInitialized()
        {
            _icon.color = new Color(1f, 1f, 1f, 0f);
            _icon.enabled = false;
        }

        public override void Update()
        {
            Color color = _icon.color;
            color.a = Mathf.Lerp(color.a, _showIcon ? 1f : 0f, Time.unscaledDeltaTime * 12.5f);
            _icon.color = color;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            UnityWebRequest unityWebRequest = _webRequest;
            if (unityWebRequest != null)
            {
                try
                {
                    unityWebRequest.Abort();
                }
                catch { }
            }

            Texture2D texture = _texture;
            if (texture)
            {
                Destroy(texture);
            }
        }

        public void LoadRobotHead(int characterModelIndex, int favouriteColorIndex)
        {
            if (!base.gameObject.activeInHierarchy || !base.enabled)
                return;

            string contentPath = AddonManager.Instance.GetAddonPath(AddonManager.EXTRAS_ADDON_ID);
            if (contentPath.IsNullOrEmpty())
                return;

            if (characterModelIndex < 0 || characterModelIndex >= MultiplayerCharacterCustomizationManager.Instance.CharacterModels.Count)
                characterModelIndex = 0;

            string filename = $"{MultiplayerCharacterCustomizationManager.Instance.CharacterModels[characterModelIndex].Name}_{favouriteColorIndex}.png".Replace("Bow 2", "Bow 1");
            if (filename.Contains("Business Bot")) filename = "Business Bot_0.png";
            if (filename.Contains("Emperor")) filename = "Emperor_0.png";
            if (filename.Contains("Sword 5")) filename = "Sword 5_0.png";

            string path = Path.Combine(contentPath, "playerIcons", filename);
            if (!File.Exists(path))
                return;

            _ = base.StartCoroutine(loadIconCoroutine(path));
        }

        private IEnumerator loadIconCoroutine(string path)
        {
            using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture($"file://{path}"))
            {
                _webRequest = unityWebRequest;
                yield return unityWebRequest.SendWebRequest();
                _webRequest = null;
                if (!unityWebRequest.isHttpError && !unityWebRequest.isNetworkError && unityWebRequest.isDone)
                {
                    Texture2D texture = (unityWebRequest.downloadHandler as DownloadHandlerTexture).texture;
                    _texture = texture;
                    _icon.texture = texture;
                    _icon.enabled = true;
                    _showIcon = true;
                }
            }
            yield break;
        }
    }
}
