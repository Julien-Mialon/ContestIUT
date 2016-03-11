using System;

namespace Contest
{
	public static class Logger
	{
		public enum Level
		{
			Debug = 0,
			Info = 1,
			Warning = 2,
			Error = 4,
			Critical = 8,
			None = 16
		}

		public static Action<Level, string> LogAction { get; set; }
		public static Level LogLevel { get; set; }

		static Logger()
		{
			LogLevel = Level.Debug;
		}

		public static void Initialize(Level level)
		{
			LogLevel = level;
		}

		public static void Info(string message)
		{
			Log(Level.Info, message);
		}

		public static void Debug(string message)
		{
			Log(Level.Debug, message);
		}

		public static void Warning(string message)
		{
			Log(Level.Warning, message);
		}

		public static void Error(string message)
		{
			Log(Level.Error, message);
		}

		public static void Critical(string message)
		{
			Log(Level.Critical, message);
		}

		public static void Log(Level level, string message)
		{
			if ((int)level < (int)LogLevel)
			{
				return;
			}
			
			Console.WriteLine($"[{level} {DateTime.Now.ToString("HH:mm:ss")}] : {message}");
		}
	}
}