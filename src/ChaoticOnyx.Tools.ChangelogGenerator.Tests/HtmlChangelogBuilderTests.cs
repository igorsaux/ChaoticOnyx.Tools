#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            var expected  = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN"" ""http://www.w3.org/TR/html4/loose.dtd"">
<html>
<head>
<title>Changelog for 24.03.2021</title>
<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
</head>

<body>
<h2 class=""date"">02.02.2020</h2>
<h3 class=""author"">Unknown updated:</h3>
<ul class=""changes bgimages16"">
<li class=""rscadd"">Added feature</li>
<li class=""rscdel"">Delete bug</li>

</ul>
<h2>Changelog for 24.03.2021</h2>
</body>
</html>
";
            
            var builder   = new HtmlChangelogBuilder(TestingProvider.HtmlTemplates, CultureInfo.InvariantCulture, null, "dd.MM.yyy");

            List<Changelog> changelogs = new()
            {
                new()
                {
                    Author = "Unknown",
                    Date   = new DateTime(2020, 02, 02),
                    Changes = new()
                    {
                        new()
                        {
                            Prefix = "rscadd", Message = "Added feature"
                        },
                        new()
                        {
                            Prefix = "rscdel", Message = "Delete bug"
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
