using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using str = PipeShot.UI.Properties.strings;

namespace PipeShot.UI {
    public partial class MainWindow {
        #region Fields

        public static InstallerHelper Helper;

        //Path to Documents/PipeShot Folder
        private static string DocPath {
            get {
                string value = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "PipeShot");
                return value;
            }
        }

        #endregion

        //Constructor
        public MainWindow() {
            InitializeComponent();

            //Update Loading Indicator
            ShowProgressIndicator();
            SetProgressStatus(str.initializing);

            //Check for Commandline Arguments
            Arguments();

            //Create Documents\PipeShot Path
            if (!Directory.Exists(DocPath)) {
                Directory.CreateDirectory(DocPath);
            }

            //Initialize Helpers
            Helper = new InstallerHelper(ErrorToast);
        }

        //Command Line Args
        private async void Arguments() {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Contains("help")) {
                Hide();
                Help(null, null);
                Close();
            }
            if (args.Contains("update")) {
                Hide();
                bool updateAvailable = await InstallerHelper.CheckForUpdates(this, true);
                if (updateAvailable) {
                    Update(null, null);
                } else {
                    Close();
                }
            }
            if (args.Contains("troubleshooting")) {
                //Task.Delay for Open/Close Animations
                await Task.Delay(400);
                await ShowOkDialog(str.troubleshooting, str.troubleshootingTips);
                await Task.Delay(200);
                Close();
            }
        }


        public void Help(object sender, RoutedEventArgs e) {
            //Process.Start("http://github.com/mrousavy/PipeShot#features");
            Help help = new Help();

            try {
                help.Owner = this;
            } catch {
                // ignored
            }

            help.Show();
        }

        public void EnableSave() {
            BtnSave.IsEnabled = true;
        }

        public void ShowProgressIndicator() {
            DoubleAnimation fadeIn = Animations.FadeIn;
            fadeIn.From = ProgressIndicator.Opacity;
            ProgressIndicator.BeginAnimation(OpacityProperty, fadeIn);
        }
        public void HideProgressIndicator() {
            DoubleAnimation fadeOut = Animations.FadeOut;
            fadeOut.From = ProgressIndicator.Opacity;
            ProgressIndicator.BeginAnimation(OpacityProperty, fadeOut);
        }

        public void SetProgressStatus(string text) {
            LoadingDesc.Text = text;
        }

		private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

		}
	}
}
