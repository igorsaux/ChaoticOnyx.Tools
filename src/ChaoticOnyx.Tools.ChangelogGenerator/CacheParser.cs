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
        private readonly ILogger?      _logger;
        private readonly ISerializer   _serializer;
        private readonly bool          _doConvert;

        public CacheParser(IDeserializer deserializer, ISerializer serializer, bool doConvert = false)
        {
            _deserializer = deserializer;
            _serializer   = serializer;
            _doConvert    = doConvert;
        }

        public CacheParser(IDeserializer deserializer, ISerializer serializer, bool doConvert, ILogger logger)
            : this(deserializer, serializer, doConvert)
        {
            _logger = logger;
        }

        /// <summary>
        /// Пасринг кэш файла.
        /// </summary>
        /// <param name="fullPath">Путь к файлу кэша.</param>
        /// <returns></returns>
        /// <exception cref="YamlException"></exception>
        public List<Changelog> ParseCacheFile(string fullPath)
        {
            List<Changelog> result;
            var             text   = File.ReadAllText(fullPath);

            try
            {
                _logger?.LogInformation(
                    $"{ChangelogGeneratorResources.PARSING_CACHE_FILE} {Path.GetFileName(fullPath)}");

                result = _deserializer.Deserialize<List<Changelog>>(text);
            }
            catch (YamlException e)
            {
                _logger?.LogError($"{ChangelogGeneratorResources.PARSING_ERROR} {Path.GetFileName(fullPath)}");
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
                        _logger?.LogInformation($"{ChangelogGeneratorResources.CACHE_CONVERTED}");
                        
                        return result;
                    }
                    catch (YamlException) { }
                }
                
                _logger?.LogError($"{ChangelogGeneratorResources.CANT_CONVERT_CACHE}");

                throw;
            }

            return result;
        }
    }
}
