using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Lib.Base
{
    public static class XmlExtensions
    {
        public static T ToObjectFromXml<T>(this string str)
        {
            return (T) ToObjectFromXml(str, typeof (T));
        }

        public static string ToXml<T>(this T obj)
        {
            return ToXml(obj, typeof (T));
        }

        public static object ToObjectFromXml(this string str, Type t)
        {
            var temp = Encoding.UTF8.GetBytes(str);
            using (var mstream = new MemoryStream(temp))
            {
                var serializer = new XmlSerializer(t);
                var o = serializer.Deserialize(mstream);
                return o;
            }
        }

        public static string ToXml(this object obj, Type t = null)
        {
            if (t == null)
                t = obj.GetType();

            using (var mstream = new MemoryStream())
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var serializer = new XmlSerializer(t);
                serializer.Serialize(mstream, obj, ns);
                return Encoding.UTF8.GetString(mstream.ToArray());
            }
        }

        public static void ToXmlFile<T>(this T obj, string filePath)
        {
            using (var fileStream = new StreamWriter(filePath, false, Encoding.UTF8))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(fileStream, obj, ns);
            }
        }

        public static string ReplaceXmlNodeName(string str, string oldNodeName, string newNodeName)
        {
            return Regex.Replace(str, $@"\<(\/?){oldNodeName}\>", m => m.Value.Contains("/") ? $"</{newNodeName}>" : $"<{newNodeName}>");
        }

        #region XML

        public static XmlNode GetXmlNode(XmlNode xnNode, string sXPath)
        {
            if (xnNode == null)
            {
                const string sError = "XmlHelper.GetXmlNode() failed, input XML node is empty.";
                throw new Exception(sError);
            }

            var xn = xnNode.SelectSingleNode(sXPath);
            if (xn == null)
            {
                string sError = string.Format("Execute XPath failed, no XML node was found! /r/nXPath:({0})/r/nXML:({0})", sXPath);
                throw new Exception(sError);
            }

            return xn;
        }

        public static string GetXmlValue(XmlNode xnNode, string sXPath, string sDefault)
        {
            if (xnNode == null)
            {
                const string sError = "XmlHelper.GetXmlValue() failed, input XML node is empty.";
                throw new Exception(sError);
            }

            var xn = xnNode.SelectSingleNode(sXPath);
            if (xn == null)
            {
                return sDefault;
            }

            return xn.Value;
        }

        public static string GetXmlText(XmlNode xnNode, string sXPath, string sDefault)
        {
            if (xnNode == null)
            {
                const string sError = "XmlHelper.GetXmlText() failed, input XML node is empty.";
                throw new Exception(sError);
            }

            var xn = xnNode.SelectSingleNode(sXPath);
            if (xn == null)
            {
                return sDefault;
            }

            return xn.InnerText;
        }

        public static string Read(string path, string node, string attribute)
        {
            string value = "";
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                
                XmlNode xn = doc.SelectSingleNode(node);
                //value = (attribute.Equals("") ? xn.InnerText : xn.Attributes[attribute].Value);
                if (attribute.Equals(""))
                {
                    value = xn.InnerText;
                }
            }
            catch (Exception)
            {
            }
            return value;
        }

        public static void Insert(string path, string node, string element, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                int count = doc.SelectNodes(node).Count;
                XmlNode xn = doc.SelectNodes(node).Item(count - 1);

                if (element.Equals(""))
                {
                    if (!attribute.Equals(""))
                    {
                        XmlElement xe = (XmlElement)xn;
                        xe.SetAttribute(attribute, value);
                    }
                }
                else
                {
                    XmlElement xe = doc.CreateElement(element);
                    if (attribute.Equals(""))
                        xe.InnerText = value;
                    else
                        xe.SetAttribute(attribute, value);
                    xn.AppendChild(xe);
                }
                doc.Save(path);
            }
            catch { }
        }

        public static void Insert(string path, string node, string element, string attribute, string value, string inner)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                int count = doc.SelectNodes(node).Count;
                XmlNode xn = doc.SelectNodes(node).Item(count - 1);
                if (element.Equals(""))
                {
                    if (!attribute.Equals(""))
                    {
                        XmlElement xe = (XmlElement)xn;
                        xe.SetAttribute(attribute, value);
                        xe.InnerText = inner;
                    }
                }
                else
                {
                    XmlElement xe = doc.CreateElement(element);
                    if (attribute.Equals(""))
                        xe.InnerText = value;
                    else
                    {
                        xe.SetAttribute(attribute, value);
                        xe.InnerText = inner;
                    }
                    xn.AppendChild(xe);
                }
                doc.Save(path);
            }
            catch { }
        }

        public static void Update(string path, string node, string attribute, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xe.InnerText = value;
                else
                    xe.SetAttribute(attribute, value);
                doc.Save(path);
            }
            catch { }
        }

        public static void Delete(string path, string node, string attribute)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode xn = doc.SelectSingleNode(node);
                XmlElement xe = (XmlElement)xn;
                if (attribute.Equals(""))
                    xn.ParentNode.RemoveChild(xn);
                else
                    xe.RemoveAttribute(attribute);
                doc.Save(path);
            }
            catch { }
        }

        public static bool LoadXml(this string fullPath, out XmlDocument doc)
        {
            doc = null;
            FileStream fs = null;
            try
            {
                string xmlText;
                fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using (var sr = new StreamReader(fs, Encoding.Default))
                {
                    fs = null;
                    xmlText = sr.ReadToEnd();
                }

                if (xmlText.IsNullOrEmpty())
                    return false;
                doc = new XmlDocument();
                doc.LoadXml(xmlText);
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("LoadXml FullPath error:" + fullPath, ex);
            }
            finally
            {
                if (fs != null)
                {
                    fs.Dispose();
                }
            }
        }

        #endregion
    }
}