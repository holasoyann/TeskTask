using TestTask.Models;
using TestTask.Parsers;
using TestTask.Parsers.Interfaces;

namespace TestTask.Managers;

public class ConfigurationManager
{
    private readonly List<Configuration> _configurations = new();
    private readonly Dictionary<SupportedConfigurationFormat, IConfigurationParser> _parserMapping = new();
    private readonly TextWriter _writer;

    public ConfigurationManager(TextWriter writer)
    {
        _writer = writer;
        _parserMapping.Add(SupportedConfigurationFormat.Csv, new CsvConfigurationParser());
        _parserMapping.Add(SupportedConfigurationFormat.Xml, new XmlConfigurationParser());
    }
    
    private void ImportAndDisplayConfigurationFromFile(string filePath)
    {
        var fileContent = File.ReadAllText(filePath);
        var rawFileExtension = Path.GetExtension(filePath).ToLower().TrimStart('.');

        var configurationFormat = GetFileFormat(rawFileExtension);
        if (configurationFormat is null)
        {
            _writer.WriteLine($"Error: Found unsupported configuration format: '{rawFileExtension}' when importing {filePath}");
            return;
        }
        
        if (!_parserMapping.TryGetValue(configurationFormat.Value, out var parser))
        {
            _writer.WriteLine($"Error: Couldn't find parser for format {configurationFormat} when importing {filePath}");
            return;
        }
        
        var config = parser.Parse(fileContent, _writer);
        if (config == null)
            return;

        _configurations.Add(config);
        DisplayCurrentConfigurations(filePath);
    }

    public void ImportAndDisplayConfigurationsFromDirectory(string directoryPath)
    {
        var files = Directory.GetFiles(directoryPath);
        if (files.Length == 0)
        {
            _writer.WriteLine("Directory is empty");
            return;
        }

        foreach (var filePath in files)
        {
           ImportAndDisplayConfigurationFromFile(filePath);
        }
    }

    private void DisplayCurrentConfigurations(string filePath)
    {
        _writer.WriteLine($"All configurations after importing from file {filePath}");
        foreach (var configuration in  _configurations)
        {
            _writer.WriteLine($"Configuration name:\n{IndentConfigValue(configuration.Name)}");
            _writer.WriteLine($"Configuration description:\n{IndentConfigValue(configuration.Description)}\n");
        }
        _writer.WriteLine("============================\n");
    }

    private static SupportedConfigurationFormat? GetFileFormat(string rawFileExtension)
    {
        Enum.TryParse(
            typeof(SupportedConfigurationFormat),
            rawFileExtension,
            true,
            out var configurationFormat);

        return (SupportedConfigurationFormat?)configurationFormat;
    }

    private static string IndentConfigValue(string value) =>
        string.IsNullOrEmpty(value)
            ? value
            : "\t" + value.Replace(Environment.NewLine, Environment.NewLine + "\t");
}