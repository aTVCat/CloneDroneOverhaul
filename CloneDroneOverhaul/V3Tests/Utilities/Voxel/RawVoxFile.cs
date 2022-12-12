using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxReader;
using VoxReader.Interfaces;

namespace CloneDroneOverhaul.V3Tests.Utilities
{
    public class RawVoxFile : IVoxFile
    {
        public int VersionNumber => throw new NotImplementedException();

        public IModel[] Models => throw new NotImplementedException();

        public IPalette Palette => throw new NotImplementedException();

        public IChunk[] Chunks => throw new NotImplementedException();
    }
}
