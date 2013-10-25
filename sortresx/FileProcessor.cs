using System;
using System.IO;
using System.Xml;
using System.Collections;

namespace Codice.SortResX
{
    public class FileProcessor
    {
        public FileProcessor(string path)
        {
            mPath = path;
            mResourceNameList = new ArrayList();
            mResourceNodes = new Hashtable();
            mDoc = new XmlDocument();
            try
            {
                mDoc.Load(mPath);
            }
            catch (XmlException ex)
            {
                Console.WriteLine("The XML file is not correct. Message: " + ex.Message);
                throw ex;
            }
        }

        public void Process()
        {
            string[] sortedNames;

            try
            {
                ExtractResources();
                sortedNames = SortResourceList();
                WriteOrderedResources(sortedNames);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when processing the file. Message: " + ex.Message);
                throw ex;
            }
        }

        void ExtractResources()
        {
            XmlNodeList rootList = mDoc.GetElementsByTagName("root");
            foreach (XmlNode rootNode in rootList)
            {
                foreach (XmlNode node in rootNode.ChildNodes)
                {
                    if (!node.Name.EndsWith("data"))
                    {
                        continue;
                    }

                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name.EndsWith("name"))
                        {
                            AddXmlNode(node, attr);
                        }
                    }
                }

                XmlNodeList deleteList = rootNode.SelectNodes("/root/data");
                foreach(XmlNode delNode in deleteList)
                {
                    rootNode.RemoveChild(delNode);
                }
            }
        }

        void AddXmlNode(XmlNode node, XmlAttribute attribute)
        {
            if (mResourceNodes.ContainsKey(attribute.Value.ToString()))
                return;

            mResourceNodes.Add(attribute.Value.ToString(), node);
            mResourceNameList.Add(attribute.Value.ToString());
        }

        string[] SortResourceList()
        {
            string[] names = new string[mResourceNameList.Count];

            for (int i = 0; i < mResourceNameList.Count; i++)
                names[i] = (string)mResourceNameList[i];

            Array.Sort(names);
            return names;
        }

        void WriteOrderedResources(string[] names)
        {
            XmlNodeList rootList = mDoc.GetElementsByTagName("root");
            foreach (XmlNode rootNode in rootList)
            {
                foreach (string key in names)
                {
                    rootNode.AppendChild((XmlNode)mResourceNodes[key]);
                }
            }
            mDoc.Save(mPath);
        }

        private ArrayList mResourceNameList = null;
        private Hashtable mResourceNodes = null;
        private XmlDocument mDoc = null;
        private string mPath = null;
    }
}