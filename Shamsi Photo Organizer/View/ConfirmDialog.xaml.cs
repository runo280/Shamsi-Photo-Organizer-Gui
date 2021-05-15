using System.Windows;

namespace Shamsi_Photo_Organizer.View
{
    public partial class ConfirmDialog
    {
        public ConfirmDialog(string firsMessage, string secondMessage)
        {
            InitializeComponent();
            LblTotal.Content = firsMessage;
            LblValid.Content = secondMessage;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            
        }
        
        public bool IsConfirmed => DialogResult != null && DialogResult.Value;
    }
}