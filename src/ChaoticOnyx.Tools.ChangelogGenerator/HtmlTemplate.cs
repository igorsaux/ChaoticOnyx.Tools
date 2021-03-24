#region

using System.IO;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
    public class HtmlTemplate
    {
        public static readonly string[] ValidTypes =
        {
            "header", "footer", "date", "change_body", "change"
        };

        public string Type
        {
            get;
            init;
        }

        public string Path
        {
            get;
            set;
        }

        public string Text
        {
            get;
            private set;
        } = string.Empty;

        public void Load()
        {
            Text = File.ReadAllText(Path);
        }
    }
}
