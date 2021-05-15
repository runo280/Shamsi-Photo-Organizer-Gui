using System.Windows;

namespace Shamsi_Photo_Organizer.View
{
    public partial class ConfirmDialog
    {
        public ConfirmDialog(int total, int valid)
        {
            InitializeComponent();
            LblTotal.Content = $"تعداد کل عکس های یافت شده: {total}";
            LblValid.Content = $"تعداد کل عکس های قابل تغییر نام: {valid}";
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            
        }
        
        public bool IsConfirmed => DialogResult != null && DialogResult.Value;
    }
}