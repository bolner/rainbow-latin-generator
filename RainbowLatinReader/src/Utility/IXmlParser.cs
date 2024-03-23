using System.Xml;

namespace RainbowLatinReader;

interface IXmlParser {
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
    public bool GoTo(string destination);

    /// <summary>
    /// Stops at the next destination and pre-fetches all text
    /// after it until either the next destination or until
    /// the end of the document.
    /// </summary>
    /// <returns>Returns false if no matching element found and the end of
    /// the document is reached, true otherwise.</returns>
    /// <exception cref="RainbowLatinException"></exception>
    public bool Next();
    public Dictionary<string, string> GetAttributes();
    public string? GetText();

    /// <summary>
    /// Read all text from a one deeper level.
    /// Ignore the 'note' elements, but get the text
    /// from other nodes.
    /// Stop when the same level is reached again.
    /// </summary>
    public string? ReadContent();

    public string? GetNodeName();
    public XmlNodeType? GetNodeType();
    public string GetDebugInfo();
}
