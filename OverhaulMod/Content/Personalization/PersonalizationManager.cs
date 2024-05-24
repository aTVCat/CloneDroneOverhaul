using ICSharpCode.SharpZipLib.Zip;
using OverhaulMod.Engine;
using OverhaulMod.Utils;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace OverhaulMod.Content.Personalization
{
    public class PersonalizationManager : Singleton<PersonalizationManager>
    {
        public const string ITEM_INFO_FILE = "itemInfo.json";

        public static readonly string[] SupportedBodyParts = new string[]
        {
            "Head",
            "Arm"
        };

        public PersonalizationItemList itemList
        {
            get;
            private set;
        }

        public override void Awake()
        {
            base.Awake();

            PersonalizationItemList personalizationItemList = new PersonalizationItemList();
            personalizationItemList.Load();
            itemList = personalizationItemList;
        }

        public void DownloadCustomizationFile(Action<bool> callback, Action<UnityWebRequest> webRequestCallback = null)
        {
            downloadCustomizationFileCoroutine(callback, webRequestCallback).Run();
        }

        private IEnumerator downloadCustomizationFileCoroutine(Action<bool> callback, Action<UnityWebRequest> webRequestCallback = null)
        {
            RepositoryManager.Instance.GetCustomFile($"https://github.com/aTVCat/Overhaul-Mod-Content/raw/main/content/customization.zip", delegate (byte[] bytes)
            {
                try
                {
                    string tempFile = Path.GetTempFileName();
                    ModIOUtils.WriteBytes(bytes, tempFile);

                    FastZip fastZip = new FastZip();
                    fastZip.ExtractZip(tempFile, ModCore.customizationFolder, null);
                }
                catch (Exception exc)
                {
                    Debug.Log(exc);
                    callback?.Invoke(false);
                    return;
                }
                itemList.Load();
                callback?.Invoke(true);
            }, delegate
            {
                callback?.Invoke(false);
            }, out UnityWebRequest unityWebRequest, -1);
            webRequestCallback?.Invoke(unityWebRequest);
            yield break;
        }

        public void ConfigureFirstPersonMover(FirstPersonMover firstPersonMover)
        {
            if (!firstPersonMover || !firstPersonMover.IsAlive())
                return;

            _ = firstPersonMover.gameObject.AddComponent<PersonalizationController>();
        }

        public bool CreateItem(string name, out PersonalizationItemInfo personalizationItem)
        {
            personalizationItem = null;
            if (name.IsNullOrEmpty())
                return false;

            string directoryName = name.Replace(" ", string.Empty);
            string directoryPath = ModCore.customizationFolder + directoryName + "/";
            string filesDirectoryPath = directoryPath + "files/";

            if (Directory.Exists(directoryPath))
                return false;

            if (!Directory.Exists(filesDirectoryPath))
                _ = Directory.CreateDirectory(filesDirectoryPath);

            _ = Directory.CreateDirectory(directoryPath);
            personalizationItem = new PersonalizationItemInfo()
            {
                Name = name,
                Description = "No description provided.",
                IsVerified = false,
                Category = PersonalizationCategory.WeaponSkins,
                EditorID = PersonalizationEditorManager.Instance.editorId,
                ItemID = Guid.NewGuid().ToString(),
                FolderPath = directoryPath
            };
            personalizationItem.FixValues();
            personalizationItem.SetAuthor(SteamFriends.GetPersonaName());
            itemList.Items.Add(personalizationItem);

            ModJsonUtils.WriteStream(directoryPath + ITEM_INFO_FILE, personalizationItem);
            return true;
        }

        public List<ColorPairFloat> GetColorPairsFromString(string dataString)
        {
            if (dataString.IsNullOrEmpty())
                return null;

            string[] split = dataString.Split('|');
            if (split.IsNullOrEmpty())
                return null;

            List<ColorPairFloat> list = new List<ColorPairFloat>();
            foreach (string oldAndNewColorsString in split)
            {
                if (oldAndNewColorsString.IsNullOrEmpty())
                    continue;

                string[] oldAndNewColors = oldAndNewColorsString.Split('-');
                if (oldAndNewColors.Length == 2)
                {
                    Color a = ModParseUtils.TryParseToColor(oldAndNewColors[0], Color.white);
                    Color b = ModParseUtils.TryParseToColor(oldAndNewColors[1], Color.white);
                    list.Add(new ColorPairFloat(a, b));
                }
            }
            return list;
        }

        public string GetStringFromColorPairs(List<ColorPairFloat> colorPairs)
        {
            if (colorPairs.IsNullOrEmpty())
                return null;

            int index = 0;

            StringBuilder stringBuilder = new StringBuilder();
            foreach(var cp in colorPairs)
            {
                string colorA = ColorUtility.ToHtmlStringRGBA(cp.ColorA);
                string colorB = ColorUtility.ToHtmlStringRGBA(cp.ColorB);
                string colorsString = $"{colorA}-{colorB}".Replace("#", string.Empty);
                stringBuilder.Append(colorsString);
                if(index+1 != colorPairs.Count)
                    stringBuilder.Append('|');

                index++;
            }
            return stringBuilder.ToString();
        }

        public static bool IsWeaponCustomizationSupported(WeaponType weaponType)
        {
            return weaponType != WeaponType.None
&& (weaponType == WeaponType.Sword
                || weaponType == WeaponType.Bow
                || weaponType == WeaponType.Hammer
                || weaponType == WeaponType.Spear
                || weaponType == WeaponType.Shield);
        }
    }
}
