#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

#endregion

namespace ChaoticOnyx.Tools.ChangelogGenerator
{
    public class HtmlChangelogBuilder
    {
        private readonly StringBuilder                                  _builder;
        private readonly Dictionary<string, Dictionary<string, string>> _contexts;
        private readonly string                                         _dateFormat;
        private readonly IFormatProvider                                _dateFormatProvider;
        private readonly ILogger?                                       _logger;
        private readonly IEnumerable<HtmlTemplate>                      _templates;

        public HtmlChangelogBuilder(IEnumerable<HtmlTemplate> templates, IFormatProvider dateFormatProvider, ILogger? logger = null, string dateFormat = "G")
        {
            _templates          = templates;
            _builder            = new();
            _dateFormatProvider = dateFormatProvider;
            _dateFormat         = dateFormat;
            _logger             = logger;

            _contexts = new()
            {
                {
                    "global", new()
                    {
                        {
                            "generating_time", DateTime.Now.ToString(_dateFormat, _dateFormatProvider)
                        }
                    }
                },
                {
                    "changelog", new()
                    {
                        {
                            "author", string.Empty
                        },
                        {
                            "date", string.Empty
                        },
                        {
                            "changes", string.Empty
                        }
                    }
                },
                {
                    "change", new()
                    {
                        {
                            "prefix", string.Empty
                        },
                        {
                            "message", string.Empty
                        }
                    }
                }
            };
        }

        public string Build(IEnumerable<Changelog> changelogs)
        {
            _builder.Clear();

            BuildTemplate(_templates.First(t => t.Type == "header"), null);

            DateTime?     lastDate       = null;
            StringBuilder changesBuilder = new();
            
            foreach (var changelog in changelogs)
            {
                changesBuilder.Clear();
                _contexts["changelog"]["author"] = changelog.Author;
                _contexts["changelog"]["date"]   = changelog.Date.ToString(_dateFormat, _dateFormatProvider);

                if (lastDate == null || lastDate != changelog.Date)
                {
                    lastDate = changelog.Date;

                    BuildTemplate(_templates.First(t => t.Type == "date"), _contexts["changelog"]);
                }

                foreach (var change in changelog.Changes)
                {
                    _contexts["change"]["prefix"] = change.Prefix;
                    _contexts["change"]["message"] = change.Message;
                    
                    var changeTemplate =
                        BuildTemplateToString(_templates.First(t => t.Type == "change"), _contexts["change"]);

                    changesBuilder.Append(changeTemplate);
                }

                _contexts["changelog"]["changes"] = changesBuilder.ToString();
                BuildTemplate(_templates.First(t => t.Type == "change_body"), _contexts["changelog"]);
            }
            
            BuildTemplate(_templates.First(t => t.Type == "footer"), null);

            return _builder.ToString();
        }

        private string BuildTemplateToString(HtmlTemplate template, Dictionary<string, string>? context)
        {
            var text = template.Text;

            foreach (var (key, value) in _contexts["global"])
            {
                text = text.Replace($"{{{key}}}", $"{value}");
            }
            
            if (context == null)
            {
                return text;
            }
            
            foreach (var (key, value) in context)
            {
                text = text.Replace($"{{{key}}}", $"{value}");
            }

            return text;
        }
        
        private HtmlChangelogBuilder BuildTemplate(HtmlTemplate template, Dictionary<string, string>? context)
        {
            _builder.Append(BuildTemplateToString(template, context));

            return this;
        }
    }
}
