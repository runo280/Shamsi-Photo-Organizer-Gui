using System.IO;
using System.Windows;
using System.Windows.Controls;
using Shamsi_Photo_Organizer.Utils;
using WK.Libraries.BetterFolderBrowserNS;

namespace Shamsi_Photo_Organizer.View
{
    public partial class Renamer
    {
        private string _inputDir;
        private string _prefix = "Photo";

        public Renamer()
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

        private void LblPrefix_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _prefix = LblPrefix.Text.Trim().Length == 0 ? "Photo" : LblPrefix.Text;
            LblNamePreview.Content = $"{_prefix}-1398-02-25__13-25-30.jpg";
        }

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            if (_inputDir == null) return;
            var allPhotosList = PhotoUtils.GetPhotosList(_inputDir);
            var countOfValidPhotos = PhotoUtils.CountOfValidMedia(allPhotosList);

            var confirmDialog = new ConfirmDialog(allPhotosList.Count, countOfValidPhotos) {Owner = GetWindow(this)};
            if (confirmDialog.ShowDialog() != true) return;
            if (confirmDialog.IsConfirmed)
            {
                PhotoUtils.RenamePhotos(allPhotosList, _prefix);
            }
        }
    }
}