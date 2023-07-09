using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CDOverhaul.CustomMultiplayer
{
    [Serializable]
    public class OverhaulPacket
    {
        public ulong Target;

        /// <summary>
        /// Called when we receive the packet
        /// </summary>
        public virtual void Handle() { }

        public virtual int GetChannel() => 0;

        public byte[] GetBytes()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                formatter.Serialize(memoryStream, this);
                return memoryStream.ToArray();
            }
        }

        public static OverhaulPacket GetPacket(byte[] array)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(array, 0, array.Length);
                _ = memoryStream.Seek(0, SeekOrigin.Begin);
                BinaryFormatter formatter = new BinaryFormatter();
                return (OverhaulPacket)formatter.Deserialize(memoryStream);
            }
        }
    }
}
