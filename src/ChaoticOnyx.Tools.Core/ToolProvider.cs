using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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

			List<Type> toolTypes = assembly.GetTypes()
										   .Where(t => t.IsAbstract == false && t.IsSubclassOf(typeof(Tool)))
										   .ToList();

			foreach (var type in toolTypes)
			{
				var attribute = type.GetCustomAttribute<ToolAttribute>();

				if (attribute == null) { continue; }

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

		public static int RunTool(ToolAttribute tool, string[]? args = null)
		{
			Type toolType     = s_tools[tool];
			var  toolInstance = (Tool?) Activator.CreateInstance(toolType);

			if (toolInstance == null) { throw new InvalidOperationException($"Can't instantiate {toolType}"); }

			toolInstance.Configure(args);

			return toolInstance.Run();
		}
	}
}
