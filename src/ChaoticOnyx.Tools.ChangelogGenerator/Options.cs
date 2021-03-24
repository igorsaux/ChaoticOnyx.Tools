#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
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

        public string ChangelogDateFormat
        {
            get;
            set;
        } = "G";
        
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

        public string TemplatesFolder
        {
            get;
            set;
        } = string.Empty;

        public List<HtmlTemplate> Templates
        {
            get;
            set;
        } = new();

        public void Validate(ILogger? logger)
        {
            var fail = false;
            ChangelogCache   = Path.GetFullPath(ChangelogCache, AppContext.BaseDirectory);
            ChangelogsFolder = Path.GetFullPath(ChangelogsFolder, AppContext.BaseDirectory);
            OutputChangelog  = Path.GetFullPath(OutputChangelog, AppContext.BaseDirectory);
            TemplatesFolder  = Path.GetFullPath(TemplatesFolder, AppContext.BaseDirectory);
            
            if (!Directory.Exists(ChangelogsFolder))
            {
                logger?.LogError($"{ChangelogGeneratorResources.FILE_DOES_NOT_EXIST} {ChangelogsFolder}");
                fail = true;
            }

            if (!Directory.Exists(TemplatesFolder))
            {
                logger?.LogError($"{ChangelogGeneratorResources.FOLDER_DOES_NOT_EXIST} {ChangelogsFolder}");
                fail = true;
            }

            if (GenerateHtml && Templates.Count == 0)
            {
                logger?.LogError($"{ChangelogGeneratorResources.NO_HTML_TEMPLATES}");
                fail = true;
            }
            
            var types = Templates.Select(t => t.Type);

            foreach (var validType in HtmlTemplate.ValidTypes)
            {
                if (!types.Contains(validType))
                {
                    logger?.LogError($"{ChangelogGeneratorResources.TEMPLATE_TYPE_NOT_FOUND} {validType}");
                    fail = true;
                }
            }

            foreach (var template in Templates)
            {
                if (!HtmlTemplate.ValidTypes.Contains(template.Type))
                {
                    logger?.LogError($"{ChangelogGeneratorResources.TEMPLATE_TYPE_ARE_NOT_VALID} {template.Type}");
                    fail = true;
                }

                template.Path = Path.GetFullPath($"{TemplatesFolder}{template.Path}", AppContext.BaseDirectory);

                template.Load();
            }

            if (fail)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
