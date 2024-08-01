using TestTask.Managers;

var manager = new ConfigurationManager(Console.Out);
var directoryPath = Path.Combine(AppContext.BaseDirectory, "../../../examples");
manager.ImportAndDisplayConfigurationsFromDirectory(directoryPath);
