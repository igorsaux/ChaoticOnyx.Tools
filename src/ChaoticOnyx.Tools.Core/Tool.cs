using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChaoticOnyx.Tools.Core
{
	/// <summary>
	///     Абстрактный класс представляющий запускаемый инструмент.
	/// </summary>
	public abstract class Tool
	{
		public abstract int Run();
		public Tool(ILogger logger, IConfiguration configuration) { }
	}
}
