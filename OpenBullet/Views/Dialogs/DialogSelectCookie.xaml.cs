using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenBulletCE.ViewModels;
using OpenBulletCE.Views.Main.Runner;
using OpenBulletCE.Views.UserControls;
using RuriLib.Models;

namespace OpenBulletCE.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for DialogSelectCookie.xaml
    /// </summary>
    public partial class DialogSelectCookie : Page
    {
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        object Caller { get; set; }
        CookieManagerViewModel vm = null;

        public DialogSelectCookie(object caller)
        {
            Caller = caller;
            vm = OB.CookieManager;
            DataContext = vm;

            InitializeComponent();
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            if (Caller.GetType() == typeof(Runner))
            {
                ((Runner)Caller).SetCookielist((Cookie)cookieListView.SelectedItem);
            }
            //else if (Caller.GetType() == typeof(UserControlWordlist))
            //{
            //    ((UserControlWordlist)Caller).Wordlist = (Wordlist)wordlistsListView.SelectedItem;
            //}
            ((MainDialog)Parent).Close();
        }

        private void listViewColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                cookieListView.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            cookieListView.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selectButton_Click(this, null);
        }

        private void ListViewItem_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                selectButton_Click(this, null);
        }

        private void importWordlistButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                folderBrowserDialog.Description = "Select path to you LOGS\r\nУкажите путь к вашим логам.";
                folderBrowserDialog.RootFolder = Environment.SpecialFolder.Desktop;

                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Add the cookie list to the runner
                        ((Runner)Caller).SetCookielist(CookieManagerViewModel.FileToCookielist(folderBrowserDialog.SelectedPath));

                        ((MainDialog)Parent).Close();
                    }
                    catch { }
                }
            }

            
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            vm.SearchString = filterTextbox.Text;
        }

        private void filterTextbox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                searchButton_Click(this, null);
        }
    }
}
