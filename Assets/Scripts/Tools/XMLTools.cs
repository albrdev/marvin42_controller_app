using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.IO;

namespace Assets.Scripts.Tools
{
    public static class XMLTools
    {
        public static Encoding DefaultEncoding { get; } = new UTF8Encoding(false); // false = Omit encoding identifier character (BOM)
        public static char DefaultIndentCharacter { get; } = ' ';
        public static int DefaultIndentCount { get; } = 2;
        public static XmlWriterSettings Settings { get; } = new XmlWriterSettings()
        {
            //Async = false,
            //CheckCharacters = true,
            //CloseOutput = false,
            //ConformanceLevel = ConformanceLevel.Document,
            Encoding = DefaultEncoding,
            Indent = true,
            IndentChars = new string(DefaultIndentCharacter, DefaultIndentCount),
            //NamespaceHandling = NamespaceHandling.Default,
            NewLineChars = "\n",
            //NewLineHandling = NewLineHandling.Replace,
            //NewLineOnAttributes = false,
            //OmitXmlDeclaration = false,
            //WriteEndDocumentOnClose = true,
        };

        public static readonly XmlSerializerNamespaces EmptyNamespace = new XmlSerializerNamespaces(new[] { new XmlQualifiedName() });

        public static string GetXMLTypeName<T>()
        {
            if(!System.Attribute.IsDefined(typeof(T), typeof(XmlTypeAttribute)))
                throw new System.ArgumentException(string.Format("XML type is not defined: \'{0}\'", typeof(T).Name));

            return ((XmlTypeAttribute)System.Attribute.GetCustomAttribute(typeof(T), typeof(XmlTypeAttribute))).TypeName;
        }

        private static readonly XDeclaration k_UTF8EncodingDeclaration = new XDeclaration("1.0", Encoding.UTF8.BodyName, null);
        private static readonly XAttribute k_XMLXSD = new XAttribute(XNamespace.Xmlns + "xsd", XmlSchema.Namespace);
        private static readonly XAttribute k_XMLXSI = new XAttribute(XNamespace.Xmlns + "xsi", XmlSchema.InstanceNamespace);

        public static void AppendConventionalNamespaces(XElement content)
        {
            content.Add(k_XMLXSD);
            content.Add(k_XMLXSI);
        }

        public static XDocument GenerateXDocumentTemplate()
        {
            return new XDocument(k_UTF8EncodingDeclaration);
        }
    }
}
