using TestTask.Models;
using TestTask.Parsers.Interfaces;

namespace TestTask.Parsers;

public class CsvConfigurationParser : IConfigurationParser
{
    private const char CsvSeparator = ';';
    
    public Configuration? Parse(string input, TextWriter writer)
    {
        var parts = input.Split(CsvSeparator);
        if (parts.Length != 2)
        {
            writer.WriteLine($"Error: Failed to parse CSV configuration");
            return null;
        }

        return new Configuration
        {
            Name = parts[0].Trim(),
            Description = parts[1].Trim()
        };
    }
}
