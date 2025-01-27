# Тестовое задание

Программа импортирует данные из различных источников в объекты `Configuration`. Она реализована в виде консольного приложения.

## Особенности

- **Поддерживаемые форматы**: Программа принимает файлы двух форматов - XML и CSV. В каждом файле ожидается описание **одного** объекта `Configuration`.
- **Отображение конфигураций**: После обработки каждого нового файла программа выводит список всех имеющихся конфигураций.
- **Автоматический выбор файлов**: Файлы автоматически выбираются из директории `TestTask/examples` без участия пользователя.

## Класс Configuration

```csharp
public class Configuration
{
    public string Name { get; set; }
    public string Description { get; set; }
}
```

## Шаблоны содержимого файлов

### XML

```xml
<?xml version="1.0" encoding="utf-8"?>
<config>
    <name>Конфигурация 1</name>
    <description>Описание Конфигурации 1</description>
</config>
```
В файлах ожидается: XML declaration, один корневой элемент `config` и дочерние элементы `name` и `description`, допускается дополнительное наличие иных дочерних элементов
### CSV

```csv
Конфигурация 2;Описание Конфигурации 2
```

## Добавление поддержки нового формата файлов

1. **Реализация нового парсера**:
   В папке `TestTask/src/Parsers` создайте класс парсера соответствующего формата файлов, реализующий интерфейс `IConfigurationParser`.

2. **Обновление поддерживаемых форматов**:
   Добавьте новый формат файлов в enum `SupportedConfigurationFormat`.

3. **Регистрация нового парсера**:
   В конструкторе класса `ConfigurationManager` добавьте в словарь `_parserMapping` пару ключ-значение: `(SupportedConfigurationFormat.new_format, new Parser())`.

## Юнит-тестирование

Юнит-тесты для основных сценариев реализованы в проекте `TestTask.Test`.