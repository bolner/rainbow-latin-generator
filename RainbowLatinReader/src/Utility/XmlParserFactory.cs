namespace RainbowLatinReader;

class XmlParserFactory : IXmlParserFactory {
    public IXmlParser GetXmlParser(ICanonFile file, List<string> destinations) {
        return new XmlParser(file, destinations);
    }
}
