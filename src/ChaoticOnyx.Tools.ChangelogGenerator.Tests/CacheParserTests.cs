using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Xunit;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.NamingConventions;

namespace ChaoticOnyx.Tools.ChangelogGenerator.Tests
{
    public class CacheParserTests : IDisposable
    {
        private readonly string            _changelogsFolder  = Path.GetFullPath("./samples/");
        private readonly string            _cacheFile         = ".all_changelog.yml";
        private readonly string            _oldCacheFile      = ".old_changelog.yml";
        private readonly string            _unknownFormatFile = ".unknown.yml";
        private readonly DateTimeConverter _dateTimeConverter;
        private readonly IDeserializer     _deserializer;
        private readonly ISerializer       _serializer;
        
        public CacheParserTests()
        {
            List<string> formats = new();
            
            formats.AddRange(CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns());
            formats.AddRange(CultureInfo.InvariantCulture.DateTimeFormat.GetAllDateTimePatterns());

            _dateTimeConverter = new(provider: CultureInfo.InvariantCulture, formats: formats.ToArray());
            
            _deserializer = new DeserializerBuilder().WithTypeConverter(_dateTimeConverter)
                                                     .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                     .Build();

            _serializer = new SerializerBuilder().WithTypeConverter(_dateTimeConverter)
                                                 .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                 .Build();
        }
        
        [Fact]
        public void CorrectParsingTest()
        {
            // Arrange
            var file   = $"{_changelogsFolder}{_cacheFile}";
            var parser = new CacheParser(_deserializer, _serializer);

            // Act
            var result = parser.ParseCacheFile(file);
            
            // Assert
            Assert.True(result.Count == 2);
            
            Assert.True(result[0].Author == "Unknown");
            Assert.True(result[1].Author == "UnknownSecond");
            
            Assert.True(result[0].Changes.Count == 2);
            Assert.True(result[1].Changes.Count == 2);
        }

        [Fact]
        public void IncorrectFormatTest()
        {
            // Arrange
            var file   = $"{_changelogsFolder}{_oldCacheFile}";
            var parser = new CacheParser(_deserializer, _serializer);
            
            // Act
            void Code() => parser.ParseCacheFile(file);
            
            // Assert
            Assert.Throws<YamlException>(Code);
        }

        [Fact]
        public void ConvertFormatTest()
        {
            // Arrange
            var file   = $"{_changelogsFolder}{_oldCacheFile}";
            var parser = new CacheParser(_deserializer, _serializer, true);
            
            // Act
            var result = parser.ParseCacheFile(file);
            
            // Assert
            Assert.True(result.Count == 2);
        }

        [Fact]
        public void FailConvertTest()
        {
            // Arrange
            var file   = $"{_changelogsFolder}{_unknownFormatFile}";
            var parser = new CacheParser(_deserializer, _serializer, true);
            
            File.WriteAllText(file, "hello, world!");
            
            // Act
            void Code() => parser.ParseCacheFile(file);
            
            // Assert
            Assert.Throws<YamlException>(Code);
        }

        public void Dispose()
        {
            var file = $"{_changelogsFolder}{_unknownFormatFile}";
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}
