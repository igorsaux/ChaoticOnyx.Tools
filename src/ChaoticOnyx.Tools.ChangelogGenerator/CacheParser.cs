#region

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
    public class CacheParser
    {
        /// <summary>
        ///     Список конвертеров кэша.
        /// </summary>
        private static readonly Func<IDeserializer, string, List<Changelog>>[] s_converters =
        {
            CacheFormatConverter.VgToChaoticOnyx
        };

        private readonly IDeserializer _deserializer;
        private readonly bool          _doConvert;
        private readonly ILogger?      _logger;

        public CacheParser(IDeserializer deserializer, bool doConvert = false)
        {
            _deserializer = deserializer;
            _doConvert    = doConvert;
        }

        public CacheParser(IDeserializer deserializer, ILogger logger, bool doConvert = false) : this(
            deserializer, doConvert)
        {
            _logger = logger;
        }

        /// <summary>
        ///     Пасринг кэш файла.
        /// </summary>
        /// <param name="fullPath">Путь к файлу кэша.</param>
        /// <returns></returns>
        /// <exception cref="YamlException"></exception>
        public List<Changelog> ParseCacheFile(string fullPath)
        {
            List<Changelog> result;
            var             text = File.ReadAllText(fullPath);

            try
            {
                _logger?.LogInformation(
                    $"{ChangelogGeneratorResources.PARSING_CACHE_FILE} {Path.GetFileName(fullPath)}");

                result = _deserializer.Deserialize<List<Changelog>>(text);
            }
            catch (YamlException e)
            {
                _logger?.LogWarning(
                    $"{ChangelogGeneratorResources.PARSING_ERROR} {e.InnerException?.Message ?? e.Message}");

                _logger?.LogTrace(e, e.Message);

                if (!_doConvert)
                {
                    throw;
                }

                foreach (var converter in s_converters)
                {
                    _logger?.LogWarning($"{ChangelogGeneratorResources.TRYING_TO_CONVERT} {{{converter.Method.Name}}}");

                    try
                    {
                        result = converter.Invoke(_deserializer, text);
                        _logger?.LogInformation($"{ChangelogGeneratorResources.FILE_CONVERTED}");

                        return result;
                    }
                    catch (YamlException) { }
                }

                _logger?.LogError($"{ChangelogGeneratorResources.CANT_CONVERT_FILE}");

                throw;
            }

            return result;
        }
    }
}
