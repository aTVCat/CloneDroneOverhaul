using OverhaulAPI;
using System.Collections.Generic;
using UnityEngine;

namespace CDOverhaul.Gameplay
{
    public class PlayerOutfitController : OverhaulGameplayController, IPlayerOutfitController
    {
        public const string AccessoryDestroyVFX_ID = "AccessoryDestroyedVFX";
        public static readonly AudioClipDefinition AccessoryDestroyedSound = AudioAPI.CreateDefinitionUsingClip(AssetController.GetAsset<AudioClip>("BallonExplosion", OverhaulAssetsPart.Sounds));

        public IPlayerOutfitController Interface;

        private PlayerOutfitData m_PlayerData;
        private static List<IPlayerAccessoryItemDefinition> m_Items;

        public override void Initialize()
        {
            base.Initialize();
            Interface = this;
            m_PlayerData = PlayerOutfitData.GetData<PlayerOutfitData>(PlayerOutfitData.Filename);


            if (OverhaulSessionController.GetKey<bool>("HasLoadedAccessories"))
            {
                return;
            }
            OverhaulSessionController.SetKey("HasLoadedAccessories", true);
            m_Items = new List<IPlayerAccessoryItemDefinition>();
            PooledPrefabController.TurnObjectIntoPooledPrefab<RobotAccessoryDestroyVFX>(AssetController.GetAsset("VFX_AccessoryDestroy", OverhaulAssetsPart.Accessories).transform, 5, AccessoryDestroyVFX_ID);

            // Igrok's Hat
            IPlayerAccessoryItemDefinition item1 = Interface.NewAccessoryItem(MechBodyPartType.Head, "Igrok's Hat", ItemFilter.None);
            IPlayerAccessoryModel model1 = new PlayerAccessoryModel();
            model1.SetModel(AssetController.GetAsset("P_Acc_Head_Igrok's hat", OverhaulAssetsPart.Accessories));
            item1.SetModel(model1);
        }

        protected override void OnDisposed()
        {
            base.OnDisposed();
        }

        public override void OnFirstPersonMoverSpawned(FirstPersonMover firstPersonMover, bool hasInitializedModel)
        {
            if (!hasInitializedModel)
            {
                return;
            }
            _ = firstPersonMover.gameObject.AddComponent<RobotOutfitWearerExpansion>();
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
                    if (m_PlayerData.EquipedAccessories.Contains(def.GetItemName()))
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