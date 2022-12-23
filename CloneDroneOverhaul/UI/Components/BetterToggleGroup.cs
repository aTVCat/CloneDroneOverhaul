using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CloneDroneOverhaul.UI.Components
{
    public static class AddAndConfigBetterToggleGroupClass
    {
        public static BetterToggleGroup AddAndConfigBetterToggleGroup(this GameObject obj, bool initialValue)
        {
            return obj.AddComponent<BetterToggleGroup>().SetUpGroup(initialValue);
        }
    }

    public class BetterToggleGroup : MonoBehaviour
    {
        public Toggle MainToggle;
        public List<BetterToggleGroupEntry> Toggles;
        public bool IsOnlyChangingMainToggleState;

        public BetterToggleGroup SetUpGroup(bool initialMainToggleValue)
        {
            Toggles = new List<BetterToggleGroupEntry>();
            MainToggle = base.GetComponent<Toggle>();
            MainToggle.isOn = initialMainToggleValue;
            MainToggle.onValueChanged.AddListener(OnValueChanged);
            return this;
        }

        public void OnValueChanged(bool val)
        {
            if (IsOnlyChangingMainToggleState)
            {
                return;
            }
            IsOnlyChangingMainToggleState = true;
            foreach (BetterToggleGroupEntry togg in Toggles)
            {
                togg.OnValueChanged(val);
            }
            IsOnlyChangingMainToggleState = false;
        }

        public void OnEntryValueUpdated(BetterToggleGroupEntry togg, bool value)
        {
            IsOnlyChangingMainToggleState = true;
            MainToggle.isOn = value;
            IsOnlyChangingMainToggleState = false;
        }

        public void AddNewEntry(BetterToggleGroupEntry togg)
        {
            OnEntryValueUpdated(togg, togg.MyToggle.isOn);
            Toggles.Add(togg);
        }

        private void OnDestroy()
        {
            foreach (BetterToggleGroupEntry togg in Toggles)
            {
                Destroy(togg);
            }
        }
    }

    public class BetterToggleGroupEntry : MonoBehaviour
    {
        public Toggle MyToggle;
        public BetterToggleGroup Group;

        public BetterToggleGroupEntry SetUp(BetterToggleGroup group, Toggle toggle, bool initialVal)
        {
            MyToggle = toggle;
            MyToggle.isOn = initialVal;
            MyToggle.onValueChanged.AddListener(OnValueChanged);
            Group = group;
            group.AddNewEntry(this);
            return this;
        }
        public void OnValueChanged(bool val)
        {
            MyToggle.isOn = val;
            if (Group.IsOnlyChangingMainToggleState)
            {
                return;
            }
            Group.OnEntryValueUpdated(this, val);
        }

        private void OnDestroy()
        {
            if (Group.Toggles.Contains(this))
            {
                Group.Toggles.Remove(this);
            }
        }
    }
}