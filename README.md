# ChaoticOnyx.Tools
Инструменты для работы с [OnyxBay](https://github.com/ChaoticOnyx/OnyxBay).

## ChaoticOnyx.Tools.ChangelogGenerator
Генератор чейнджлогов.

### Возможности
- [x] Создание кэша изменении.
- [ ] Поддержка хранения изменении в форматах XML, JSON.
- [ ] Поддержка CI.
- [ ] Генерация HTML на основе кэша.

### Настройки
Все возможные настройки перечислены в секции "Options" файла [appsettings.json](https://github.com/igorsaux/ChaoticOnyx.Tools/blob/master/src/ChaoticOnyx.Tools.ChangelogGenerator/appsettings.json)
- `ChangelogsFolder` - папка с временными .yml файлами изменении.
- `ChangelogCache` - .yml кэш файл.
- `OutputChangelog` - выходной html файл.
- `DryRun` - тестовый прогон, не удаляет и не изменяет файлы.
- `AutoConvert` - при неудачной попытке парсинга кэша и при значении параметра `true` - пытается подобрать формат кэша и сконвертировать его в новый формат.
- `DateCulture` - выходной формат даты.

### Требования
- [.NET Runtime 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
