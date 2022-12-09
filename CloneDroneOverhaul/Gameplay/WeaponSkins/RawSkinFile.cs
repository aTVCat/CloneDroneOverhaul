// Decompiled with JetBrains decompiler
// Type: CloneDroneOverhaul.WeaponSkins.RawSkinFile
// Assembly: CloneDroneOverhaulRW, Version=0.2.1.1, Culture=neutral, PublicKeyToken=null
// MVID: 2D597193-58CB-4838-8B24-76A1B530E6FA
// Assembly location: D:\Clone drone stuff\Clone drone overhaul rewrite\CloneDroneOverhaul\CloneDroneOverhaul\bin\Debug\CloneDroneOverhaulRW.dll

using VoxReader.Interfaces;

namespace CloneDroneOverhaul.WeaponSkins
{
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
