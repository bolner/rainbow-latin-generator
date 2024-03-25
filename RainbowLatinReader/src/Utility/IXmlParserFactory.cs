namespace RainbowLatinReader;

interface IXmlParserFactory {
    public IXmlParser GetXmlParser(ICanonFile file, List<string> destinations);
}
