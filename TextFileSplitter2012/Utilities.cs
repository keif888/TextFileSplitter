using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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
            , RowsProcessed
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

        //public static string XmlSerializeToString(this object objectInstance)
        //{
        //    var serializer = new XmlSerializer(objectInstance.GetType());
        //    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        //    ns.Add("", "");
        //    var sb = new StringBuilder();
        //    XmlWriterSettings settings = new XmlWriterSettings();
        //    settings.Indent = false;
        //    settings.Encoding = new UnicodeEncoding(false, false); // little-endian, omit byte order mark
        //    settings.OmitXmlDeclaration = false;

        //    using (XmlWriter writer = XmlWriter.Create(sb, settings))
        //    {
        //        serializer.Serialize(writer, objectInstance, ns);
        //    }

        //    return sb.ToString();
        //}

        //public static T XmlDeserializeFromString<T>(string objectData)
        //{
        //    return (T)XmlDeserializeFromString(objectData, typeof(T));
        //}

        //public static object XmlDeserializeFromString(string objectData, Type type)
        //{
        //    var serializer = new XmlSerializer(type);
        //    object result;

        //    using (TextReader reader = new StringReader(objectData))
        //    {
        //        result = serializer.Deserialize(reader);
        //    }

        //    return result;
        //}

        //public static String XmlDecodeFromString(string encodedString)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append("<data>");
        //    sb.Append(encodedString);
        //    sb.Append("</data>");

        //    XmlReaderSettings readerSettings = new XmlReaderSettings();
        //    readerSettings.ConformanceLevel = ConformanceLevel.Fragment;

        //    XmlReader xmlSource = XmlReader.Create(new StringReader(sb.ToString()));
        //    xmlSource.MoveToContent();
        //    return xmlSource.ReadElementContentAsString();
        //}

        //public static String XmlEncodeToString(string decodedString)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    XmlWriterSettings writerSettings = new XmlWriterSettings();
        //    writerSettings.ConformanceLevel = ConformanceLevel.Fragment;
        //    writerSettings.Indent = false;
        //    XmlWriter xmlTarget = XmlWriter.Create(sb, writerSettings);
        //    xmlTarget.WriteString(decodedString);
        //    xmlTarget.Close();
        //    return sb.ToString();
        //}

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
            if (oldStatus == DTSValidationStatus.VS_ISBROKEN || newStatus == DTSValidationStatus.VS_ISBROKEN)
            {
                return DTSValidationStatus.VS_ISBROKEN;
            }
            else
            {
                return DTSValidationStatus.VS_NEEDSNEWMETADATA;
            }
        }

        public static String ReplaceEscapes(String stringToCleanse)
        {
            String workString = stringToCleanse.Replace("\\", @"\");
            return workString.Replace("\n", @"\n").Replace("\a", @"\a").Replace("\b", @"\b").Replace("\f", @"\f").Replace("\r", @"\r").Replace("\t", @"\t").Replace("\v", @"\v").Replace("\'", @"\'");
        }

        public static String DynamicClassStringFromOutput(SSISOutput output, Boolean firstRowColumnNames, String rowTerminator, String columnDelimiter)
        {
            String classString = string.Empty;
            classString += "[DelimitedRecord(\"" + ReplaceEscapes(columnDelimiter) + "\")]\r\n";
            if (firstRowColumnNames)
            {
                classString += "[IgnoreFirst(1)]\r\n";
            }
            classString += "public sealed class " + output.Name + "\r\n";
            classString += "{\r\n";
            for (int i = 0; i < output.OutputColumnCollection.Count; i++)
            {
                SSISOutputColumn outputColumn = output.OutputColumnCollection[i];
                if (!outputColumn.IsDerived)
                {
                    String conversionString = (String)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.dotNetFormatString);
                    Boolean isOptional = (Boolean)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.isColumnOptional);
                    if ((i + 1 == output.OutputColumnCollection.Count) && (!String.IsNullOrEmpty(rowTerminator)))
                    {
                        classString += "[FieldDelimiterAttribute(\"" + ReplaceEscapes(rowTerminator) + "\")]\r\n";
                    }
                    if (isOptional)
                    {
                        classString += "[FieldOptional()]\r\n";
                    }
                    if ((Boolean)ManageProperties.GetPropertyValue(outputColumn.CustomPropertyCollection, ManageProperties.nullResultOnConversionError))
                    {
                        classString += "[FieldNullOnError()]\r\n";
                    }
                    switch (outputColumn.SSISDataType)
                    {
                        case DataType.DT_BOOL:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.Boolean, \"" + conversionString + "\")]\r\n";
                            classString += "public Boolean? ";
                            break;
                        case DataType.DT_DATE:
                        case DataType.DT_DBDATE:
                        case DataType.DT_DBTIME:
                        case DataType.DT_DBTIME2:
                        case DataType.DT_DBTIMESTAMP:
                        case DataType.DT_DBTIMESTAMP2:
                        case DataType.DT_DBTIMESTAMPOFFSET:
                        case DataType.DT_FILETIME:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.Date, \"" + conversionString + "\")]\r\n";
                            classString += "public DateTime? ";
                            break;
                        case DataType.DT_CY:
                        case DataType.DT_DECIMAL:
                        case DataType.DT_NUMERIC:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.Decimal, \"" + conversionString + "\")]\r\n";
                            classString += "public Decimal? ";
                            break;
                        case DataType.DT_I1:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.Byte, \"" + conversionString + "\")]\r\n";
                            classString += "public Byte? ";
                            break;
                        case DataType.DT_I2:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.Int16, \"" + conversionString + "\")]\r\n";
                            classString += "public Int16? ";
                            break;
                        case DataType.DT_I4:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.Int32, \"" + conversionString + "\")]\r\n";
                            classString += "public Int32? ";
                            break;
                        case DataType.DT_I8:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.Int64, \"" + conversionString + "\")]\r\n";
                            classString += "public Int64? ";
                            break;
                        case DataType.DT_R4:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.Single, \"" + conversionString + "\")]\r\n";
                            classString += "public Single? ";
                            break;
                        case DataType.DT_R8:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.Dpuble, \"" + conversionString + "\")]\r\n";
                            classString += "public Double? ";
                            break;
                        case DataType.DT_UI1:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.SByte, \"" + conversionString + "\")]\r\n";
                            classString += "public SByte? ";
                            break;
                        case DataType.DT_UI2:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.UInt16, \"" + conversionString + "\")]\r\n";
                            classString += "public UInt16? ";
                            break;
                        case DataType.DT_UI4:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.UInt32, \"" + conversionString + "\")]\r\n";
                            classString += "public UInt32? ";
                            break;
                        case DataType.DT_UI8:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.UInt64, \"" + conversionString + "\")]\r\n";
                            classString += "public UInt64? ";
                            break;
                        case DataType.DT_GUID:
                            if (!String.IsNullOrEmpty(conversionString))
                                classString += "[FieldConverter(ConverterKind.Guid, \"" + conversionString + "\")]\r\n";
                            classString += "public Guid? ";
                            break;
                        case DataType.DT_STR:
                        case DataType.DT_TEXT:
                        case DataType.DT_NTEXT:
                        case DataType.DT_WSTR:
                            classString += "public String ";
                            break;
                        default:
                            classString += "public String ";
                            break;
                    }
                    classString += outputColumn.Name + ";\r\n";
                }
            }
            classString += "}\r\n";
            return classString;
        }
    }
}
