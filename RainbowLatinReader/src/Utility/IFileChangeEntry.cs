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
namespace RainbowLatinReader;

interface IFileChangeEntry {
    public enum ChangeType {
        StringReplace, SectionReplace, RegEx
    };

    /// <summary>
    /// Tells whether the change replaces a string
    /// or a section.
    /// </summary>
    public ChangeType GetChangeType();

    /// <summary>
    /// File name of the document.
    /// </summary>
    public string GetDocument();

    /// <summary>
    /// If ChangeType = StringReplace, then
    /// this returns the string to be replaced.
    /// </summary>
    public string GetMatch();

    /// <summary>
    /// The new string.
    /// </summary>
    public string GetReplace();

    /// <summary>
    /// If ChangeType = SectionReplace, then
    /// this returns the beginning of the section
    /// to be replaced.
    /// </summary>
    public string GetStart();

    /// <summary>
    /// If ChangeType = SectionReplace, then
    /// this returns the end of the section
    /// to be replaced.
    /// </summary>
    public string GetEnd();

    /// <summary>
    /// Replaces the first occurance of the change pattern.
    /// </summary>
    /// <param name="text">Reference to the text. Serves as
    /// both input and output. Will only be overwritten if
    /// the pattern matches.</param>
    /// <returns>True if the pattern matches, false otherwise.</returns>
    public bool Apply(ref string text);

    /// <summary>
    /// Returns a description of the change for debugging
    /// and error messages.
    /// </summary>
    public string ToString();
}
