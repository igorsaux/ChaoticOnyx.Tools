#region

using System.Collections.Generic;
using System.IO;
using Xunit;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator.Tests
{
    public class HtmlChangelogBuilderTests
    {
        [Fact]
        public void SimpleHtmlChangelogBuildTest()
        {
            // Arrange
            var expected =
                @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
<html>
    <head>
        <title>Changelog for 25 Mar 2021</title>
        <meta http-equiv=""Content-Type"" content=""text/html""; charset=""utf-8"">
    </head>

    <body>
        <h2 class=""date"">02 Feb 2020</h2>
        <h3 class=""author"">Unknown updated:</h3>
        <ul class=""changes bgimages16"">
            <li class=""rscadd"">Added feature</li>
            <li class=""rscdel"">Deleted bug</li>
        </ul>
        <h3 class=""author"">Unknown updated:</h3>
        <ul class=""changes bgimages16"">
            <li class=""rscadd"">Added feature 2</li>
            <li class=""rscdel"">Deleted bug 2</li>
        </ul>
        <h2 class=""date"">03 Feb 2020</h2>
        <h3 class=""author"">UnknownSecond updated:</h3>
        <ul class=""changes bgimages16"">
            <li class=""rscadd"">Added yet another feature</li>
        </ul>

        <h2>Changelog for 25 Mar 2021</h2>

    </body>
</html>";

            var builder = new HtmlChangelogBuilder(
                File.ReadAllText($"{TestingProvider.SamplesFolder}test_template.tmpl"));

            List<Changelog> changelogs = new()
            {
                new()
                {
                    Author = "Unknown",
                    Date   = new(2020, 02, 02),
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
                },
                new()
                {
                    Author = "Unknown",
                    Date   = new(2020, 02, 02),
                    Changes = new()
                    {
                        new()
                        {
                            Prefix = "rscadd", Message = "Added feature 2"
                        },
                        new()
                        {
                            Prefix = "rscdel", Message = "Deleted bug 2"
                        }
                    }
                },
                new()
                {
                    Author = "UnknownSecond",
                    Date   = new(2020, 02, 03),
                    Changes = new()
                    {
                        new()
                        {
                            Prefix = "rscadd", Message = "Added yet another feature"
                        }
                    }
                }
            };

            // Act
            var result = builder.Build(changelogs);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
