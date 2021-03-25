#region

using System;
using System.Globalization;
using System.IO;
using Microsoft.Extensions.Logging;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
    public class Options
    {
        public string ChangelogsFolder
        {
            get;
            set;
        } = string.Empty;

        public string ChangelogCache
        {
            get;
            set;
        } = string.Empty;

        public string OutputChangelog
        {
            get;
            set;
        } = string.Empty;

        public bool DryRun
        {
            get;
            set;
        } = false;

        public bool AutoConvert
        {
            get;
            set;
        } = true;

        public bool GenerateHtml
        {
            get;
            set;
        } = true;

        public string DateCulture
        {
            get;
            set;
        } = CultureInfo.CurrentCulture.ToString();

        public string Template
        {
            get;
            set;
        } = string.Empty;

        public string[] ValidPrefixes
        {
            get;
            set;
        } = Array.Empty<string>();

        public bool CiMode
        {
            get;
            set;
        } = false;

        public void Validate(ILogger? logger)
        {
            var fail = false;
            ChangelogCache   = Path.GetFullPath(ChangelogCache);
            ChangelogsFolder = Path.GetFullPath(ChangelogsFolder);
            OutputChangelog  = Path.GetFullPath(OutputChangelog);
            Template         = Path.GetFullPath(Template);

            if (!Directory.Exists(ChangelogsFolder))
            {
                logger?.LogError($"{ChangelogGeneratorResources.FOLDER_DOES_NOT_EXIST} {ChangelogsFolder}");
                fail = true;
            }

            if (GenerateHtml && string.IsNullOrEmpty(Template))
            {
                logger?.LogError($"{ChangelogGeneratorResources.FILE_DOES_NOT_EXIST} {Template}");
                fail = true;
            }

            if (fail)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
