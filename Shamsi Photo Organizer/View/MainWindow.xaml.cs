using System.IO;
using System.Windows;
using System.Windows.Controls;
using Shamsi_Photo_Organizer.Model;
using WK.Libraries.BetterFolderBrowserNS;

namespace Shamsi_Photo_Organizer.View
{
    public partial class MainWindow
    {
        private string _inputDir;
        private string _outputDir;
        private bool _rename;
        private OrganizeMethod _method = OrganizeMethod.ByYear;

        public MainWindow()
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

        private void BtnOutputDir_OnClick(object sender, RoutedEventArgs e)
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

                //TODO check the output dir is not subdirectory of input dir

                LblOutputDir.Text = selectedFolders;
                _outputDir = selectedFolders;
                CheckBoxCopy.IsEnabled = true;
            }
        }

        private void BtnCleanOutputDir_OnClick(object sender, RoutedEventArgs e)
        {
            LblOutputDir.Text = "";
            _outputDir = null;
            CheckBoxCopy.IsEnabled = false;
            CheckBoxCopy.IsChecked = false;
        }

        private void CheckBoxRename_OnChecked(object sender, RoutedEventArgs e)
        {
            if (LblPrefix.IsEnabled) return;
            LblPrefix.IsEnabled = true;
            _rename = true;
        }

        private void CheckBoxRename_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (!LblPrefix.IsEnabled) return;
            LblPrefix.IsEnabled = false;
            _rename = false;
        }

        private void LblPrefix_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            LblNamePreview.Content = $"{LblPrefix.Text}-1398-02-25__13-25-30.jpg";
        }

        private void BtnStart_OnClick(object sender, RoutedEventArgs e)
        {
            if (_inputDir == null) return;
            if (_rename && LblPrefix.Text.Trim().Length == 0) return;
            var list = Utils.Utils.getPhotos(_inputDir);
            var count = Utils.Utils.getValidPhotosCount(list);
            MessageBox.Show($"Photos count: {list.Count}");
            Utils.Utils.organizeFile(_inputDir, _outputDir, _rename ? LblPrefix.Text : "Photo", _rename,
                CheckBoxCopy.IsEnabled,
                _method);
        }

        private void BtnHelp_OnClick(object sender, RoutedEventArgs e)
        {
            //TODO help dialog
        }

        private void BtnAbout_OnClick(object sender, RoutedEventArgs e)
        {
            //TODO about dialog
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