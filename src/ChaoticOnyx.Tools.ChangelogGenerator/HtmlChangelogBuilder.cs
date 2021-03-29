#region

using System;
using System.Collections.Generic;
using System.Text;
using Scriban;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
    public class HtmlChangelogBuilder
    {
        private readonly StringBuilder _builder;
        private readonly Template      _template;

        public HtmlChangelogBuilder(string template)
        {
            _template = Template.Parse(template);
            _builder  = new();
        }

        public string Build(IEnumerable<Changelog> changelogs)
        {
            _builder.Clear();

            var context = new
            {
                GeneratingTime = DateTime.Now, Changelogs = changelogs
            };

            _builder.Append(_template.Render(context));

            return _builder.ToString();
        }
    }
}
