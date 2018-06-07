using PipeShot.UI.Properties;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Shell;
using System.Collections.Generic;


namespace PipeShot.UI {
	/// <summary>
	///     Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application, ISingleInstanceApp
	{
		private const string Unique = "PipeShot_UI_UID";

		[STAThread]
		public static void Main() {
			if (SingleInstance<App>.InitializeAsFirstInstance(Unique))
			{
				var application = new App();

				application.InitializeComponent();
				application.Run();

				// Allow single instance code to perform cleanup operations
				SingleInstance<App>.Cleanup();
			}
		}

		public App() {
			DispatcherUnhandledException += UnhandledException;

			LoadConfig();
			LoadLanguage();
		}

		private static void LoadLanguage() {
			string language = ConfigHelper.Language;

			//If language is not yet set manually, select system default
			if (!string.IsNullOrWhiteSpace(language)) {
				Thread.CurrentThread.CurrentCulture = new CultureInfo(language);
				Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
				FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(
					XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
			} else {
				ConfigHelper.Language = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
				ConfigHelper.Save();
			}
		}

		//Load from config.json
		private static void LoadConfig() {
			ConfigHelper.Exists();
			ConfigHelper.JsonConfig = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(ConfigHelper.ConfigFile));
		}

		//Unhandled Exception User Message Boxes
		private static void UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
			WriteError(e.Exception);

			if (MessageBox.Show(string.Format(strings.unhandledErrorDescription, e.Exception.Message),
				"PipeShot Error",
				MessageBoxButton.YesNo,
				MessageBoxImage.Error) == MessageBoxResult.Yes) {
				try {
					//Restart with same args
					string[] argumentsArray = Environment.GetCommandLineArgs();
					string arguments = argumentsArray.Aggregate("", (current, arg) => current + (arg.Contains(" ") ? "\"" + arg + "\" " : arg + " "));
					string exePath = Assembly.GetEntryAssembly().Location;

					Process.Start(exePath, arguments);
				} catch {
					// ignored
				}
			} else {
				Current.Shutdown();
				Thread.Sleep(1000);
				Process.GetCurrentProcess().Kill();
				return;
			}
		}


		private static void WriteError(Exception ex) {
			try {
				string errorFile = Path.Combine(ConfigHelper.ConfigPath, "ui.error.txt");
				string nl = Environment.NewLine;
				string errorDetails = $"!PipeShot.UI Error @{DateTime.Now}" + nl +
						$"    Error Message: {ex.Message}" + nl + nl +
						$"    Error Stacktrace: {ex.StackTrace}";

				if (File.Exists(errorFile)) {
					File.AppendAllText(errorFile,
						nl + nl + "---------------------------------------------------------" + errorDetails);
				} else {
					File.WriteAllText(errorFile,
						$"Details for an Exception in PipeShot.UI. " +
						"You can tell me about this error on http://www.github.com/mrousavy/PipeShot/issues so I can fix it as soon as possible!"
						+ nl + errorDetails);
				}
			} catch { }
		}

		#region ISingleInstanceApp Members

		public bool SignalExternalCommandLineArgs(IList<string> args)
		{
			// handle command line arguments of second instance
			// …
			return true;
		}

		#endregion
	}
}