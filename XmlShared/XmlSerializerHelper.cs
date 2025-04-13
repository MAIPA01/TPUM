using System.Xml.Serialization;

namespace TPUM.XmlShared
{
    public static class XmlSerializerHelper
    {
        public static string Serialize<T>(T obj)
        {
            using var stringWriter = new StringWriter();
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringWriter, obj);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return stringWriter.ToString();
        }

        public static bool TryDeserialize<T>(string xml, out T result)
        {
            try
            {
                result = Deserialize<T>(xml);
                return true;
            }
            catch
            {
                result = default!;
                return false;
            }
        }

        public static T Deserialize<T>(string xml)
        {
            using var stringReader = new StringReader(xml);
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stringReader)!;
        }
    }
}