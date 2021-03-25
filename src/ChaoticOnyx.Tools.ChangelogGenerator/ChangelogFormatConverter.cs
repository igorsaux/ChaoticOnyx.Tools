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
            var          parsed  = deserializer.Deserialize<VgChangelog>(text);
            List<Change> changes = new();

            foreach (var e in parsed.Changes)
            {
                foreach (var (prefix, message) in e)
                {
                    changes.Add(new()
                    {
                        Prefix = prefix, Message = message
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
                init;
            }

            public DateTime Date
            {
                get;
                init;
            }

            public bool DeleteAfter
            {
                get;
                init;
            }

            public List<Dictionary<string, string>> Changes
            {
                get;
                init;
            }

            public VgChangelog()
            {
                Author      = string.Empty;
                Date        = DateTime.Now;
                DeleteAfter = false;
                Changes     = new();
            }
        }
    }
}
