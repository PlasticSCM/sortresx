using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Codice.SortResX
{
    public class FileProcessor
    {
        private readonly List<string> _mResourceNameList;
        private readonly Dictionary<string, XmlNode> _mResourceNodes;
        private readonly XmlDocument _mDoc;
        private readonly string _mPath;

        private readonly Dictionary<string, string> _xpathQuery = new Dictionary<string, string>
        {
            {".resx", "data/@name"},
            {".dbml", "*[local-name() != 'ConnectionString']/@Name"}
        };

        public FileProcessor(string path)
        {
            _mPath = path;
            _mResourceNameList = new List<string>();
            _mResourceNodes = new Dictionary<string, XmlNode>();
            _mDoc = new XmlDocument();
            try
            {
                _mDoc.Load(_mPath);
            }
            catch (XmlException ex)
            {
                Console.WriteLine("The XML file is not correct. Message: " + ex.Message);
                throw;
            }
        }

        public bool Process()
        {
            try
            {
                if (!_xpathQuery.TryGetValue(Path.GetExtension(_mPath).ToLowerInvariant(), out var query))
                {
                    Console.WriteLine("Error when processing the file. Unsupported file extension: " + Path.GetExtension(_mPath));
                    return false;
                }

                ExtractResources(query);

	            var shouldSaveSort = TrySortResourceList(out var sortedNames);
                if (shouldSaveSort)
                {
                    WriteOrderedResources(sortedNames);
                    Console.WriteLine("Resx file '{0}' sorted successfully.", _mPath);
                    return true;
                }

                Console.WriteLine("Resx file '{0}' is already sorted. Nothing to do.", _mPath);
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error when processing the file. Message: " + ex.Message);
                throw;
            }
        }

        void ExtractResources(string query)
        {
            foreach (XmlAttribute attribute in _mDoc.DocumentElement.SelectNodes(query))
            {
                var element = attribute.OwnerElement;
                AddXmlNode(element, attribute);
                element.ParentNode.RemoveChild(element);
            }
        }

        void AddXmlNode(XmlNode node, XmlAttribute attribute)
        {
            if (_mResourceNodes.ContainsKey(attribute.Value))
                return;

            _mResourceNodes.Add(attribute.Value, node);
            _mResourceNameList.Add(attribute.Value);
        }

        bool TrySortResourceList(out string[]sortedNames)
        {
            string[] names = new string[_mResourceNameList.Count];

            for (int i = 0; i < _mResourceNameList.Count; i++)
                names[i] = _mResourceNameList[i];

	        sortedNames = names.OrderBy(s => s).ToArray();

	        return !sortedNames.SequenceEqual(names);
        }

        void WriteOrderedResources(string[] names)
        {
            foreach (string key in names)
            {
                _mDoc.DocumentElement.AppendChild(_mResourceNodes[key]);
            }

            _mDoc.Save(_mPath);
        }
    }
}
