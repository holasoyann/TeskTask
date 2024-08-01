using TestTask.Models;

namespace TestTask.Parsers.Interfaces;

public interface IConfigurationParser
{
    Configuration? Parse(string input, TextWriter writer);
}
