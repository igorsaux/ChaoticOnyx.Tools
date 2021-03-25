#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.Converters;
using YamlDotNet.Serialization.NamingConventions;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator.Tests
{
    public static class TestingProvider
    {
        public static readonly string            SamplesFolder = Path.GetFullPath("./samples/");
        public static readonly DateTimeConverter DateTimeConverter;
        public static readonly IDeserializer     Deserializer;
        public static readonly ISerializer       Serializer;

        static TestingProvider()
        {
            List<string> formats = new();
            formats.AddRange(CultureInfo.CurrentCulture.DateTimeFormat.GetAllDateTimePatterns());
            formats.AddRange(CultureInfo.InvariantCulture.DateTimeFormat.GetAllDateTimePatterns());
            DateTimeConverter = new(DateTimeKind.Local, CultureInfo.InvariantCulture, formats.ToArray());

            Deserializer = new DeserializerBuilder().WithTypeConverter(DateTimeConverter)
                                                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                    .Build();

            Serializer = new SerializerBuilder().WithTypeConverter(DateTimeConverter)
                                                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                                .Build();
        }
    }
}
