using System.Windows;

namespace Shamsi_Photo_Organizer.View
{
    public partial class StartPage
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void renamer_onclcik(object sender, RoutedEventArgs e)
        {
            new Renamer().Show();
        }

        private void organizer_onclcik(object sender, RoutedEventArgs e)
        {
            new Organizer().Show();
        }
    }
}