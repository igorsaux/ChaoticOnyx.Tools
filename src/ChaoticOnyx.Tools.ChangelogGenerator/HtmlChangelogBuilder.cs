#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Scriban;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
    public class HtmlChangelogBuilder
    {
        private readonly StringBuilder   _builder;
        private readonly string          _dateFormat;
        private readonly IFormatProvider _dateFormatProvider;
        private readonly ILogger?        _logger;
        private readonly Template        _template;

        public HtmlChangelogBuilder(string          template,
                                    IFormatProvider dateFormatProvider,
                                    ILogger?        logger     = null,
                                    string          dateFormat = "G")
        {
            _template           = Template.Parse(template);
            _builder            = new();
            _dateFormatProvider = dateFormatProvider;
            _dateFormat         = dateFormat;
            _logger             = logger;
        }

        public string Build(IEnumerable<Changelog> changelogs)
        {
            _builder.Clear();

            var context = new
            {
                GeneratingTime = DateTime.Now,
                Changelogs = changelogs
            };

            _builder.Append(_template.Render(context));

            return _builder.ToString();
        }
    }
}
