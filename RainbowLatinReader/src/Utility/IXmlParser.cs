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

namespace RainbowLatinReader;

interface IXmlParser : IDisposable {
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
