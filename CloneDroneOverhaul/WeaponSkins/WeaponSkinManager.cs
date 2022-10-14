using CloneDroneOverhaul.Modules;
using ModLibrary;
using UnityEngine;
using System;
using VoxReader;
using VoxReader.Interfaces;

namespace CloneDroneOverhaul.WeaponSkins
{
    public class WeaponSkinManager : ModuleBase
    {
		IVoxFile file;
        public override bool IsEnabled()
        {
            return true;
        }

        public override void OnActivated()
        {
            file = VoxReader.VoxReader.Read(OverhaulMain.Instance.ModInfo.FolderPath + "WeaponSkinPacks/TheDarkPast/bow.vox");
        }
    }

	internal class RawSkinFile : IVoxFile
	{

		public int VersionNumber { get; }

		public IModel[] Models { get; }

		public IPalette Palette { get; }

		public IChunk[] Chunks { get; }

		internal RawSkinFile(int versionNumber, IModel[] models, IPalette palette, IChunk[] chunks)
		{
			this.VersionNumber = versionNumber;
			this.Models = models;
			this.Palette = palette;
			this.Chunks = chunks;
		}
	}
}
