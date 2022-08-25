using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace PSW.ITT.Common
{
    public static class XMLParser
    {
        public static T DeserializeXMLToObject<T>(string xmlString)
        {
            T returnObject = default(T);
            if (string.IsNullOrEmpty(xmlString)) return default(T);

            try
            {
                using (TextReader reader = new StringReader(xmlString))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    returnObject = (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex);
            }
            return returnObject;
        }


        public static string SerializeObjectToXML<T>(this T serialisableObject)
        {
            var xmlSerializer = new XmlSerializer(serialisableObject.GetType());
            string xmlString = null;
            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var xw = XmlWriter.Create(ms,
                        new XmlWriterSettings()
                        {
                            Encoding = new UTF8Encoding(true),
                            Indent = false,
                            NewLineOnAttributes = false,
                        }))
                    {
                        xmlSerializer.Serialize(xw, serialisableObject);
                        xmlString = Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to parse xml: {0}", ex);
            }
            return xmlString;
        }
    }
}