using System.Text.RegularExpressions;
using System.Xml;
using TestTask.Models;
using TestTask.Parsers.Interfaces;

namespace TestTask.Parsers;

public class XmlConfigurationParser : IConfigurationParser
{
    private const string RootConfigNode = "config";
    private const string ConfigNameNode = "name";
    private const string ConfigDescriptionNode = "description";

    public Configuration? Parse(string input, TextWriter writer)
    {
        var document = new XmlDocument();
        var parsingResult = TryParseXml(document, input);
        if (parsingResult is false)
        {
            writer.WriteLine("Error: Failed to parse XML configuration");
            return null;
        }
        
        var xmlDeclarationExists = document.FirstChild.NodeType == XmlNodeType.XmlDeclaration;
        if (!xmlDeclarationExists)
        {
            writer.WriteLine("Error: Content has no XML declaration");
            return null;
        }

        var possibleRoots = document.GetElementsByTagName(RootConfigNode);

        var rootNode = possibleRoots.Item(0);
        return GetConfiguration(rootNode, writer);
    }
 
    private static bool TryParseXml(XmlDocument document, string input)
    {
        try
        {
            document.LoadXml(input);
            return true;
        }
        catch (XmlException)
        {
            return false;
        }
    }

    private static Configuration? GetConfiguration(XmlNode rootNode, TextWriter writer)
    {
        var (configName, nameNodeCheckResult) = CheckNodeExistence(rootNode, ConfigNameNode);
        if (nameNodeCheckResult is false)
        {
            writer.WriteLine("Error: Failed to find config name element");
            return null;
        }
        var (configDesc, descNodeCheckResult) = CheckNodeExistence(rootNode, ConfigDescriptionNode);
        if (descNodeCheckResult is false)
        {
            writer.WriteLine("Error: Failed to find config description element");
            return null;
        }

        return new Configuration
        {
            Name = configName!.Trim(),
            Description = configDesc!.Trim()
        };
    }

    private static (string? Value,bool Result) CheckNodeExistence(XmlNode? rootNode, string nodeNameToCheck)
    {
        if (rootNode is null)
            return (null, false);
        
        foreach (XmlNode childNode in rootNode.ChildNodes)
        {
            if (childNode.Name == nodeNameToCheck)
            {
                var trim = Regex.Replace(childNode.InnerText, "\n(\t){1}", "\n");
                return (trim, true);
            }
        }

        return (null, false);
    }
}