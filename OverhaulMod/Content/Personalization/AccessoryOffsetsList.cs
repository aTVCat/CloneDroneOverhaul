using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class AccessoryOffsetsList
    {
        public List<AccessoryOffset> Offsets; // the index of offset corresponds to character model index. hopefully doborog doesnt change the order of the skins

        public void InitializeList()
        {
            if (Offsets == null) Offsets = new List<AccessoryOffset>();

            int characterModelCount = MultiplayerCharacterCustomizationManager.Instance.CharacterModels.Count;
            while (Offsets.Count < characterModelCount)
            {
                AccessoryOffset offset = new AccessoryOffset();
                offset.InitializeTransformArrays();
                offset.SetScale(Vector3.one);

                Offsets.Add(offset);
            }
        }
    }
}
