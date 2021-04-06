using System;

namespace ChaoticOnyx.Tools.ChangelogGenerator.Parsers
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ChangelogParserAttribute : Attribute
	{
		public string Extension { get; }

		public ChangelogParserAttribute(string extension) { Extension = extension; }
	}
}
