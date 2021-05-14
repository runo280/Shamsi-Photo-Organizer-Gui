using System.IO;
using System.Windows;
using System.Windows.Controls;
using Shamsi_Photo_Organizer.Model;
using WK.Libraries.BetterFolderBrowserNS;
using static Shamsi_Photo_Organizer.Utils.Utils;

namespace Shamsi_Photo_Organizer.View
{
    public partial class Organizer
    {
        private string _inputDir;
        private OrganizeMethod _method = OrganizeMethod.ByYear;

        public Organizer()
        {
            InitializeComponent();
        }

        private void BtnInputDir_Click(object sender, RoutedEventArgs e)
        {
            using (var betterFolderBrowser = new BetterFolderBrowser())
            {
                betterFolderBrowser.Title = Properties.Resources.ChoosePhotosPath;
                betterFolderBrowser.Multiselect = false;

                betterFolderBrowser.ShowDialog();

                string selectedFolders = betterFolderBrowser.SelectedFolder;

                //may be user come back without selecting a path
                try
                {
                    Path.GetDirectoryName(selectedFolders);
                }
                catch
                {
                    return;
                }

                LblInputDir.Text = selectedFolders;
                _inputDir = selectedFolders;
            }
        }


        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            if (_inputDir == null) return;
            var allPhotosList = GetPhotosList(_inputDir);
            var countOfValidPhotos = CountOfValidMedia(allPhotosList);
            var message =
                $"تعداد کل عکسها: {allPhotosList.Count}\n\nتعداد عکس های قابل سازماندهی: {countOfValidPhotos}";
            MessageBoxResult messageBoxResult = MessageBox.Show(message, "شروع سازماندهی", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                OrganizePhotos(allPhotosList, _method);
            }
        }

        private void RbOnChecked(object sender, RoutedEventArgs e)
        {
            switch (((RadioButton) sender).Name)
            {
                case "RbYear":
                    _method = OrganizeMethod.ByYear;
                    break;

                case "RbYearMonth":
                    _method = OrganizeMethod.ByYearMonth;
                    break;

                case "RbMonthInYear":
                    _method = OrganizeMethod.ByMonthInYear;
                    break;
            }
        }
    }
}