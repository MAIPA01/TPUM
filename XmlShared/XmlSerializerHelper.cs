using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace TPUM.XmlShared
{
    public static class XmlSerializerHelper
    {
        private static XmlReaderSettings? _xmlSettings = null;

        private static void InitXmlReaderSettings()
        {
            if (_xmlSettings != null) return;
            _xmlSettings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema
            };
            try
            {
                _xmlSettings.Schemas.Add(null, Path.Combine(AppContext.BaseDirectory, "Schema", "schema0.xsd"));
                _xmlSettings.Schemas.Add(null, Path.Combine(AppContext.BaseDirectory, "Schema", "schema1.xsd"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            _xmlSettings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            _xmlSettings.ValidationEventHandler += (sender, e) =>
            {
                throw new Exception($"Walidacja: {e.Severity} – {e.Message}");
            };
        }

        public static string Serialize<T>(T obj)
        {
            InitXmlReaderSettings();
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
            InitXmlReaderSettings();
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
            InitXmlReaderSettings();
            using var stringReader = new StringReader(xml);
            using (var reader = XmlReader.Create(new StringReader(xml), _xmlSettings))
            {
                while (reader.Read()) { }
            }
            var serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stringReader)!;
        }
    }
}