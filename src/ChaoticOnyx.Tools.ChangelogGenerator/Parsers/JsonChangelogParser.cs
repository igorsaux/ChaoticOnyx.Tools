using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;

namespace ChaoticOnyx.Tools.ChangelogGenerator.Parsers
{
	[ChangelogParser(".json")]
	public class JsonChangelogParser : ChangelogParser
	{
		private readonly JsonSerializerOptions _options;

		public JsonChangelogParser()
		{
			_options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
		}

		public override Changelog Parse(string text)
		{
			var result = JsonSerializer.Deserialize<Changelog>(text, _options);
			Debug.Assert(result != null);

			return result!;
		}

		public override ICollection<Changelog> ParseCache(string text)
		{
			var result = JsonSerializer.Deserialize<ICollection<Changelog>>(text, _options);
			Debug.Assert(result != null);

			return result;
		}

		public override string SerializeCache(ICollection<Changelog> cache)
		{
			return JsonSerializer.Serialize(cache, _options);
		}
	}
}
