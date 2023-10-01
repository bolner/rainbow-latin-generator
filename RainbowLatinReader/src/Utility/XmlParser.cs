namespace RainbowLatinReader;

using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// This parser has some very specific design choices:
/// - Minimal interface for this application, so unit testing is easy.
/// - The element names in the XML files cannot contain "." (period) characters.
/// </summary>
sealed class XmlParser : IXmlParser, IDisposable {
    private bool isDisposed = false;
    private bool prefetched = false;
    private readonly string filePath;
    private readonly XmlReader reader;
    private readonly List<string> traps = new();
    private readonly Dictionary<string, List<string>> captures = new();
    private readonly Dictionary<string, string> attributes = new();
    private string? content = null;
    private readonly Stack<string> trace = new();
    private readonly Regex whitespaceRegEx = new(
        @"[\s]+",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    /// <summary>
    /// Create an XmlParser object.
    /// </summary>
    /// <param name="stream">The XML will be read from this stream.</param>
    /// <param name="filePath">This is only used for error messages.
    ///     Should contain the full path to the xml file.</param>
    public XmlParser(Stream stream, string filePath) {
        reader = XmlReader.Create(stream, new XmlReaderSettings() {
            DtdProcessing = DtdProcessing.Parse
        });

        this.filePath = filePath;
    }

    public void SetTrap(string path) {
        traps.Add(path);
    }

    public void ClearTraps() {
        traps.Clear();
    }

    public Dictionary<string, List<string>> GetCaptures() {
        return new Dictionary<string, List<string>>(captures);
    }

    public Dictionary<string, string> GetAttributes() {
        return new Dictionary<string, string>(attributes);
    }

    public string? GetContent() {
        return content;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="destination"></param>
    /// <returns></returns>
    /// <exception cref="RainbowLatinException"></exception>
    public bool Next(string destination) {
        captures.Clear();
        attributes.Clear();
        content = null;

        while(!reader.EOF) {
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
                if (reader.Name.Contains(".")) {
                    IXmlLineInfo xmlInfo = (IXmlLineInfo)reader;
                    throw new RainbowLatinException("Invalid XML document. Period characters"
                        + $" are not allowed in element names. LINE {xmlInfo.LineNumber} in "
                        + $"FILE '{filePath}'.");
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
                else if (reader.Depth > trace.Count && trace.Count > 0) {
                    // Same level AND not the root node => Step over
                    trace.Pop();
                }

                trace.Push(reader.Name);

                /*
                    Test for destination
                */
                string path = string.Join(".", trace.Reverse());
                if (MatchesTheEnding(path, destination)) {
                    ReadProperties();
                    ReadText();

                    return true;
                }

                /*
                    Test for traps
                */
                foreach(string trap in traps) {
                    if (MatchesTheEnding(path, trap)) {
                        ReadText();

                        if (content != null) {
                            if (!captures.ContainsKey(trap)) {
                                captures[trap] = new List<string>();
                            }

                            captures[trap].Add(content);
                        }
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Read all text from a one deeper level.
    /// Ignore the 'note' elements, but get the text
    /// from other nodes.
    /// </summary>
    /// <returns>True if any text was found, False otherwise.</returns>
    private bool ReadText() {
        List<string> parts = new();
        int baseDepth = reader.Depth;

        while(!reader.EOF) {
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
                    parts.Add(reader.Value);
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

        if (parts.Count > 0) {
            content = string.Join("", parts);
            content = whitespaceRegEx.Replace(content, " ").Trim();

            return true;
        }

        return false;
    }

    private bool ReadProperties() {
        if (reader.NodeType != XmlNodeType.Element) {
            return false;
        }

        for (int i = 0; i < reader.AttributeCount; i++){
            reader.MoveToAttribute(i);
            attributes[reader.Name] = reader.Value;
        }

        return attributes.Count > 0;
    }

    private bool MatchesTheEnding(string path, string ending) {
        if (ending.Length > path.Length) {
            return false;
        }
        else if (ending.Length < path.Length) {
            return ("." + path).EndsWith("." + ending);
        }

        return path == ending;
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
            isDisposed = true;
        }
    }
}
