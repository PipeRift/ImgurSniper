using System.Windows.Input;

namespace PipeShot.UI {
    /// <summary>
    ///     Interaction logic for HotKeySelector.xaml
    /// </summary>
    public partial class HotKeySelector {
        public Key Key;

        public HotKeySelector() {
            InitializeComponent();
        }

        private void SelectKey(object sender, KeyEventArgs e) {
            Key = e.Key;

            DialogResult = Key != Key.Escape;
        }
    }
}