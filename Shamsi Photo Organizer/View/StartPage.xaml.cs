using System.Windows;

namespace Shamsi_Photo_Organizer.View
{
    public partial class StartPage
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public StartPage()
        {
            InitializeComponent();
        }

        private void renamer_onclcik(object sender, RoutedEventArgs e)
        {
            new Renamer {ShowInTaskbar = false, Owner = Application.Current.MainWindow}.ShowDialog();
        }

        private void organizer_onclcik(object sender, RoutedEventArgs e)
        {
            new Organizer {ShowInTaskbar = false, Owner = Application.Current.MainWindow}.ShowDialog();
        }
    }
}