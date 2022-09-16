using CloneDroneOverhaul.PooledPrefabs;
using CloneDroneOverhaul.Utilities;
using ModLibrary;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.ImageEffects;
using System.Collections.Generic;
using System.IO;

namespace CloneDroneOverhaul.Modules
{
    public class ModDataManager : ModuleBase
    {
        public string Mod_Folder
        {
            get
            {
                return Application.persistentDataPath + "/CloneDroneOverhaul";
            }
        }
        public string Addons_Folder
        {
            get
            {
                return Mod_Folder + "/Addons";
            }
        }

        public override bool ShouldWork()
        {
            return true;
        }

        public override void OnActivated()
        {
            checkFolders();
        }

        private void checkFolders()
        {
            if (!Directory.Exists(Mod_Folder))
            {
                Directory.CreateDirectory(Mod_Folder);
                OverhaulMain.Timer.AddActionToCompleteNextFrame(showMessageAboutFolderCreation, new object[] { Mod_Folder });
            }
            if (!Directory.Exists(Addons_Folder))
            {
                Directory.CreateDirectory(Addons_Folder);
                OverhaulMain.Timer.AddActionToCompleteNextFrame(showMessageAboutFolderCreation, new object[] { Addons_Folder });
            }
        }

        private void showMessageAboutFolderCreation(object[] args)
        {
            CloneDroneOverhaul.UI.Notifications.Notification notif = new UI.Notifications.Notification();
            notif.SetUp("New folder created", "Created: " + (string)args[0], 20, Vector2.zero, Color.clear, new UI.Notifications.Notification.NotificationButton[] { new UI.Notifications.Notification.NotificationButton { Action = new UnityEngine.Events.UnityAction(notif.HideThis), Text = "OK" } });
        }
    }
}
