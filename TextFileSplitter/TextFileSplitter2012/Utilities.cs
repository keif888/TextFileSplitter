using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace Martin.SQLServer.Dts
{
    public static class Utilities
    {
        public enum typeOfOutputEnum
        {
            ErrorRecords
            , KeyRecords
            , DataRecords
            , PassThrough
            , MasterRecord
            , ChildMasterRecord
            , ChildRecord
        }

        public enum usageOfColumnEnum
        {
              RowType
            , RowData
            , Passthrough
            , Key
            , Ignore
            , MasterValue
        }

        public static string XmlSerializeToString(this object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.Encoding = new UnicodeEncoding(false, false); // little-endian, omit byte order mark
            settings.OmitXmlDeclaration = false;

            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                serializer.Serialize(writer, objectInstance, ns);
            }

            return sb.ToString();
        }

        public static T XmlDeserializeFromString<T>(string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static object XmlDeserializeFromString(string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        public static String XmlDecodeFromString(string encodedString)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<data>");
            sb.Append(encodedString);
            sb.Append("</data>");

            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.ConformanceLevel = ConformanceLevel.Fragment;

            XmlReader xmlSource = XmlReader.Create(new StringReader(sb.ToString()));
            xmlSource.MoveToContent();
            return xmlSource.ReadElementContentAsString();
        }

        public static String XmlEncodeToString(string decodedString)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings writerSettings = new XmlWriterSettings();
            writerSettings.ConformanceLevel = ConformanceLevel.Fragment;
            writerSettings.Indent = false;
            XmlWriter xmlTarget = XmlWriter.Create(sb, writerSettings);
            xmlTarget.WriteString(decodedString);
            xmlTarget.Close();
            return sb.ToString();
        }

        public static DTSValidationStatus CompareValidationValues(DTSValidationStatus oldStatus, DTSValidationStatus newStatus)
        {
            if (oldStatus == DTSValidationStatus.VS_ISVALID && newStatus == DTSValidationStatus.VS_ISVALID)
            {
                return DTSValidationStatus.VS_ISVALID;
            }
            if (oldStatus == DTSValidationStatus.VS_ISCORRUPT || newStatus == DTSValidationStatus.VS_ISCORRUPT)
            {
                return DTSValidationStatus.VS_ISCORRUPT;
            }
            else
            {
                return DTSValidationStatus.VS_ISBROKEN;
            }
        }
    }
}
