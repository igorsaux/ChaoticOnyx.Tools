using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChaoticOnyx.Tools.Core
{
	/// <summary>
	///     Поставщик инструментов.
	/// </summary>
	public static class ToolProvider
	{
		private static readonly Dictionary<ToolAttribute, Type> s_tools = new();

		public static ReadOnlyCollection<ToolAttribute> Tools { get; }

		/// <summary>
		///     Собирает все инструменты в сборке.
		/// </summary>
		static ToolProvider()
		{
			var assembly = Assembly.GetAssembly(typeof(Tool));
			Debug.Assert(assembly != null);

			var toolTypes = assembly.GetTypes()
									.Where(t => t.IsAbstract == false && t.IsSubclassOf(typeof(Tool)))
									.ToList();

			foreach (var type in toolTypes)
			{
				ToolAttribute? attribute = type.GetCustomAttribute<ToolAttribute>();

				if (attribute == null)
				{
					continue;
				}

				s_tools.Add(attribute, type);
			}

			Tools = s_tools.Keys.ToList()
						   .AsReadOnly();
		}

		/// <summary>
		///     Возвращает инструмент запускаемый указанной командой.
		/// </summary>
		/// <param name="command">Команда запуска инструмента.</param>
		/// <returns>Возвращает null если инструмент не найден.</returns>
		public static ToolAttribute? GetTool(string command)
		{
			return Tools.FirstOrDefault(t => t.RunCommand == command);
		}

		public static int RunTool(ToolAttribute tool, string[] args)
		{
			Type   toolType     = s_tools[tool];
			string toolTypeName = toolType.Name;

			IHost? host = Host.CreateDefaultBuilder(args)
							  .ConfigureHostConfiguration(c => c.AddJsonFile($"{toolTypeName}.json"))
							  .Build();

			ILoggerFactory? loggerFactory = host.Services.GetService<ILoggerFactory>();
			IConfiguration? configuration = host.Services.GetService<IConfiguration>();
			Debug.Assert(loggerFactory != null);
			Debug.Assert(configuration != null);
			ILogger?        logger        = loggerFactory.CreateLogger(toolTypeName);
			var             toolInstance  = (Tool) Activator.CreateInstance(toolType, logger, configuration);
			Debug.Assert(toolInstance != null);

			return toolInstance.Run();
		}
	}
}
