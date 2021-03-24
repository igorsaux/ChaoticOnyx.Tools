#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using YamlDotNet.Core;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator.Tests
{
    public class ChangelogParsingTests : IDisposable
    {
        private readonly string _changelogsFolder = TestingProvider.ChangelogsFolder;
        private readonly string _tempFile         = "out1.yml";

        [Fact]
        public void ParsingTextTest()
        {
            // Arrange
            var parser = new ChangelogParser(TestingProvider.Deserializer, TestingProvider.Serializer);
            string valid = @"
author: Unknown
date: ""2018-02-27""
deleteAfter: True
changes:
  - prefix: rscadd
    message: ""Added a changelog editing system that should cause fewer conflicts and more accurate timestamps.""
  - prefix: rscdel
    message: ""Killed innocent kittens.""
";

            // Act
            var result = parser.ParseText(valid);
            
            // Assert
            Assert.True(result.Author == "Unknown");
            Assert.True(result.Date.Date == new DateTime(2018, 02, 27).Date);
            Assert.True(result.DeleteAfter);
            Assert.True(result.Changes.Count == 2);
            
            Assert.True(result.Changes[0].Prefix == "rscadd");
            Assert.True(result.Changes[0].Message == "Added a changelog editing system that should cause fewer conflicts and more accurate timestamps.");
            Assert.True(result.Changes[1].Prefix == "rscdel");
            Assert.True(result.Changes[1].Message == "Killed innocent kittens.");
        }

        [Fact]
        public void ParsingFileTest()
        {
            // Arrange
            var parser = new ChangelogParser(TestingProvider.Deserializer, TestingProvider.Serializer);
            var file   = Path.GetFullPath("test.yml", _changelogsFolder);
            
            // Act
            var result = parser.ParseFile(file);
            
            // Assert
            Assert.True(result.Author == "Unknown");
            Assert.True(result.DeleteAfter);
            Assert.True(result.Date.Date == new DateTime(2018, 02, 27).Date);
            Assert.True(result.Changes.Count == 2);
            
            Assert.True(result.Changes[0].Prefix == "rscadd");
            Assert.True(result.Changes[0].Message == "Added features");
            Assert.True(result.Changes[1].Prefix == "rscdel");
            Assert.True(result.Changes[1].Message == "Deleted bugs");
        }

        [Fact]
        public void ParsingFolderTest()
        {
            // Arrange
            var parser = new ChangelogParser(TestingProvider.Deserializer, TestingProvider.Serializer);
            
            // Act
            var result = parser.ParseFolder(_changelogsFolder);
            
            // Assert
            Assert.True(result.Count == 2);
        }

        [Fact]
        public void SavingChangelogsTest()
        {
            // Arrange
            var file = $"{_changelogsFolder}{_tempFile}";

            var parser = new ChangelogParser(TestingProvider.Deserializer, TestingProvider.Serializer);

            var changelogs = new Dictionary<string, Changelog>
            {
                {
                    file, new()
                    {
                        Author      = "Unknown",
                        DeleteAfter = false,
                        Date        = new DateTime(2021, 04, 24).AddHours(1),
                        Changes = new()
                        {
                            new()
                            {
                                Prefix = "rscadd", Message = "Added feature"
                            },
                            new()
                            {
                                Prefix = "rscdel", Message = "Deleted bug"
                            }
                        }
                    }
                }
            };

            // Act
            parser.SaveChangelogs(changelogs);
            var parsed   = parser.ParseFile(changelogs.Keys.First(k => k == file));
            var original = changelogs[file];
            
            // Assert
            Assert.True(File.Exists(file));
            Assert.True(original.Author == parsed.Author);
            Assert.True(original.Changes.Count == parsed.Changes.Count);
            Assert.True(original.Date.Date == parsed.Date.Date);
        }

        [Fact]
        public void InvalidFileParsingTest()
        {
            // Arrange
            var file   = $"{_changelogsFolder}{_tempFile}";
            File.WriteAllText(file, "hello, world!");
            var parser = new ChangelogParser(TestingProvider.Deserializer, TestingProvider.Serializer);

            // Act
            void Code() => parser.ParseFolder(_changelogsFolder);
            
            // Assert
            Assert.Throws<YamlException>(Code);
        }

        public void Dispose()
        {
            var file = $"{_changelogsFolder}{_tempFile}";
            
            if (File.Exists(file))
            {
                File.Delete(file);
            }
        }
    }
}
