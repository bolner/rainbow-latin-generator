/*
Copyright 2024 Tamas Bolner

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Xml;
using System.Text.RegularExpressions;
using System.Text;

namespace RainbowLatinReader;

/// <summary>
/// This parser is for the "canonical literature" documents.
/// </summary>
sealed class XmlParser : IXmlParser {
    private bool isDisposed = false;
    private bool prefetched = false;
    private readonly ICanonFile file;
    private readonly Stream stream;
    private readonly XmlReader reader;
    private readonly Dictionary<string, string> attributes = [];
    private readonly StringBuilder content = new();
    private readonly Stack<string> trace = new();
    private readonly Regex whitespaceRegEx = new(
        @"[\s]+",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
    private readonly List<Regex> destinations = [];
    private string? nodeName = null;
    private XmlNodeType? nodeType = null;
    private int lineNumber = 0;

    /// <summary>
    /// Create an XmlParser object.
    /// </summary>
    /// <param name="file">The file to be parsed.</param>
    /// <param name="destinations">List of regular expressions that
    /// are matched against element paths, used by the Next() method.</param>
    public XmlParser(ICanonFile file, List<string> destinations) {
        this.file = file;
        stream = file.Open();

        reader = XmlReader.Create(stream, new XmlReaderSettings() {
            DtdProcessing = DtdProcessing.Ignore
        });

        foreach(string destination in destinations) {
            this.destinations.Add(
                new Regex(destination, RegexOptions.Compiled | RegexOptions.IgnoreCase)
            );
        }
    }

    public Dictionary<string, string> GetAttributes() {
        return new Dictionary<string, string>(attributes);
    }

    public string? GetText() {
        if (content.Length < 1) {
            return null;
        }
        
        return whitespaceRegEx.Replace(content.ToString(), " ");
    }

    /// <summary>
    /// Starts parsing the document starting from the current location
    /// until the destination is found, then stops and reads to attributes.
    /// You'll have to make an additional call to ReadContent() in order
    /// to get the text contents of the element.
    /// All "note" elements are skipped / ignored.
    /// Returns false if no matching element found and the end of
    /// the document is reached, true otherwise.
    /// </summary>
    /// <param name="destinations">A path pattern to search for.
    /// Stop when it is reached.</param>
    /// <returns>Returns false if no matching element found and the end of
    /// the document is reached, true otherwise.</returns>
    /// <exception cref="RainbowLatinException"></exception>
    public bool GoTo(string destination) {
        Regex dest = new(destination);

        attributes.Clear();
        content.Clear();

        try {
            while(!reader.EOF || prefetched) {
                if (!prefetched) {
                    if (!reader.Read()) {
                        break;
                    }
                } else {
                    prefetched = false;
                }

                /*
                    Element
                */
                if (reader.NodeType == XmlNodeType.Element) {
                    if (reader.Name.ToLower() == "note") {
                        reader.Skip();
                        prefetched = true;
                        continue;
                    }

                    /*
                        Update trace
                    */
                    if (reader.Depth < trace.Count) {
                        // Step out
                        for(int i = trace.Count; i > reader.Depth; i--) {
                            trace.Pop();
                        }
                    }

                    trace.Push(reader.Name);

                    /*
                        Test for destination
                    */
                    string path = string.Join(".", trace.Reverse());
                    if (dest.IsMatch(path)) {
                        ReadProperties();
                        
                        return true;
                    }
                }
                else if (reader.NodeType == XmlNodeType.Text) {
                    if (reader.Value != "") {
                        content.Append(reader.Value);
                    }
                }
            }
        } catch (Exception ex) {
            throw new RainbowLatinException("XmlParser.Next(): " + ex.Message + "\n" + GetDebugInfo(), ex);
        }

        return false;
    }

    /// <summary>
    /// Stops at the next destination and pre-fetches all text
    /// after it until either the next destination or until
    /// the end of the document.
    /// </summary>
    /// <returns>Returns false if no matching element found and the end of
    /// the document is reached, true otherwise.</returns>
    /// <exception cref="RainbowLatinException"></exception>
    public bool Next() {
        attributes.Clear();
        content.Clear();

        try {
            while(!reader.EOF || prefetched) {
                if (!prefetched) {
                    if (!reader.Read()) {
                        break;
                    }
                } else {
                    prefetched = false;
                }

                /*
                    Element
                */
                if (reader.NodeType == XmlNodeType.Element) {
                    if (reader.Name.ToLower() == "note") {
                        reader.Skip();
                        prefetched = true;
                        continue;
                    }

                    /*
                        Update trace
                    */
                    if (reader.Depth < trace.Count) {
                        // Step out
                        for(int i = trace.Count; i > reader.Depth; i--) {
                            trace.Pop();
                        }
                    }

                    trace.Push(reader.Name);

                    /*
                        Test for destination
                    */
                    string path = string.Join(".", trace.Reverse());
                    foreach(Regex destination in destinations) {
                        if (destination.IsMatch(path)) {
                            ReadProperties();
                            
                            return true;
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.Text) {
                    if (reader.Value != "") {
                        content.Append(reader.Value);
                    }
                }
            }
        } catch (Exception ex) {
            throw new RainbowLatinException("XmlParser.Next(): " + ex.Message + "\n" + GetDebugInfo(), ex);
        }

        return false;
    }

    private bool ReadProperties() {
        nodeType = reader.NodeType;
        nodeName = reader.Name;
        lineNumber = ((IXmlLineInfo)reader).LineNumber;

        if (reader.NodeType != XmlNodeType.Element) {
            return false;
        }

        while (reader.MoveToNextAttribute()) {
            attributes[reader.Name] = reader.Value;
        }

        return attributes.Count > 0;
    }

    /// <summary>
    /// Read all text from a one deeper level.
    /// Ignore the 'note' elements, but get the text
    /// from other nodes.
    /// Stop when the same level is reached again.
    /// </summary>
    public string? ReadContent() {
        int baseDepth = reader.Depth;
        if (reader.NodeType == XmlNodeType.Attribute) {
            /*
                We are on the last attribute of a node
                and attributes are also a level lower
                than the node, just like the content.
            */
            baseDepth--;
        }
        StringBuilder parts = new();
        
        while(!reader.EOF || prefetched) {
            if (!prefetched) {
                if (!reader.Read()) {
                    break;
                }
            } else {
                prefetched = false;
            }

            if (reader.Depth <= baseDepth) {
                prefetched = true;
                break;
            }

            if (reader.NodeType == XmlNodeType.Text) {
                if (reader.Value != "") {
                    parts.Append(reader.Value);
                }
            }
            else if (reader.NodeType == XmlNodeType.Element) {
                if (reader.Name.ToLower() == "note") {
                        reader.Skip();
                        prefetched = true;
                        continue;
                }
            }
        }

        content.Clear();
        content.Append(whitespaceRegEx.Replace(parts.ToString(), " ").Trim());

        return GetText();
    }

    /// <summary>
    /// IDisposable interface
    ///  - We have only managed resources and this class is sealed,
    ///    therefore most of the methods here are not required:
    ///    https://learn.microsoft.com/en-us/dotnet/api/system.idisposable?view=net-8.0
    /// </summary>
    public void Dispose() {
        if (!isDisposed) {
            reader.Dispose();
            stream.Dispose();
            isDisposed = true;
        }
    }

    public string? GetNodeName() {
        return nodeName;
    }

    public XmlNodeType? GetNodeType() {
        return nodeType;
    }

    public string GetDebugInfo() {
        return $"FILE '{file.GetPath()}', LINE {lineNumber}.";
    }

    public void Skip() {
        reader.Skip();
        prefetched = true;
    }
}
