using TestTask.Managers;

namespace TestTask.Test;

public class Tests
{
    private ConfigurationManager _manager = null!;
    private const string DirectoryPath = "Test";

    [SetUp]
    public void Setup()
    {
        Directory.CreateDirectory(DirectoryPath);
    }

    [TearDown]
    public void TearDown()
    {
        Directory.Delete(DirectoryPath, true);
    }

    [TestCase("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<config>\n\t<name>Просто пустая</name>\n\t<description></description>\n</config>")]
    [TestCase("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<config>\n\t<name></name>\n\t<description></description>\n</config>")]
    [TestCase("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<config>\n\t<name></name>\n\t<description>Пустое описание</description>\n</config>")]
    public void ShouldImportConfigWhenXmlPropertiesAreEmpty(string emptyPropsInput)
    {
        WriteToFile(DirectoryPath + "/emptyProps.xml", emptyPropsInput);
        using var sw = new StringWriter();
        _manager = new ConfigurationManager(sw);
        Console.SetOut(sw);

        _manager.ImportAndDisplayConfigurationsFromDirectory(DirectoryPath);

        Assert.That(sw.ToString(), Does.Not.Contain("Error"));
    }

    [TestCase("Пустой;")]
    [TestCase(";Пустой")]
    [TestCase(";")]
    public void ShouldImportConfigWhenCsvPropertiesAreEmpty(string emptyPropsInput)
    {
        WriteToFile(DirectoryPath + "/emptyProps.csv", emptyPropsInput);
        using var sw = new StringWriter();
        _manager = new ConfigurationManager(sw);
        Console.SetOut(sw);

        _manager.ImportAndDisplayConfigurationsFromDirectory(DirectoryPath);

        Assert.That(sw.ToString(), Does.Not.Contain("Error"));
    }

    [Test]
    public void ShouldWriteMessageWhenDirectoryIsEmpty()
    {
        using var sw = new StringWriter();
        _manager = new ConfigurationManager(sw);
        Console.SetOut(sw);

        _manager.ImportAndDisplayConfigurationsFromDirectory(DirectoryPath);

        Assert.That(RemoveNewLinesEscapeChars(sw.ToString()), Is.EqualTo("Directory is empty"));
    }

    [Test]
    public void ShouldWriteErrorMessageWhenExtensionIsNotSupported()
    {
        const string input =
            "\"<?xml version=\\\"1.0\\\" encoding=\\\"utf-8\\\"?>\\n<config>\\n\\t<name></name>\\n\\t<description>Пустое описание</description>\\n</config>\"";
        const string fileExtension = "txt";
        var filePath = Path.GetRelativePath(".", Path.Combine(DirectoryPath + "/badExtension." + fileExtension));
        WriteToFile(filePath, input);
        
        using var sw = new StringWriter();
        _manager = new ConfigurationManager(sw);
        Console.SetOut(sw);

        _manager.ImportAndDisplayConfigurationsFromDirectory(DirectoryPath);

        Assert.That(
            RemoveNewLinesEscapeChars(sw.ToString()),
            Is.EqualTo($"Error: Found unsupported configuration format: '{fileExtension}' when importing {filePath}"));
    }
    
    [TestCase(
        "Название конфигурации;Описание конфигурации",
        "Error: Failed to parse XML configuration")]
    [TestCase(
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "\n<config>\n\t<name>Название1</name>\n\t<description>Описание1</description>\n</config>" +
            "\n<config>\n\t<name>Название2</name>\n\t<description>Описание2</description>\n</config>",
        "Error: Failed to parse XML configuration")]
    [TestCase(
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
        "Error: Failed to parse XML configuration")]
    [TestCase(
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
        "\n<config>\n\t<description>Описание2</description>\n</config>",
        "Error: Failed to find config name element")]
    [TestCase(
        "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
        "\n<config>\n\t<header>Название1</header>\n\t<name>Название1</name>\n</config>",
        "Error: Failed to find config description element")]
    [TestCase("<config>\n\t<name>Название1</name>\n\t<description>Описание</description>\n</config>",
        "Error: Content has no XML declaration")]
    public void ShouldWriteErrorMessageWhenXmlIsInvalid(string invalidXmlInput, string errorMessage)
    {
        var filePath = Path.GetRelativePath(
            ".", Path.Combine($"{DirectoryPath}/invalidXml.xml"));
        WriteToFile(filePath, invalidXmlInput);
        
        using var sw = new StringWriter();
        _manager = new ConfigurationManager(sw);
        Console.SetOut(sw);

        _manager.ImportAndDisplayConfigurationsFromDirectory(DirectoryPath);

        Assert.That(RemoveNewLinesEscapeChars(sw.ToString()), Is.EqualTo(errorMessage));
    }

    [TestCase(";;")]
    [TestCase("")]
    public void ShouldWriteErrorMessageWhenCsvIsInvalid(string invalidCsvInput)
    {
        var filePath = Path.GetRelativePath(
            ".", Path.Combine($"{DirectoryPath}/invalidCsv.csv"));
        WriteToFile(filePath, invalidCsvInput);
        
        using var sw = new StringWriter();
        _manager = new ConfigurationManager(sw);
        Console.SetOut(sw);

        _manager.ImportAndDisplayConfigurationsFromDirectory(DirectoryPath);

        Assert.That(RemoveNewLinesEscapeChars(sw.ToString()),
            Is.EqualTo("Error: Failed to parse CSV configuration"));
    }

    private static string RemoveNewLinesEscapeChars(string input) =>
        input.Replace(Environment.NewLine, string.Empty);

    private static void WriteToFile(string path, string input)
    {
        var writer = File.AppendText(path);
        writer.Write(input);
        writer.Close();
    }
}