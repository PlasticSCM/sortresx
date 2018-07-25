using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Codice.SortResX
{
    public class FileProcessor
    {
        public FileProcessor(string path)
        {
            mPath = path;
            mResourceNameList = new List<string>();
            mResourceNodes = new Dictionary<string, XmlNode>();
            mDoc = new XmlDocument();
            try
            {
                mDoc.Load(mPath);
            }
            catch (XmlException ex)
            {
                Console.WriteLine("The XML file is not correct. Message: " + ex.Message);
                throw;
            }
        }

        public void Process()
        {
            string[] sortedNames;

            try
            {
                var xpathQuery = new Dictionary<string, string>();
                xpathQuery.Add(".resx", "data/@name");
                xpathQuery.Add(".dbml", "*[local-name() != 'ConnectionString']/@Name");

                string query = null;
                if (!xpathQuery.TryGetValue(Path.GetExtension(mPath).ToLowerInvariant(), out query))
                {
                    Console.WriteLine("Error when processing the file. Unsupported file extension: " + Path.GetExtension(mPath));
                    return;
                }

                ExtractResources(query);
                sortedNames = SortResourceList();
                WriteOrderedResources(sortedNames);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when processing the file. Message: " + ex.Message);
                throw;
            }
        }

        void ExtractResources(string query)
        {
            foreach (XmlAttribute attribute in mDoc.DocumentElement.SelectNodes(query))
            {
                var element = attribute.OwnerElement;
                AddXmlNode(element, attribute);
                element.ParentNode.RemoveChild(element);
            }
        }

        void AddXmlNode(XmlNode node, XmlAttribute attribute)
        {
            if (mResourceNodes.ContainsKey(attribute.Value))
                return;

            mResourceNodes.Add(attribute.Value, node);
            mResourceNameList.Add(attribute.Value);
        }

        string[] SortResourceList()
        {
            string[] names = new string[mResourceNameList.Count];

            for (int i = 0; i < mResourceNameList.Count; i++)
                names[i] = mResourceNameList[i];

            Array.Sort(names);
            return names;
        }

        void WriteOrderedResources(string[] names)
        {
            foreach (string key in names)
            {
                mDoc.DocumentElement.AppendChild(mResourceNodes[key]);
            }

            mDoc.Save(mPath);
        }

        private List<string> mResourceNameList = null;
        private Dictionary<string, XmlNode> mResourceNodes = null;
        private XmlDocument mDoc = null;
        private string mPath = null;
    }
}
