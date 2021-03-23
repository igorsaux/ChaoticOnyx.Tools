using System;
using System.Collections.Generic;
using Xunit;

namespace ChaoticOnyx.Tools.ChangelogGenerator.Tests
{
    public class CacheMergerTests
    {
        [Fact]
        public void MergeTest()
        {
            // Arrange
            var cache = new List<Changelog>()
            {
                new()
                {
                    Author = "Unknown",
                    Date   = DateTime.Now,
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
                    Author = "UnknownSecond",
                    Date = DateTime.Now.AddDays(1),
                    Changes = new()
                    {
                        new()
                        {
                            Prefix = "rscadd", Message = "Fixes"
                        }
                    }
                },
                new()
                {
                    Author = "Unknown",
                    Date = DateTime.Now,
                    Changes = new()
                    {
                        new()
                        {
                            Prefix = "rscadd", Message = "And another feature!"
                        },
                        new()
                        {
                            Prefix = "rscadd", Message = "Added feature"
                        }
                    }
                },
                new()
                {
                    Author = "Unknown",
                    Date = DateTime.Now.AddDays(2),
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
            };
            
            // Act
           var result = CacheMerger.Merge(cache);
           
           // Assert
           Assert.True(result.Count == 3);
           Assert.True(result[0].Changes.Count == 3);
           Assert.True(result[1].Changes.Count == 1);
           Assert.True(result[2].Changes.Count == 2);
        }
    }
}
