#region

using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
    /// <summary>
    ///     Десериализованная информация об изменениях.
    /// </summary>
    public sealed class Changelog
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

        [YamlMember(DefaultValuesHandling = DefaultValuesHandling.OmitDefaults)]
        public bool DeleteAfter
        {
            get;
            set;
        }

        public List<Change> Changes
        {
            get;
            set;
        }

        public Changelog()
        {
            Author      = string.Empty;
            Date        = DateTime.Now;
            DeleteAfter = false;
            Changes     = new();
        }
    }
}
