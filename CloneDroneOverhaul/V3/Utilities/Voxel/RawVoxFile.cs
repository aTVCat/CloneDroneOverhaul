using System;
using VoxReader.Interfaces;

namespace CloneDroneOverhaul.V3.Utilities
{
    public class RawVoxFile : IVoxFile
    {
        public int VersionNumber => throw new NotImplementedException();

        public IModel[] Models => throw new NotImplementedException();

        public IPalette Palette => throw new NotImplementedException();

        public IChunk[] Chunks => throw new NotImplementedException();
    }
}
