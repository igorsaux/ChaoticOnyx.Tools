#region

using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

#pragma warning disable 8618

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
    }
}
