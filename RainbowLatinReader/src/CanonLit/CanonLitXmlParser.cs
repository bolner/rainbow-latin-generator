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
sealed class CanonLitXmlParser : ICanonLitXmlParser {
    private bool isDisposed = false;

    /// <summary>
    /// The reader's position is already on the next node.
    /// There can be 2 reasons for the prefetched state:
    /// - When using the Skip() method, it already reads the node that
    ///     comes after the skipped one.
    /// - When a destination is detected we wish to re-execute the
    ///     whole logic without stepping over the destination tag.
    /// </summary>
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
    private bool addNewLine = false;
    private readonly HashSet<string> ignoreTags = ["note", "bibl", "del"];
    private readonly HashSet<string> addNewLineAfterTag = ["l"];
    private readonly HashSet<string> choice_accepted = ["abbr", "choice", "expan",
        "ex", "corr", "sic", "reg", "orig"];
    private readonly HashSet<string> choice_outer = ["choice", "abbr"];
    private readonly HashSet<string> choice_inner = ["expan", "corr", "orig"];

    /// <summary>
    /// Create an XmlParser object.
    /// </summary>
    /// <param name="file">The file to be parsed.</param>
    /// <param name="destinations">List of regular expressions that
    /// are matched against element paths, used by the Next() method.</param>
    public CanonLitXmlParser(ICanonFile file, List<string> destinations) {
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

    /// <summary>
    /// Returns the accumulated text from the buffer
    /// and then clears the text buffer.
    /// </summary>
    /// <returns>The accumulated text or null if there's none.</returns>
    public string? FetchTextBuffer() {
        if (content.Length < 1) {
            return null;
        }

        string response = whitespaceRegEx.Replace(content.ToString(), " ");
        content.Clear();

        return response;
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
    /// <param name="stopAt">The parsing won't go further than
    /// this element.</param>
    /// <returns>Returns false if no matching element found and the end of
    /// the document is reached, true otherwise.</returns>
    /// <exception cref="RainbowLatinException"></exception>
    public bool GoTo(string destination, string? stopAt = null) {
        Regex dest = new(destination);
        Regex? stop = null;

        if (stopAt != null) {
            stop = new(stopAt);
        }

        attributes.Clear();
        content.Clear();

        try {
            while(Read()) {
                /*
                    Element
                */
                if (reader.NodeType == XmlNodeType.Element) {
                    string name = reader.Name.ToLower();

                    if (ignoreTags.Contains(name)) {
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

                    if (stop != null) {
                        if (stop.IsMatch(path)) {
                            prefetched = true;
                            return false;
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

    /// <summary>
    /// Handles these choice structures:
    /// - ‹abbr›‹expan›suppromus es‹/expan›suppromu's‹/abbr› -
    /// - ‹abbr›suppromu's‹expan›suppromus es‹/expan›‹/abbr› -
    /// - ‹abbr›ausu's‹expan›‹ex›ausus es‹/ex›‹/expan› -
    /// - ‹choice›‹abbr›M.‹/abbr›‹expan›M‹ex›arci‹/ex›‹/expan›‹/choice› -
    /// - ‹choice›‹corr›ante‹/corr›‹sic›anta‹/sic›‹/choice› -
    /// - ‹choice›‹reg›aenamque‹/reg›‹orig›ænamque‹/orig›‹/choice› -
    /// </summary>
    /// <returns>Returns true if a choice structure was found, false otherwise.</returns>
    private bool DetectAndHandleChoices() {
        if (reader.NodeType == XmlNodeType.EndElement) {
            return false;
        }

        Stack<string> ending = [];

        if (choice_outer.Contains(reader.Name)) {
            ending.Push(reader.Name);
        } else {
            return false;
        }

        while (Read())
        {
            if (reader.NodeType == XmlNodeType.EndElement
                && reader.Name == ending.Peek())
            {
                ending.Pop();

                if (ending.Count == 0) {
                    break;
                }

                continue;
            }

            if (reader.NodeType == XmlNodeType.Element) {
                if (ignoreTags.Contains(reader.Name)) {
                    reader.Skip();
                    prefetched = true;
                    continue;
                }

                if (!choice_accepted.Contains(reader.Name)) {
                    throw new RainbowLatinException($"Invalid choice structure. Unexpected tag: {reader.Name}"
                        + "\n" + GetDebugInfo() + "\nChoice stack content: " +
                        string.Join(", ", ending));
                }

                if (choice_inner.Contains(reader.Name)) {
                    ending.Push(reader.Name);
                }
            }
            else if (reader.NodeType == XmlNodeType.Text) {
                if (choice_inner.Contains(ending.Peek())) {
                    content.Append(reader.Value);
                }
            }
        }

        return true;
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
            while(Read()) {
                /*
                    Element
                */
                if (reader.NodeType == XmlNodeType.Element) {
                    string name = reader.Name.ToLower();

                    if (ignoreTags.Contains(name)) {
                        reader.Skip();
                        prefetched = true;
                        continue;
                    }

                    if (addNewLineAfterTag.Contains(name)) {
                        addNewLine = true;
                    }

                    if (DetectAndHandleChoices()) {
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
                        if (addNewLine) {
                            addNewLine = false;
                            content.Append("<br>");
                        }
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

    private bool Read() {
        if (!prefetched) {
            if (reader.EOF) {
                return false;
            }

            bool ret = reader.Read();
            lineNumber = ((IXmlLineInfo)reader).LineNumber;

            return ret;
        }

        prefetched = false;

        return true;
    }

    /// <summary>
    /// Read all text from a one deeper level.
    /// Ignore the 'note' elements, but get the text
    /// from other nodes.
    /// Stop when the same level is reached again.
    /// </summary>
    public string? FetchContent() {
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
        
        while(Read()) {
            if (reader.Depth <= baseDepth) {
                prefetched = true;
                break;
            }

            if (reader.NodeType == XmlNodeType.Text) {
                if (reader.Value != "") {
                    if (addNewLine) {
                        addNewLine = false;
                        content.Append("<br>");
                    }

                    parts.Append(reader.Value);
                }
            }
            else if (reader.NodeType == XmlNodeType.Element) {
                string name = reader.Name.ToLower();

                if (ignoreTags.Contains(name)) {
                    reader.Skip();
                    prefetched = true;
                    continue;
                }

                if (addNewLineAfterTag.Contains(name)) {
                    addNewLine = true;
                }

                if (DetectAndHandleChoices()) {
                    continue;
                }
            }
        }

        content.Clear();

        if (addNewLine) {
            addNewLine = false;
            content.Append("<br>");
        }
        content.Append(whitespaceRegEx.Replace(parts.ToString(), " ").Trim());

        return FetchTextBuffer();
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
