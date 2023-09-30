namespace RainbowLatinReader;

using System.IO;
using System.Xml;
using System.Collections.Generic;

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
    private readonly Dictionary<string, string> properties = new();
    private string? content = null;
    private readonly Stack<string> trace = new();

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

    public Dictionary<string, string> GetProperties() {
        return new Dictionary<string, string>(properties);
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
        content = null;

        while(!reader.EOF) {
            if (!prefetched) {
                if (!reader.Read()) {
                    break;
                }
            }

            prefetched = false;

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
    /// Read all text from a one deeper level,
    /// and ignore the text contents of embedded elements.
    /// </summary>
    /// <returns>True if any text was found, False otherwise.</returns>
    private bool ReadText() {
        List<string> parts = new();

        while(reader.Read()) {
            if (reader.Depth < trace.Count) {
                prefetched = true;
                break;
            }

            if (reader.NodeType == XmlNodeType.Text) {
                string text = reader.Value.Trim();
                
                if (text != "") {
                    parts.Add(text);
                }
            }
        }

        if (parts.Count > 0) {
            content = string.Join(" ", parts);
            return true;
        }

        return false;
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
