using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MediaPortal.Configuration;
using MediaPortal.GUI.Library;
using Vattenmelon.Nrk.Parser;

namespace Vattenmelon.Nrk.Browser.Translation
{
    public class TranslationService : ITranslationService
    {
        private string path;
        
        Dictionary<string, string> translatedStrings = new Dictionary<string, string>();

        /// <summary>
        /// This is the constructor actually used by the plugin. It gets the currently used language in mediaportal and loads the appropriate language
        /// </summary>
        public void Init()
        {
            string lang = GUILocalizeStrings.GetCultureName(GUILocalizeStrings.CurrentLanguage());
            Log.Info("Using language " + lang);
            path = Config.GetSubFolder(Config.Dir.Language, NrkConstants.LANGUAGE_DIR);
            loadTranslatedStringsFromFile(lang);
        }
        /// <summary>
        /// This constructor is intended for testing purposes
        /// </summary>
        /// <param name="language">The language to load</param>
        /// <param name="directory">The full path (without filename) to where the language files are located</param>
        public void InitWithParam(String language, String directory)
        {
            this.path = directory;
            loadTranslatedStringsFromFile(language);

        }
        private void loadTranslatedStringsFromFile(string lang)
        {
            
            XmlDocument doc = new XmlDocument();
            string pathToLanguageFile = String.Empty;
            try
            {
                pathToLanguageFile = Path.Combine(path, lang + ".xml");
                doc.Load(pathToLanguageFile);
                addStringsToDictionary(doc);
            }
            catch (Exception e)
            {
                
                if (e.GetType() == typeof(FileNotFoundException)){
                    Log.Warn("Cannot find translation file {0}.  Falling back to hardcoded english", pathToLanguageFile);
                }
                else{
                    Log.Error(String.Format("Error in translation xml file: {0}. Falling back to hardcoded english", lang), e);
                }
                
            }
            
        }

        private void addStringsToDictionary(XmlDocument doc)
        {
            foreach (XmlNode stringEntry in doc.DocumentElement.ChildNodes)
            {
                if (stringEntry.NodeType == XmlNodeType.Element)
                {
                    translatedStrings.Add(stringEntry.Attributes.GetNamedItem("id").Value, stringEntry.InnerText);
                }
            }
        }

        /// <summary>
        /// Returns the translated string for the key stringKey
        /// </summary>
        /// <param name="stringKey">The key for the translated string.</param>
        /// <returns>The translated string</returns>
        public string Get(string stringKey)
        {
            return translatedStrings[stringKey];
        }
        /// <summary>
        /// Method that checks if the key is translated.
        /// </summary>
        /// <param name="stringKey">The stringKey to check</param>
        /// <returns>True if it is translated, false if not.S</returns>
        public bool Contains(string stringKey)
        {
            return translatedStrings.ContainsKey(stringKey);
        }
        /// <summary>
        /// Returns the numger of translated strings.
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfTranslatedStrings()
        {
            return translatedStrings.Count;
        }
    }
}