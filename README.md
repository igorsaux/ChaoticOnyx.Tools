# ChaoticOnyx.Tools [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/ChaoticOnyx.Tools?style=for-the-badge)](https://www.nuget.org/packages/ChaoticOnyx.Tools/)
Инструменты заточенные для работы с [OnyxBay](https://github.com/ChaoticOnyx/OnyxBay) но не только.

## Требования
- [.NET Runtime 5.0](https://dotnet.microsoft.com/download/dotnet/5.0/runtime)

## Установка
`dotnet tool install --global ChaoticOnyx.Tools`

## ChaoticOnyx.Tools.ChangelogGenerator
Улучшенная версия старого генератора чейнджлогов с /VG/.

### Возможности
- [x] Создание кэша изменении.
- [x] Генерация списка изменении в виде HTML на основе шаблонов.
- [ ] Поддержка хранения изменении в форматах XML, JSON.

### Настройки
Все возможные настройки перечислены в секции "Options" файла [appsettings.json](https://github.com/igorsaux/ChaoticOnyx.Tools/blob/master/src/ChaoticOnyx.Tools.ChangelogGenerator/appsettings.json)

##  ChaoticOnyx.Tools.StyleCop :construction:
Аналог StyleCop'а но для языка DM. Работает на основе [Hekate](https://github.com/igorsaux/ChaoticOnyx.Hekate).

### Возможности
- [ ] Настройка правил форматирования по файлу с эталонным кодом.
- [ ] Автоформатирование кода.
- [ ] API для работы с кодом.
