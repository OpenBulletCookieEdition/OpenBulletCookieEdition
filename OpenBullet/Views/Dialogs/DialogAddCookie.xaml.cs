using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;
using OpenBulletCE.Views.Main;
using RuriLib.Models;

namespace OpenBulletCE.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogAddCookie.xaml
    /// </summary>
    public partial class DialogAddCookie : Page
    {
        public object Caller { get; set; }

        public DialogAddCookie(object caller)
        {
            InitializeComponent();

            Caller = caller;

        }

        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (Caller.GetType() == typeof(CookieManager))
            {
                if (nameTextbox.Text.Trim() == string.Empty) { System.Windows.Forms.MessageBox.Show("The name cannot be blank"); return; }

                var path = locationTextbox.Text;
                ((CookieManager)Caller).AddCookie(new Cookie(nameTextbox.Text, path));
            }
            ((MainDialog)Parent).Close();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();

            folderBrowserDialog.Description = "Select path to you LOGS\r\nУкажите путь к вашим логам.";
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;

            bool? show = folderBrowserDialog.ShowDialog();
            if (show != null && show == true)
            {
                locationTextbox.Text = folderBrowserDialog.SelectedPath;
                nameTextbox.Text = System.IO.Path.GetFileNameWithoutExtension(folderBrowserDialog.SelectedPath);
            }


        }
    }
}
