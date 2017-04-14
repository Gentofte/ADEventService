using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;

namespace GK.AD.DTO
{
    // ================================================================================
    public class Serializer
    {
        // -----------------------------------------------------------------------------
        public static bool IsEqual(byte[] source, byte[] target)
        {
            if (source == null || target == null || source.LongLength != target.LongLength)
                return false;

            for (long i = 0; i < source.LongLength; i++)
            {
                if (source[i] != target[i])
                    return false;
            }

            return true;
        }

        // -----------------------------------------------------------------------------
        public static byte[] SerializeToByteArray<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        // -----------------------------------------------------------------------------
        public static T DeSerializeFromByteArray<T>(byte[] ba)
        {
            using (MemoryStream ms = new MemoryStream(ba))
            {
                BinaryFormatter bf = new BinaryFormatter();
                return (T)bf.Deserialize(ms);
            }
        }

        // -----------------------------------------------------------------------------
        public static string SerializeToXML<T>(T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        // -----------------------------------------------------------------------------
        public static T DeserializeFromXML<T>(string xml)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(xml);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;

                DataContractSerializer deserializer = new DataContractSerializer(typeof(T));
                return (T) deserializer.ReadObject(stream);
            }
        }

        // -----------------------------------------------------------------------------
        public static string SerializeToJson<T>(T obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            using (StreamReader reader = new StreamReader(memoryStream))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(memoryStream, obj);
                memoryStream.Position = 0;
                return reader.ReadToEnd();
            }
        }

        // -----------------------------------------------------------------------------
        public static T DeserializeFromJson<T>(string json)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
                stream.Write(data, 0, data.Length);
                stream.Position = 0;

                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(T));
                return (T)deserializer.ReadObject(stream);
            }
        }
    }
}
