using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DotNetCasClient.Utils
{
    /// <summary>
    /// A utility class for serializing and deserializing objects.
    /// </summary>
    public sealed class SerializationUtil
    {
        /// <summary>
        /// Serializes an object to a byte array.
        /// </summary>
        /// <param name="obj">Object to serialize into a byte array.</param>
        /// <returns></returns>
        public static byte[] Serialize(Object obj)
        {
            if (obj == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes a byte array into an object.
        /// </summary>
        /// <param name="byteArray">Byte array representing a serialized object.</param>
        /// <returns></returns>
        public static Object Deserialize(byte[] byteArray)
        {
            if (byteArray == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                ms.Write(byteArray, 0, byteArray.Length);
                ms.Seek(0, SeekOrigin.Begin);
                Object obj = formatter.Deserialize(ms);
                return obj;
            }
        }
    }
}
