using System.Xml;
using System.Text.RegularExpressions;

namespace RainbowLatinReader;

/// <summary>
/// This parser has some very specific design choices:
/// - Minimal interface for this application, so unit testing is easy.
/// - The element names in the XML files cannot contain "." (period) characters.
/// </summary>
sealed class XmlParser : IXmlParser, IDisposable {
    private bool isDisposed = false;
    private bool prefetched = false;
    private readonly ICanonFile file;
    private readonly Stream stream;
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
    /// <param name="file">The file to be parsed.</param>
    public XmlParser(ICanonFile file) {
        this.file = file;
        stream = file.Open();

        reader = XmlReader.Create(stream, new XmlReaderSettings() {
            DtdProcessing = DtdProcessing.Parse
        });
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
                if (reader.Name.Contains(".")) {
                    IXmlLineInfo xmlInfo = (IXmlLineInfo)reader;
                    throw new RainbowLatinException("Invalid XML document. Period characters"
                        + $" are not allowed in element names. LINE {xmlInfo.LineNumber} in "
                        + $"FILE '{file.GetPath()}'.");
                }

                /*
                    Update trace
                */
                // TODO: check if duplicates in trace
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
                if (MatchesTheEnding(path, destination)) {
                    ReadProperties();
                    content = ReadText();

                    return true;
                }

                /*
                    Test for traps
                */
                foreach(string trap in traps) {
                    if (MatchesTheEnding(path, trap)) {
                        var trapContent = ReadText();

                        if (trapContent != null) {
                            trapContent = trapContent.Trim();

                            if (trapContent != "") {
                                if (!captures.ContainsKey(trap)) {
                                    captures[trap] = new List<string>();
                                }

                                captures[trap].Add(trapContent);
                            }
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
    /// <returns>The text if any was found, null otherwise.</returns>
    private string? ReadText() {
        List<string> parts = new();
        int baseDepth = reader.Depth;

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

            return content;
        }

        return null;
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
            stream.Dispose();
            isDisposed = true;
        }
    }
}
