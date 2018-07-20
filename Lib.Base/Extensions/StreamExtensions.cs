using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace Lib.Base
{

    public static class StreamExtensions
    {
        public static TObject ToData<TObject>(this MemoryStream stream) where TObject:class
        {
            using (stream)
            {
                stream.Position = 0;
                var deserializer = new BinaryFormatter();
                var temp = deserializer.Deserialize(stream);
                return temp as TObject;
            }
        }

        public static TObject ToDataAsStruct<TObject>(this MemoryStream stream) where TObject:struct
        {
            using (stream)
            {
                stream.Position = 0;
                var deserializer = new BinaryFormatter();
                var temp = deserializer.Deserialize(stream);
                return (TObject) temp;
            }
        }

        public static MemoryStream ToStream<TObject>(this TObject obj)
        {
            var serializer = new BinaryFormatter();
            var stream = new MemoryStream();
            serializer.Serialize(stream, obj);
            return stream;
        }

        public static TObject DeepClone<TObject>(this TObject obj) where TObject:class
        {
            return obj.ToStream().ToData<TObject>();
        }

        public static Task<int> ReadAllBytesAsync(this Stream stream, out byte[] buffer)
        {
            if (stream == null) throw new ArgumentNullException("stream");
            int initialCapacity = stream.CanSeek ? (int)stream.Length : 0;
            buffer = new byte[initialCapacity];
            return Task<int>.Factory.FromAsync(stream.BeginRead, stream.EndRead, buffer, 0, initialCapacity, null);
        }

        public static byte[] StreamToBytes(this Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}