#region

using System;
using System.IO;
using Xunit;
using YamlDotNet.Core;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator.Tests
{
    public class CacheParserTests : IDisposable
    {
        private readonly string _cacheFile         = ".all_changelog.yml";
        private readonly string _changelogsFolder  = TestingProvider.SamplesFolder;
        private readonly string _oldCacheFile      = ".old_changelog.yml";
        private readonly string _unknownFormatFile = ".unknown.yml";

        [Fact]
        public void CorrectParsingTest()
        {
            // Arrange
            var file   = $"{_changelogsFolder}{_cacheFile}";
            var parser = new CacheParser(TestingProvider.Deserializer, TestingProvider.Serializer);

            // Act
            var result = parser.ParseCacheFile(file);

            // Assert
            Assert.True(result.Count == 2);

            Assert.True(result[0]
                            .Author == "Unknown");

            Assert.True(result[1]
                            .Author == "UnknownSecond");

            Assert.True(result[0]
                        .Changes.Count == 2);

            Assert.True(result[1]
                        .Changes.Count == 2);
        }

        [Fact]
        public void IncorrectFormatTest()
        {
            // Arrange
            var file   = $"{_changelogsFolder}{_oldCacheFile}";
            var parser = new CacheParser(TestingProvider.Deserializer, TestingProvider.Serializer);

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
            var parser = new CacheParser(TestingProvider.Deserializer, TestingProvider.Serializer, true);

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
            var parser = new CacheParser(TestingProvider.Deserializer, TestingProvider.Serializer, true);
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
