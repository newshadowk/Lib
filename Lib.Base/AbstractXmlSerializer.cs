/*
 * Author beefsteak
 * Email  beefsteak@live.cn
 */

using System;
using System.Xml.Serialization;

namespace Lib.Base
{
    public class AbstractXmlSerializer<TAbstractType> : IXmlSerializable
    {
        public static implicit operator TAbstractType(AbstractXmlSerializer<TAbstractType> o)
        {
            return o.Data;
        }

        public static implicit operator AbstractXmlSerializer<TAbstractType>(TAbstractType o)
        {
            return o == null ? null : new AbstractXmlSerializer<TAbstractType>(o);
        }

        private TAbstractType _data;
        /// <summary>
        /// [Concrete] Data to be stored/is stored as XML.
        /// </summary>
        public TAbstractType Data
        {
            get { return _data; }
            set { _data = value; }
        }

        /// <summary>
        /// **DO NOT USE** This is only added to enable XML Serialization.
        /// </summary>
        /// <remarks>DO NOT USE THIS CONSTRUCTOR</remarks>
        public AbstractXmlSerializer()
        {
            // Default Ctor (Required for Xml Serialization - DO NOT USE)
        }

        /// <summary>
        /// Initialises the Serializer to work with the given data.
        /// </summary>
        /// <param name="data">Concrete Object of the AbstractType Specified.</param>
        public AbstractXmlSerializer(TAbstractType data)
        {
            _data = data;
        }

        #region IXmlSerializable

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            // Cast the Data back from the Abstract Type.
            string typeAttrib = reader.GetAttribute("type");

            // Ensure the Type was Specified
            if (typeAttrib == null)
                throw new ArgumentNullException("Unable to Read Xml Data for Abstract Type '" + typeof(TAbstractType).Name +
                    "' because no 'type' attribute was specified in the XML.");

            Type type = Type.GetType(typeAttrib);

            // Check the Type is Found.
            if (type == null)
                throw new InvalidCastException("Unable to Read Xml Data for Abstract Type '" + typeof(TAbstractType).Name +
                    "' because the type specified in the XML was not found.");

            // Check the Type is a Subclass of the AbstractType.
            if (!type.IsSubclassOf(typeof(TAbstractType)))
                throw new InvalidCastException("Unable to Read Xml Data for Abstract Type '" + typeof(TAbstractType).Name +
                    "' because the Type specified in the XML differs ('" + type.Name + "').");

            // Read the Data, Deserializing based on the (now known) concrete type.
            reader.ReadStartElement();
            Data = (TAbstractType)new XmlSerializer(type).Deserialize(reader);
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {

            // Write the Type Name to the XML Element as an Attrib and Serialize
            var type = _data.GetType();

            // BugFix: Assembly must be FQN since Types can/are external to current.
            writer.WriteAttributeString("type", type.AssemblyQualifiedName);
            new XmlSerializer(type).Serialize(writer, _data);
        }

        #endregion
    }
}