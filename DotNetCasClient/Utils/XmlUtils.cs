using System;
using System.IO;
using System.Xml;
using log4net;

namespace DotNetCasClient.Utils
{
  /// <summary>
  /// Utility methods for parsing XML.
  /// </summary>
  /// <author>Catherine D. Winfrey (.Net)</author>
  public sealed class XmlUtils
  {
    /// <summary>
    /// Access to the log file
    /// </summary>
    static ILog LOG =
      LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
    /// <summary>
    /// Parses the text for a specified element, assuming that there is at most one such
    /// element
    /// </summary>
    /// <param name="xmlAsString">the xml to be parsed</param>
    /// <param name="qualifiedElementName">the element to match,qualified with namespace</param>
    /// <returns>the text value of the element</returns>
    public static string GetTextForElement(string xmlAsString, string qualifiedElementName)
    {
      string elementText = null;
      if (CommonUtils.IsNotBlank(xmlAsString)  &&
          CommonUtils.IsNotBlank(qualifiedElementName))
      {
        TextReader textReader = new StringReader(xmlAsString);
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.ConformanceLevel = ConformanceLevel.Auto;
        settings.IgnoreWhitespace = true;
        XmlReader reader = XmlReader.Create(textReader, settings);
        bool foundElement = reader.ReadToFollowing(qualifiedElementName);
        if (foundElement) {
          elementText = reader.ReadElementString();
        }
        if (reader != null) {
          reader.Close();
        }
        if (textReader != null) {
          textReader.Close();
        }
      }
      return elementText;
    }
  }
}
