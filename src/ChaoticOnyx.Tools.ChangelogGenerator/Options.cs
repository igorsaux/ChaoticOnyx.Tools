#region

using System.Globalization;
using System.IO;

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

        public string DateCulture
        {
            get;
            set;
        } = CultureInfo.CurrentCulture.ToString();

        public void Validate()
        {
            if (!Directory.Exists(ChangelogsFolder))
            {
                throw new DirectoryNotFoundException(ChangelogsFolder);
            }
        }
    }
}
