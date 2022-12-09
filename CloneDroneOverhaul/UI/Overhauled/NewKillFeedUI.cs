using CloneDroneOverhaul.UI.Components;
using CloneDroneOverhaul.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI
{
    public class NewKillFeedUI : ModGUIBase
    {
        private RectTransform rt;
        private ModdedObject prefab;
        private List<GameObject> objects = new List<GameObject>();
        private List<KillEventData> events = new List<KillEventData>();

        public static NewKillFeedUI Instance;
        private float timeToCheckNextEvent = 0.7f;

        public override void OnInstanceStart()
        {
            base.MyModdedObject = base.GetComponent<ModdedObject>();
            rt = MyModdedObject.GetObjectFromList<RectTransform>(0);
            prefab = MyModdedObject.GetObjectFromList<ModdedObject>(1);
            prefab.gameObject.SetActive(false);
            Instance = this;
        }

        public void Show()
        {
            base.gameObject.SetActive(true);
        }
        public void Hide()
        {
            base.gameObject.SetActive(false);
        }

        public override void OnFixedUpdate()
        {
            if (objects.Count > 0)
            {
                foreach (GameObject obj in objects)
                {
                    obj.transform.localPosition += new Vector3(0, 0.75f, 0);
                }
            }

            if (Time.unscaledTime >= timeToCheckNextEvent && events.Count > 0)
            {
                timeToCheckNextEvent = Time.unscaledTime + 0.7f;
                checkEvent(events[events.Count - 1]);
                events.RemoveAt(events.Count - 1);
            }
        }

        public override void RunFunction<T>(string name, T obj)
        {
            if (name == "Bolt.OnEvent" && typeof(T) == typeof(MultiplayerKillEvent))
            {
                MultiplayerKillEvent evnt = obj as MultiplayerKillEvent;
                KillEventData data = new KillEventData();
                data.KillerID = evnt.KillerPlayFabID;
                data.KillerName = evnt.KillerName;
                data.VictimName = evnt.VictimName;
                data.DamageType = evnt.DamageType;
                data.WasKicked = evnt.WasKicked;
                events.Add(data);
            }
        }

        private void checkEvent(KillEventData evnt)
        {
            if (evnt == null || string.IsNullOrEmpty(evnt.VictimName))
            {
                return;
            }
            ModdedObject obj1 = Instantiate<ModdedObject>(prefab, rt, true);
            obj1.GetComponent<Animator>().Play("KillFeedUI_Item");

            if (!string.IsNullOrEmpty(evnt.KillerName))
            {
                obj1.GetObjectFromList<Text>(0).text = evnt.KillerName;
            }
            else
            {
                obj1.GetObjectFromList<Text>(0).gameObject.SetActive(false);
            }

            obj1.GetObjectFromList<Text>(2).text = evnt.VictimName;
            obj1.GetObjectFromList<RectTransform>(3).gameObject.SetActive(evnt.WasKicked);

            DamageSourceType type = (DamageSourceType)evnt.DamageType;

            if (type != DamageSourceType.Sword && type != DamageSourceType.Arrow && type != DamageSourceType.Hammer && type != DamageSourceType.Spear)
            {
                obj1.GetObjectFromList<Image>(1).sprite = OverhaulCacheManager.GetCached<Sprite>("KillMethodType_" + type.ToString());
            }
            else
            {
                if (!string.IsNullOrEmpty(evnt.KillerID))
                {
                    bool isFire = false;
                    Character charr = CharacterTracker.Instance.TryGetLivingCharacterWithPlayFabID(evnt.KillerID);
                    if (charr != null)
                    {
                        RobotShortInformation rInfo = charr.GetRobotInfo();
                        if (rInfo.UpgradeCollection != null && rInfo.UpgradeCollection is PlayerUpgradeCollection)
                        {
                            PlayerUpgradeCollection coll = rInfo.UpgradeCollection as PlayerUpgradeCollection;
                            switch (type)
                            {
                                case DamageSourceType.Sword:
                                    isFire = coll.HasUpgrade(UpgradeType.FireSword);
                                    break;
                                case DamageSourceType.Hammer:
                                    isFire = coll.HasUpgrade(UpgradeType.FireHammer);
                                    break;
                                case DamageSourceType.Spear:
                                    isFire = coll.HasUpgrade(UpgradeType.FireSpear);
                                    break;
                                case DamageSourceType.Arrow:
                                    isFire = coll.HasUpgrade(UpgradeType.FireArrow);
                                    break;
                            }

                            obj1.GetObjectFromList<Image>(1).sprite = OverhaulCacheManager.GetCached<Sprite>("KillMethodType_" + (isFire ? "Fire" : "") + type.ToString());
                        }
                    }
                }
            }

            obj1.gameObject.SetActive(true);
            obj1.gameObject.AddComponent<DestroyAfterWaitWithCallback>().SetUp(6, removeObj);

            objects.Add(obj1.gameObject);
        }

        private void removeObj(GameObject gObj)
        {
            if (objects.Contains(gObj))
            {
                objects.Remove(gObj);
            }
        }

        public class KillEventData
        {
            public string VictimName;
            public string KillerName;
            public string KillerID;
            public int DamageType;
            public bool WasKicked;
        }
    }
}
