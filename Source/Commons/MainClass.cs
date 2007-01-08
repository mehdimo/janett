namespace Janett.Commons
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Runtime.InteropServices;

	internal class MainClass
	{
		private static Commandlet commandlet;

		[DllImport("kernel32.dll")]
		private static extern bool SetConsoleCtrlHandler(ControlEventHandler e, bool add);

		public enum ConsoleEvent
		{
			CTRL_C = 0,
			CTRL_BREAK = 1,
			CTRL_CLOSE = 2,
			CTRL_LOGOFF = 5,
			CTRL_SHUTDOWN = 6
		}

		public delegate void ControlEventHandler(ConsoleEvent consoleEvent);

		private static int Main(string[] args)
		{
			ControlEventHandler eventHandler = new ControlEventHandler(Handler);
			SetConsoleCtrlHandler(eventHandler, true);

			Discovery dis = new Discovery();
			Assembly assembly = Assembly.GetExecutingAssembly();
			string directory = Path.GetDirectoryName(assembly.Location);
			foreach (string file in Directory.GetFiles(directory, "*.dll"))
			{
				dis.AddAssembly(file);
			}
			string exeName = Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().CodeBase);
			Type commandType = dis.GetClass(typeof(NamedAttribute), new NamedAttribute(exeName));

			if (commandType == null)
			{
				WriteOutput(string.Format("Coult not find Command '{0}' to execute", exeName));
				return -4;
			}
			CommandLineParser parser = new CommandLineParser(commandType, args);
			if (args.Length == 0 || args[0] == "/?" || args[0] == "--help")
			{
				Console.WriteLine(parser.Usage);
				return -1;
			}
			if (args.Length == 1 && (args[0] == "/v" || args[0] == "-v" || args[0] == "--v"))
			{
				Version ver = Assembly.GetExecutingAssembly().GetName().Version;
				Console.WriteLine(exeName + " version " + ver.ToString());
				return 0;
			}
			try
			{
				ICommand com = (ICommand) parser.Parse();
				if (com is Commandlet)
				{
					commandlet = (Commandlet) com;
					commandlet.ExecutableName = exeName;
					commandlet.ExecutableDirectory = directory;
				}
				com.Execute();
				if (com.Outputs != null)
				{
					foreach (object res in com.Outputs)
					{
						WriteOutput(res.ToString());
					}
				}
				if (com.Result != null)
					return Int32.Parse(com.Result);
				else
					return 0;
			}
			catch (CommandLineArgumentException ex)
			{
				WriteOutput(ex.Message + "\r\n");
				WriteOutput(parser.Usage);
				return -1;
			}
			catch (ApplicationException ex)
			{
				WriteOutput(ex.Message);
				return -3;
			}
			catch (Exception ex)
			{
				commandlet.Exception(ex);
				while (ex != null)
				{
					WriteOutput(ex.Message + "\r\n" + ex.StackTrace);
					ex = ex.InnerException;
				}
				return -2;
			}
		}

		private static void Handler(ConsoleEvent consoleEvent)
		{
			commandlet.Terminate();
			Process.GetCurrentProcess().Kill();
		}

		private static void WriteOutput(string output)
		{
			Console.WriteLine(output);
			System.Diagnostics.Debug.WriteLine(output);
		}
	}
}