#region

using Xunit;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator.Tests
{
    public class ChangelogFormatConverterTests
    {
        [Fact]
        public void VgToChaoticOnyxConverterTest()
        {
            // Arrange
            
            var text = @"author: Unknown
delete-after: True
changes:
  - rscadd: Added feature
  - rscdel: Deleted feature
";

            // Act
            var result = ChangelogFormatConverter.VgToChaoticOnyx(TestingProvider.Deserializer, text);
            
            // Assert
            Assert.True(result.Author == "Unknown");
            Assert.True(result.DeleteAfter);
            Assert.True(result.Changes.Count == 2);
            
            Assert.True(result.Changes[0].Prefix == "rscadd");
            Assert.True(result.Changes[0].Message == "Added feature");
            
            Assert.True(result.Changes[1].Prefix == "rscdel");
            Assert.True(result.Changes[1].Message == "Deleted feature");
        }
    }
}
