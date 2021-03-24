#region

using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
    /// <summary>
    ///     Конвертеры файлов изменении.
    /// </summary>
    public static class ChangelogFormatConverter
    {
        /// <summary>
        ///     Конвертирует /VG/ формат извенении в нормальный.
        /// </summary>
        public static Changelog VgToChaoticOnyx(IDeserializer deserializer, string text)
        {
            text = text.Replace("delete-after:", "deleteAfter:");
            Changelog result;

            var          parsed  = deserializer.Deserialize<VgChangelog>(text);
            List<Change> changes = new();

            foreach (var e in parsed.Changes)
            {
                foreach (var (prefix, message) in e)
                {
                    changes.Add(new()
                    {
                        Prefix = prefix,
                        Message =  message
                    });
                }
            }

            return new()
            {
                Author = parsed.Author, Date = parsed.Date, DeleteAfter = parsed.DeleteAfter, Changes = changes
            };
        }

        private class VgChangelog
        {
            public string Author
            {
                get;
                set;
            }

            public DateTime Date
            {
                get;
                set;
            }

            public bool DeleteAfter
            {
                get;
                set;
            }

            public List<Dictionary<string, string>> Changes
            {
                get;
                set;
            }
        }
    }
}
