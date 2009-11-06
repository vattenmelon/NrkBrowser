using System;

namespace Vattenmelon.Nrk.Browser.Translation
{
    public interface ITranslationService
    {
        /// <summary>
        /// This is the constructor actually used by the plugin. It gets the currently used language in mediaportal and loads the appropriate language
        /// </summary>
        void Init();

        /// <summary>
        /// This constructor is intended for testing purposes
        /// </summary>
        /// <param name="language">The language to load</param>
        /// <param name="directory">The full path (without filename) to where the language files are located</param>
        void InitWithParam(String language, String directory);

        /// <summary>
        /// Returns the translated string for the key stringKey
        /// </summary>
        /// <param name="stringKey">The key for the translated string.</param>
        /// <returns>The translated string</returns>
        string Get(string stringKey);

        /// <summary>
        /// Method that checks if the key is translated.
        /// </summary>
        /// <param name="stringKey">The stringKey to check</param>
        /// <returns>True if it is translated, false if not.S</returns>
        bool Contains(string stringKey);

        /// <summary>
        /// Returns the numger of translated strings.
        /// </summary>
        /// <returns></returns>
        int GetNumberOfTranslatedStrings();
    }
}