using OverhaulAPI;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class PlayerOutfitController : OverhaulGameplayController, IPlayerOutfitController
    {
        [OverhaulSettingAttribute("Player.Outfits.Equipped", "", !OverhaulVersion.IsDebugBuild)]
        public static string EquippedOutfit;

        public const string AccessoryDestroyVFX_ID = "AccessoryDestroyedVFX";
        public static readonly AudioClipDefinition AccessoryDestroyedSound = AudioAPI.CreateDefinitionUsingClip(AssetsController.GetAsset<AudioClip>("BallonExplosion", OverhaulAssetsPart.Sounds));

        public IPlayerOutfitController Interface;
        private static List<IPlayerAccessoryItemDefinition> m_Items;

        public override void Initialize()
        {
            base.Initialize();
            Interface = this;

            if (OverhaulSessionController.GetKey<bool>("HasLoadedAccessories"))
            {
                return;
            }
            OverhaulSessionController.SetKey("HasLoadedAccessories", true);
            m_Items = new List<IPlayerAccessoryItemDefinition>();
            PooledPrefabController.TurnObjectIntoPooledPrefab<RobotAccessoryDestroyVFX>(AssetsController.GetAsset("VFX_AccessoryDestroy", OverhaulAssetsPart.Accessories).transform, 5, AccessoryDestroyVFX_ID);

            AddAccessoryItem("Igrok's hat", MechBodyPartType.Head, "P_Acc_Head_Igrok's hat");
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        public void AddAccessoryItem(string name, MechBodyPartType bodyPart, string assetName)
        {
            GameObject gObject = AssetsController.GetAsset(assetName, OverhaulAssetsPart.Accessories);
            IPlayerAccessoryModel model1 = new PlayerAccessoryModel();
            model1.SetModel(gObject);
            IPlayerAccessoryItemDefinition item1 = Interface.NewAccessoryItem(bodyPart, name, ItemFilter.None);
            item1.SetModel(model1);
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
            {
                return;
            }
            _ = firstPersonMover.gameObject.AddComponent<RobotOutfitWearer>();
        }

        IPlayerAccessoryItemDefinition IPlayerOutfitController.NewAccessoryItem(MechBodyPartType partType, string itemName, ItemFilter filter)
        {
            IPlayerAccessoryItemDefinition item = new PlayerAccessoryItemDefinition();
            item.SetItemName(itemName);
            item.SetBodypartType(partType);
            item.SetFilter(filter);
            (item as PlayerAccessoryItemDefinition).LoadOffsets();
            m_Items.Add(item);
            return item;
        }

        IPlayerAccessoryItemDefinition IPlayerOutfitController.GetAccessoryItem(string skinName, out ItemNullResult error)
        {
            throw new System.NotImplementedException();
        }

        IPlayerAccessoryItemDefinition[] IPlayerOutfitController.GetAccessoryItems(ItemFilter filter)
        {
            List<IPlayerAccessoryItemDefinition> list = new List<IPlayerAccessoryItemDefinition>();
            if(filter == ItemFilter.Equipped)
            {
                foreach(IPlayerAccessoryItemDefinition def in m_Items)
                {
                    if (EquippedOutfit.Contains(def.GetItemName()))
                    {
                        list.Add(def);
                    }
                }
                return list.ToArray();
            }
            return null;
        }

        IPlayerAccessoryItemDefinition[] IPlayerOutfitController.GetAccessoryItems(FirstPersonMover firstPersonMover)
        {
            return Interface.GetAccessoryItems(ItemFilter.Equipped);
        }

        public override string[] Commands()
        {
            throw new System.NotImplementedException();
        }

        public override string OnCommandRan(string[] command)
        {
            throw new System.NotImplementedException();
        }
    }
}