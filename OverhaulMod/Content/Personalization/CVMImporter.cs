using PicaVoxel;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OverhaulMod.Content.Personalization
{
    public class CVMImporter
    {
        // code by X606
        public enum WeaponSpecialType
        {
            EMPHammer,
            FireHammer,
            FireSword,
            GreatSword,
            GreatFireSword
        }

        // code by X606
        public class SaveClass
        {
            public EnemyType Base;

            public Dictionary<PicaVoxelPoint, Voxel>[] ModifiedVoxels;

            public Dictionary<WeaponSpecialType, Dictionary<PicaVoxelPoint, Voxel>> CustomSpecialWeaponModels;

            public void LoadData(byte[] data)
            {
                int num = 0;
                Base = (EnemyType)BitConverter.ToInt32(ReadData(data, 4, ref num), 0);
                int num2 = BitConverter.ToInt32(ReadData(data, 4, ref num), 0);
                ModifiedVoxels = new Dictionary<PicaVoxelPoint, Voxel>[num2];
                for (int i = 0; i < num2; i++)
                {
                    ModifiedVoxels[i] = new Dictionary<PicaVoxelPoint, Voxel>();
                    int num3 = BitConverter.ToInt32(ReadData(data, 4, ref num), 0);
                    for (int j = 0; j < num3; j++)
                    {
                        KeyValuePair<PicaVoxelPoint, Voxel> keyValuePair = ReadVoxelInfoFromList(data, ref num);
                        ModifiedVoxels[i].Add(keyValuePair.Key, keyValuePair.Value);
                    }
                }
                CustomSpecialWeaponModels = new Dictionary<WeaponSpecialType, Dictionary<PicaVoxelPoint, Voxel>>();
                bool flag = num >= data.Length;
                if (!flag)
                {
                    int num4 = BitConverter.ToInt32(ReadData(data, 4, ref num), 0);
                    for (int k = 0; k < num4; k++)
                    {
                        WeaponSpecialType key = (WeaponSpecialType)BitConverter.ToInt32(ReadData(data, 4, ref num), 0);
                        int num5 = BitConverter.ToInt32(ReadData(data, 4, ref num), 0);
                        Dictionary<PicaVoxelPoint, Voxel> dictionary = new Dictionary<PicaVoxelPoint, Voxel>();
                        for (int l = 0; l < num5; l++)
                        {
                            KeyValuePair<PicaVoxelPoint, Voxel> keyValuePair2 = ReadVoxelInfoFromList(data, ref num);
                            dictionary.Add(keyValuePair2.Key, keyValuePair2.Value);
                        }
                        CustomSpecialWeaponModels.Add(key, dictionary);
                    }
                }
            }

            private byte[] ReadData(byte[] data, int length, ref int index)
            {
                byte[] array = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    array[i] = data[index + i];
                }
                index += length;
                return array;
            }

            private KeyValuePair<PicaVoxelPoint, Voxel> ReadVoxelInfoFromList(byte[] data, ref int index)
            {
                PicaVoxelPoint picaVoxelPoint = new PicaVoxelPoint(0, 0, 0)
                {
                    X = BitConverter.ToInt32(ReadData(data, 4, ref index), 0),
                    Y = BitConverter.ToInt32(ReadData(data, 4, ref index), 0),
                    Z = BitConverter.ToInt32(ReadData(data, 4, ref index), 0)
                };
                Voxel value = default;
                byte[] array = ReadData(data, 4, ref index);
                value.Color = new Color32(array[0], array[1], array[2], array[3]);
                value.State = (VoxelState)ReadData(data, 1, ref index)[0];
                value.Value = ReadData(data, 1, ref index)[0];
                return new KeyValuePair<PicaVoxelPoint, Voxel>(picaVoxelPoint, value);
            }
        }
    }
}
