using System.Runtime.Serialization.Formatters.Binary;

namespace BlackHole.Wormhole
{
    internal class WormholeService
    {
        internal void StoreInsert(object entityType)
        {
            byte[] data;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                if(entityType != null)
                {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                    bf.Serialize(ms, entityType);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                    data = ms.ToArray();
                }
            }
        }

        internal void ReadStoredCommand(byte[] data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                object obj = bf.Deserialize(ms);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
            var files = Directory.EnumerateFiles("");

            if (files.Any())
            {
                foreach(var file in files)
                {

                }
            }
        }
    }
}
