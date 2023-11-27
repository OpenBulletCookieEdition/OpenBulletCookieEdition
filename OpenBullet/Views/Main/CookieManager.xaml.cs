using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using OpenBulletCE.ViewModels;
using OpenBulletCE.Views.Dialogs;
using RuriLib.Models;
using RuriLib.Utils;

namespace OpenBulletCE.Views.Main
{
    /// <summary>
    /// Interaction logic for CookiesManager.xaml
    /// </summary>
    public partial class CookieManager : Page
    {
        private CookieManagerViewModel vm = null;
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        public CookieManager()
        {
            vm = OB.CookieManager;
            DataContext = vm;

            InitializeComponent();
        }

        public void AddCookie(Cookie cookie)
        {
            try
            {
                vm.Add(cookie);
            }
            catch (Exception e)
            {
                OB.Logger.LogError(Components.CookieManager, e.Message);
            }
        }

        #region Buttons
        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            (new MainDialog(new DialogAddCookie(this), "Add Cookies Folder")).ShowDialog();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            OB.Logger.LogInfo(Components.CookieManager, $"Deleting {cookieListView.SelectedItems.Count} references from the DB");
            foreach (var cookie in cookieListView.SelectedItems.Cast<Cookie>().ToList())
            {
                vm.Remove(cookie);
            }
            OB.Logger.LogInfo(Components.CookieManager, "Successfully deleted the wordlist references from the DB");
        }

        private void deleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            OB.Logger.LogWarning(Components.CookieManager, "Purge selected, prompting warning");

            if (MessageBox.Show("This will purge the WHOLE Cookies DB, are you sure you want to continue?", "WARNING", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                OB.Logger.LogInfo(Components.CookieManager, "Purge initiated");
                vm.RemoveAll();
                OB.Logger.LogInfo(Components.CookieManager, "Purge finished");
            }
            else { OB.Logger.LogInfo(Components.CookieManager, "Purge dismissed"); }
        }

        private void deleteNotFoundWordlistsButton_Click(object sender, RoutedEventArgs e)
        {
            OB.Logger.LogWarning(Components.CookieManager, "Deleting wordlists with missing files.");
            vm.DeleteNotFound();
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
        #endregion

        #region ListView
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

        private void wordlistListViewDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var path in files)
                {
                    try
                    {
                        // Build the cookie object
                        var name = System.IO.Path.GetFileNameWithoutExtension(path);
                        var pathAllCookieFiles = ParseCookieFiles.Parse(path);
                        var cookie = new Cookie(name, path, pathAllCookieFiles);

                        // Add the cookie to the manager
                        AddCookie(cookie);
                    }
                    catch { }
                }
            }
        }
        #endregion
    }
}
