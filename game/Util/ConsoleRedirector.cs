using System.Diagnostics;
using System.IO;
using System.Text;

namespace ShGame.game.Util
{
	public class ConsoleRedirector
	{
		private readonly string LogDirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

		private static ConsoleRedirector _outputSingleton;
		private static ConsoleRedirector OutputSingleton {
			get
			{
				if (_outputSingleton == null) {
					_outputSingleton = new ConsoleRedirector();
				}
				return _outputSingleton;
			}
		}

		static ConsoleRedirector(){
			
		}

		public StreamWriter SW { get; set; }

		public ConsoleRedirector() {
			EnsureLogDirectoryExists();
			InstantiateStreamWriter();
		}

		~ConsoleRedirector()
		{
			if (SW != null)
			{
				try
				{
					SW.Dispose();
				}
				catch (ObjectDisposedException) { } // object already disposed - ignore exception
			}
		}

		public static void WriteLine(string str)
		{
			Console.WriteLine(str);
			OutputSingleton.SW.WriteLine(str);
		}

		public static void Write(string str)
		{
			Console.Write(str);
			OutputSingleton.SW.Write(str);
		}

		private void InstantiateStreamWriter()
		{
			string filePath = Path.Combine(LogDirPath, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")) + ".txt";
			try
			{
				SW = new StreamWriter(filePath);
				SW.AutoFlush = true;
			}
			catch (UnauthorizedAccessException ex)
			{
				throw new ApplicationException(string.Format("Access denied. Could not instantiate StreamWriter using path: {0}.", filePath), ex);
			}
		}

		private void EnsureLogDirectoryExists()
		{
			if (!Directory.Exists(LogDirPath))
			{
				try
				{
					Directory.CreateDirectory(LogDirPath);
				}
				catch (UnauthorizedAccessException ex)
				{
					throw new ApplicationException(string.Format("Access denied. Could not create log directory using path: {0}.", LogDirPath), ex);
				}
			}
		}
	}
}
