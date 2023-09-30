namespace RainbowLatinReader;

class RainbowLatinException : Exception {
    public RainbowLatinException() { }

    public RainbowLatinException(string message)
        : base(message) { }

    public RainbowLatinException(string message, Exception inner)
        : base(message, inner) { }
}
