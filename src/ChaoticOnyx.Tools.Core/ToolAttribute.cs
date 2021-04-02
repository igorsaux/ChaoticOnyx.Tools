using System;
using System.Threading.Tasks;

namespace ChaoticOnyx.Tools.Core
{
	[AttributeUsage(AttributeTargets.Class)]
	public class ToolAttribute : Attribute
	{
		/// <summary>
		///     Описание инструмента. Используется для отображения.
		/// </summary>
		/// <value></value>
		public string Description { get; }

		/// <summary>
		///     Команда для запуска инструмента.
		/// </summary>
		/// <value></value>
		public string RunCommand { get; }

		public ToolAttribute(string description, string runCommand)
		{
			Description = description;
			RunCommand  = runCommand;
		}
	}

	public static class ToolAttributeExtensions
	{
		public static int Run(this ToolAttribute tool, string[] args) { return ToolProvider.RunTool(tool, args); }
	}
}
